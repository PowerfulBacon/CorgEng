using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GJ2022.DmiIconConversionUtility
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        byte[] bytes;

        /// <summary>
        /// Run the conversion utility
        /// </summary>
        private void ConvertFileButton_Click(object sender, EventArgs e)
        {

            try
            {

                Console.WriteLine("Allocated console");

                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;

                Console.WriteLine("Disposed of existing images");

                string outputDirectory = $"{Directory.GetCurrentDirectory()}\\temp";
                string selectedFilePath = "";
                string safeFileName = "";

                //Step 0: Get the file
                using (OpenFileDialog fileDialog = new OpenFileDialog())
                {
                    fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                    fileDialog.Filter = "dmi files (*.dmi)|*.dmi|All Files|*.*";
                    fileDialog.FilterIndex = 2;
                    fileDialog.RestoreDirectory = false;

                    if (fileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    selectedFilePath = fileDialog.FileName;
                    safeFileName = fileDialog.SafeFileName;
                }

                string fileRawName = safeFileName.Substring(0, safeFileName.Length - 4);

                //Read the DMI texture
                TextureDmi texture = new TextureDmi(selectedFilePath);

                listBox1.Items.Clear();

                //Generate the output bitmap
                Directory.CreateDirectory(outputDirectory);
                FileStream createdBitmapFile = File.Create($"{outputDirectory}/{fileRawName}.bmp");

                int newWidth = (int)Math.Pow(2, (int)Math.Log(texture.width - 1, 2) + 1);
                int newHeight = (int)Math.Pow(2, (int)Math.Log(texture.height - 1, 2) + 1);
                int xBorder = newWidth - texture.width;
                int yBorder = newHeight - texture.height;

                int totalDataLength = 4 * newWidth * newHeight;

                int dataIndex = 150;
                byte[] sizeInBytes = BitConverter.GetBytes(totalDataLength + dataIndex);
                byte[] startingPointer = BitConverter.GetBytes((uint)dataIndex);  //TODO
                byte[] widthArray = BitConverter.GetBytes(newWidth);
                byte[] heightArray = BitConverter.GetBytes(newHeight);

                byte[] headerData = new byte[dataIndex];

                //Insert the header
                headerData[0x00] = (byte)'B';
                headerData[0x01] = (byte)'M';
                //Insert the size in bytes
                sizeInBytes.CopyTo(headerData, 0x02);
                //Insert the pointer to the start of the data
                startingPointer.CopyTo(headerData, 0x0A);

                //DIB Header

                //Insert the size of the header
                BitConverter.GetBytes((uint)124).CopyTo(headerData, 0x0E);
                //Insert the width and height
                widthArray.CopyTo(headerData, 0x12);
                heightArray.CopyTo(headerData, 0x16);
                //Insert 1 for the number of colour planes
                headerData[0x1A] = 1;
                //Number of bits per pixel (32, 8 for each in RGBA)
                headerData[0x1C] = 32;
                //Compression method being used (BI_BITFIELDS)
                headerData[0x1E] = 3;
                //Image size for raw bitmap data (0)
                headerData[0x22] = 0;
                //Horizontal and vertical resolution
                headerData[0x26] = 196;
                headerData[0x27] = 14;
                headerData[0x2A] = 196;
                headerData[0x2B] = 14;

                //Add in this random stuff idk what it does
                headerData[56] = 255;
                headerData[59] = 255;
                headerData[62] = 255;
                headerData[69] = 255;
                headerData[70] = 32;
                headerData[71] = 110;
                headerData[72] = 105;
                headerData[73] = 87;
                headerData[138] = 255;
                headerData[143] = 255;
                headerData[148] = 255;

                //Write the first 54 bytes to the file
                createdBitmapFile.Write(headerData, 0, headerData.Length);

                for (int y = 0; y < newHeight; y++)
                {
                    for (int x = 0; x < newWidth; x++)
                    {
                        if (x >= newWidth - xBorder || y < yBorder)
                        {
                            createdBitmapFile.WriteByte(0);
                            createdBitmapFile.WriteByte(0);
                            createdBitmapFile.WriteByte(0);
                            createdBitmapFile.WriteByte(255);
                        }
                        else
                        {
                            int i = 4 * (x + (newHeight - y - 1) * texture.width);
                            createdBitmapFile.WriteByte(texture.data[i + 2]);
                            createdBitmapFile.WriteByte(texture.data[i + 1]);
                            createdBitmapFile.WriteByte(texture.data[i + 0]);
                            createdBitmapFile.WriteByte(texture.data[i + 3]);
                        }
                    }
                }

                createdBitmapFile.Seek(0, SeekOrigin.Begin);

                //Load the file into a display
                for (int j = 0; j < Math.Min(texture.data.Length + dataIndex, 1000); j++)
                {
                    listBox1.Items.Add($"{j}\t:{createdBitmapFile.ReadByte()}");
                }

                //Store temp
                bytes = new byte[1000 + 54];
                headerData.CopyTo(bytes, 0);
                texture.data.Take(1000).ToArray().CopyTo(bytes, 54);

                //Dispose
                createdBitmapFile.Close();
                createdBitmapFile.Dispose();

                //IMAGE FILE CREATED SUCCESSFULLY, CREATE JSON FILE NOW
                string jsonText = @"{
    ""textures"": [
";


                foreach (string iconStateName in texture.iconStates.Keys)
                {
                    IconState iconState = texture.iconStates[iconStateName];
                    jsonText += @"      {
";
                    jsonText += $@"          ""id"": ""{fileRawName}.{iconStateName}"",
";
                    jsonText += $@"          ""file"": ""{fileRawName}.bmp"",
";
                    jsonText += $@"          ""width"": {texture.iconWidth},
";
                    jsonText += $@"          ""height"": {texture.iconHeight},
";
                    //Insert direction
                    switch (iconState.dirs)
                    {
                        case 4:
                            jsonText += $@"          ""directional"": ""CARDINAL"",
";
                            break;
                        case 8:
                            jsonText += $@"          ""directional"": ""CARDINAL_DIAGONAL"",
";
                            break;
                    }
                    //Insert other stuff we don't use yet
                    if (iconState.frames > 1)
                        jsonText += $@"          ""animation_frames"": {iconState.frames},
";
                    if (iconState.rewind)
                        jsonText += $@"          ""animation_rewind"": true
";
                    if (iconState.loop)
                        jsonText += $@"          ""animation_loop"": true,
";
                    if (iconState.delay?.Length > 0)
                    {
                        jsonText += $@"          ""animation_delays"": [
";
                        foreach (float delay in iconState.delay)
                        {
                            jsonText += $@"             {delay},
";
                        }
                        jsonText += $@"          ],
";
                    }
                    if (iconState.movement)
                        jsonText += $@"          ""is_movement_state"": true,
";
                    jsonText += $@"          ""index_x"": {iconState.spriteSheetPos % texture.spritesheet_width},
";
                    jsonText += $@"          ""index_y"": {iconState.spriteSheetPos / texture.spritesheet_height}
";
                    //Close the json
                    jsonText += @"      },
";
                }
                jsonText = jsonText.Substring(0, jsonText.Length - 1);
                jsonText += @"  ]
}
";
                File.WriteAllText($"{outputDirectory}/temp.json", jsonText);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "oh dear...");
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelChar.Text = $"Char: {((char)bytes[listBox1.SelectedIndex])}";
            label2Byte.Text = $"Int16: {BitConverter.ToUInt16(bytes, listBox1.SelectedIndex)}";
            label4Byte.Text = $"Int32: {BitConverter.ToUInt32(bytes, listBox1.SelectedIndex)}";
        }

        private void buttonReadBmp_Click(object sender, EventArgs e)
        {
            try
            {

                Console.WriteLine("Allocated console");

                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;

                Console.WriteLine("Disposed of existing images");

                string outputDirectory = $"{Directory.GetCurrentDirectory()}\\temp";
                string selectedFilePath = "";

                //Step 0: Get the file
                using (OpenFileDialog fileDialog = new OpenFileDialog())
                {
                    fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                    fileDialog.Filter = "dmi files (*.dmi)|*.dmi|All Files|*.*";
                    fileDialog.FilterIndex = 2;
                    fileDialog.RestoreDirectory = false;

                    if (fileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    selectedFilePath = fileDialog.FileName;
                }

                byte[] read = File.ReadAllBytes(selectedFilePath);

                listBox1.Items.Clear();

                //Load the file into a display
                for (int i = 0; i < Math.Min(read.Length, 10000); i++)
                {
                    listBox1.Items.Add($"{i}\t:{read[i]}");
                }

                //Store temp
                bytes = new byte[10000 + 54];
                read.Take(10000 + 54).ToArray().CopyTo(bytes, 0);

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "oh dear...");
            }
        }
    }
}
