using CorgEng.GenericInterfaces.Font.Characters;
using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Font.Characters
{
    internal class FontCharacter : IFontCharacter
    {

        public ITexture TextureFile { get; }

        public int CharacterCode { get; }

        public int TextureXPosition { get; }

        public int TextureYPosition { get; }

        public int TextureWidth { get; }

        public int TextureHeight { get; }

        public int CharacterXOffset { get; }

        public int CharacterYOffset { get; }

        public int CharacterXAdvance { get; }

        public FontCharacter(ITexture textureFile, int characterCode, int textureXPosition, int textureYPosition, int textureWidth, int textureHeight, int characterXOffset, int characterYOffset, int characterXAdvance)
        {
            TextureFile = textureFile;
            CharacterCode = characterCode;
            TextureXPosition = textureXPosition;
            TextureYPosition = textureYPosition;
            TextureWidth = textureWidth;
            TextureHeight = textureHeight;
            CharacterXOffset = characterXOffset;
            CharacterYOffset = characterYOffset;
            CharacterXAdvance = characterXAdvance;
        }

    }
}
