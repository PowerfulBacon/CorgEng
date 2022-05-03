using CorgEng.Core.Dependencies;
using CorgEng.Font.Characters;
using CorgEng.GenericInterfaces.Font.Characters;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.Rendering.Textures.Bitmap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CorgEng.Font.Fonts
{
    internal class Font : IFont
    {

        private static Regex whitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        [UsingDependency]
        private static ITextureFactory BitmapFactory;

        [UsingDependency]
        private static ILogger Logger;

        private IFontCharacter[] fontCharacters;

        public Font(string fontName)
        {
            LoadFontCharacters(fontName);
        }

        /// <summary>
        /// Locate and load the file name.
        /// </summary>
        private void LoadFontCharacters(string fileName)
        {
            //Load them into a dictionary, so we can store them in an array later
            List<IFontCharacter> loadedCharacterCache = new List<IFontCharacter>();
            //Store the maximum character value for setting our array length. We need fast access to characters.
            int maximumCharacterCode = 0;
            //Get the file and read its contents
            //These shouldn't be huge, so we should have enough memory for this.
            string[] lines = File.ReadAllLines($"Content/Font/{fileName}.fnt");
            //Get the texture file
            ITexture textureFile = null;
            //Begin parsing
            foreach (string line in lines)
            {
                //Ignore lines that we can't process
                if (!line.StartsWith("page") && !line.StartsWith("char"))
                    continue;
                //Load stuff into a dictionary
                Dictionary<string, string> lineParameters = new Dictionary<string, string>();
                //Split the line by some regex expressing
                string[] splitLine = whitespaceRegex.Split(line);
                Logger.WriteLine($"Located {splitLine.Length} things");
                //Work out each one
                foreach (string linepart in splitLine)
                {
                    int positionOfEquals = linepart.IndexOf("=");
                    //Continue if we cannot locate an equals sign
                    if (positionOfEquals == -1)
                        continue;
                    Logger.WriteLine(linepart);
                    //Load into a dictionary
                    string trimmedLine = linepart.Trim();
                    lineParameters.Add(trimmedLine.Substring(0, positionOfEquals), trimmedLine.Substring(positionOfEquals + 1));
                }
                //Perform important calculations
                if (line.StartsWith("page"))
                {
                    string fontTextureFileName = lineParameters["file"].Substring(1, lineParameters["file"].Length - 2);
                    Logger?.WriteLine($"Accessing font texture file at {fontTextureFileName}", LogType.DEBUG);
                    textureFile = BitmapFactory.CreateTexture($"Content/Font/{fontTextureFileName}");
                }
                else if (line.StartsWith("char") && !line.StartsWith("chars"))
                {
                    int characterId = int.Parse(lineParameters["id"]);
                    maximumCharacterCode = Math.Max(maximumCharacterCode, characterId);
                    loadedCharacterCache.Add(new FontCharacter(
                        textureFile,
                        characterId,
                        int.Parse(lineParameters["x"]),
                        int.Parse(lineParameters["y"]),
                        int.Parse(lineParameters["width"]),
                        int.Parse(lineParameters["height"]),
                        int.Parse(lineParameters["xoffset"]),
                        int.Parse(lineParameters["yoffset"]),
                        int.Parse(lineParameters["xadvance"])
                        ));
                }
            }
            //Complete the character cache
            fontCharacters = new IFontCharacter[maximumCharacterCode + 1];
            //Insert characters
            foreach (IFontCharacter fontCharacter in loadedCharacterCache)
            {
                fontCharacters[fontCharacter.CharacterCode] = fontCharacter;
            }
            //Complete loading
            Logger?.WriteLine($"Loaded font {fileName}, with {loadedCharacterCache.Count} characters.", LogType.DEBUG);
        }

        /// <summary>
        /// Get a specified character.
        /// </summary>
        public IFontCharacter GetCharacter(int code)
        {
            //WILL RETURN INVALID CHARACTERS //TODO//
            return fontCharacters[code];
        }

    }
}
