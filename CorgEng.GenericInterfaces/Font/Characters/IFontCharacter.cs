using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Font.Characters
{
    public interface IFontCharacter
    {

        /// <summary>
        /// The texture file of the font character
        /// </summary>
        ITexture TextureFile { get; }

        /// <summary>
        /// The character code of this character
        /// </summary>
        int CharacterCode { get; }

        /// <summary>
        /// The X position of this character in the texture file.
        /// In pixels.
        /// </summary>
        int TextureXPosition { get; }

        /// <summary>
        /// The Y position of this character in the texture file.
        /// In pixels.
        /// </summary>
        int TextureYPosition { get; }

        /// <summary>
        /// The width of the character in the texture file in pixels.
        /// </summary>
        int TextureWidth { get; }

        /// <summary>
        /// The height of the character in the texture file in pixels.
        /// </summary>
        int TextureHeight { get; }

        /// <summary>
        /// The X offset of the character in pixels. Relative to the width of the texture file.
        /// </summary>
        int CharacterXOffset { get; }

        /// <summary>
        /// The Y offset of the character in pixels. Relative to the height of the texture file.
        /// </summary>
        int CharacterYOffset { get; }

        /// <summary>
        /// The X advance of the characcter in pixels (How far the text printer moves forward when we print this character).
        /// Relative to the width of the texture file.
        /// </summary>
        int CharacterXAdvance { get; }

    }
}
