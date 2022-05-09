using System;

namespace libpngsharp
{
    /// <summary>
    /// The color types of a PNG image.
    /// </summary>
    [Flags]
    public enum PngColorType : byte
    {
        /// <summary>
        /// The image uses a color palette.
        /// </summary>
        Palette = 1,

        /// <summary>
        /// The image uses true colors.
        /// </summary>
        Color = 2,

        /// <summary>
        /// The image has an alpha channel.
        /// </summary>
        Alpha = 4,

        /// <summary>
        /// The image is a grayscale image.
        /// </summary>
        Gray = 0,

        /// <summary>
        /// The image uses a color palette.
        /// </summary>
        ColorPalette = (Color | Palette),

        /// <summary>
        /// The image uses a RGB pixel format.
        /// </summary>
        RGB = Color,

        /// <summary>
        /// The image uses a RGBA pixel format.
        /// </summary>
        RGBA = (Color | Alpha),

        /// <summary>
        /// The image uses a grayscale and has an alpha channel.
        /// </summary>
        GrayAlpha = (Alpha),
    }
}