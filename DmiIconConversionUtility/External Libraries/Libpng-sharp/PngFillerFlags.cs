namespace libpngsharp
{
    /// <summary>
    /// Specifies options for adding a filler byte to a pixel.
    /// </summary>
    public enum PngFillerFlags : int
    {
        /// <summary>
        /// Adds the filler at the start of the pixel array.
        /// </summary>
        Before = 0,

        /// <summary>
        /// Adds the filler at the end of the pixel array.
        /// </summary>
        After = 1
    }
}