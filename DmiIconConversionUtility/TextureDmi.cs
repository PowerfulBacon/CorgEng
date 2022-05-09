using libpngsharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace GJ2022.DmiIconConversionUtility
{
    class TextureDmi
    {

        //Scale of the texture in the loaded image
        public int texture_scale = 32;

        //Bitmap
        protected byte[] header = new byte[54];

        //Width and height of the texture file (Full)
        public int width;
        public int height;

        // Amount of 32x32 blocks per width (255 / 32 (8))
        public int spritesheet_width;
        public int spritesheet_height;

        public int iconWidth = 0;
        public int iconHeight = 0;

        //The data of the loaded png image.
        public byte[] data;

        //How icon states relate to which point on the spritesheet.
        public Dictionary<string, IconState> iconStates = new Dictionary<string, IconState>();

        public TextureDmi(string fileName)
        {

            Console.WriteLine("Starting...");

            //The stream we will be using.
            Stream stream;

            //Open a steram to read the texture.
            try
            {
                stream = File.OpenRead($"{fileName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to read image {fileName}.");
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine("Attempting to load PngDecoder");

            //Create the PNG decoder
            PngDecoder decoder = new PngDecoder(stream);

            Console.WriteLine(Marshal.PtrToStringAuto(decoder.infoPtr));

            //Load width and height
            height = decoder.Height;
            width = decoder.Width;

            Console.WriteLine($"PNG Image loaded with colour type: {decoder.ColorType}");
            if (decoder.ColorType != PngColorType.RGBA)
            {
                Console.WriteLine("INVALID PNG COLOUR TYPE, ATTEMPTING TO CONVERT.");
                decoder.TransformPaletteToRgb();
                decoder.SaveTransformations();
                Console.WriteLine($"PNG Image loaded with colour type: {decoder.ColorType}");
            }

            //Read the image
            data = new byte[decoder.DecompressedSize];
            decoder.Decode(data);
            //decoder.TransformSetBgr();


            //Begin reading DMI data.
            string dmi_data = decoder.GetPngText();

            //Cache
            string currentStateName = string.Empty;
            IconState currentState = new IconState();

            iconWidth = 0;
            iconHeight = 0;

            int currentSpriteSheetPointer = 0;

            //Console.WriteLine(dmi_data);

            //Read in the data
            foreach (string line in dmi_data.Split('\n'))
            {
                if (currentStateName == string.Empty)
                {
                    //Metadata reading
                    if (line.Contains("width = "))
                    {
                        iconWidth = int.Parse(line.Substring(line.IndexOf("width = ") + 8));
                    }
                    else if (line.Contains("height = "))
                    {
                        iconHeight = int.Parse(line.Substring(line.IndexOf("height = ") + 9));
                    }
                    //Start
                    else if (line.StartsWith("state = "))
                    {
                        int firstIndex = line.IndexOf('"') + 1;
                        int lastIndex = line.LastIndexOf('"');
                        currentStateName = line.Substring(firstIndex, lastIndex - firstIndex);
                        if (currentStateName == string.Empty)
                        {
                            currentStateName = "default";
                        }
                    }
                }
                else
                {
                    //Invalid line
                    if (!line.Contains(" = "))
                        continue;
                    //Read variables
                    string trimmedLine = line.TrimStart();
                    int indexOfEquals = trimmedLine.IndexOf(" = ");
                    string variable = trimmedLine.Substring(0, indexOfEquals);
                    string value = trimmedLine.Substring(indexOfEquals + 3);
                    //Switch
                    switch (variable)
                    {
                        case "dirs":
                            currentState.dirs = int.Parse(value);
                            break;
                        case "frames":
                            currentState.frames = int.Parse(value);
                            break;
                        case "rewind":
                            if (value == "1")
                                currentState.rewind = true;
                            break;
                        case "loop":
                            if (value == "1")
                                currentState.loop = true;
                            break;
                        case "movement":
                            if (value == "1")
                            {
                                currentState.movement = true;
                                currentStateName = $"{currentStateName}-mov";
                            }
                            break;
                        case "delay":
                            string[] delays = value.Split(',');
                            float[] delayLengths = new float[delays.Length];
                            for (int i = 0; i < delays.Length; i++)
                            {
                                delayLengths[i] = float.Parse(delays[i]);
                            }
                            currentState.delay = delayLengths;
                            break;
                        case "state":
                            //Lets add our icon
                            if (!iconStates.ContainsKey(currentStateName))
                            {
                                iconStates.Add(currentStateName, currentState);
                            }
                            else
                            {
                                Console.WriteLine($"DUPLICATE ICON_STATE: {currentStateName} IN FILE: {fileName}");
                            }
                            //Move along all our animation states
                            if (currentState.delay != null)
                            {
                                currentSpriteSheetPointer += currentState.delay.Length * (int)currentState.dirs;
                            }
                            else
                            {
                                //Move 1 along
                                currentSpriteSheetPointer += (int)currentState.dirs;
                            }
                            //Make the next icon state
                            currentStateName = value.Replace("\"", "");
                            if (currentStateName == string.Empty)
                            {
                                currentStateName = "(no name)";
                            }
                            currentState = new IconState();
                            currentState.spriteSheetPos = currentSpriteSheetPointer;
                            break;
                        default:
                            Console.WriteLine($"INVALID DMI VARIABLE: {variable} | {line}");
                            break;
                    }
                }
            }

            //Add the final one.
            iconStates.Add(currentStateName, currentState);

            //Check for invalid width/height setups
            if (iconWidth != iconHeight)
            {
                Console.WriteLine($"ERROR: Iconwidth != iconheight in {fileName}.dmi, they should both be the same");
            }

            //Set spritesheet scale and texture scale.
            this.texture_scale = iconWidth;
            spritesheet_width = width / iconWidth;
            spritesheet_height = height / iconHeight;

            Console.WriteLine($"Texture ({fileName}) Loaded with size {width}x{height} with icon size {iconWidth}x{iconHeight}.");
        }

    }
}
