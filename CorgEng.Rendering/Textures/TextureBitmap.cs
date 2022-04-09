using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.Rendering.Textures
{
    public class TextureBitmap : ITexture
    {

        [UsingDependency]
        private static ILogger Log;

        private enum BitmapCompressionMethod
        {
            BI_RGB = 0,
            BI_RLE8 = 1,
            BI_RLE4 = 2,
            BI_BITFIELDS = 3,
            BI_JPEG = 4,
            BI_PNG = 5,
            BI_ALPHABITFIELDS = 6,
            BI_CMYK = 11,
            BI_CMYKRLE8 = 12,
            BI_CMYKRLE4 = 13,
        }

        public int Width { get; private set; }

        public int Height { get; private set; }

        public uint TextureID { get; private set; }

        public unsafe void ReadTexture(string fileName)
        {
            //Get the contents of the bitmap file
            byte[] fileContents = File.ReadAllBytes(fileName);

            //If the file is not a BMP
            if (fileContents == null || fileContents.Length < 54)
            {
                throw new ArgumentException($"The file provided is not a valid bitmap. File: {fileName}");
            }

            //Check the file to make sure it is actually a BMP
            byte[] header = fileContents.Take(54).ToArray();
            //Bitmaps start with BM
            if (header[0] != 'B' || header[1] != 'M')
            {
                throw new ArgumentException($"The file provided ({fileName}) is not a bitmap image.");
            }

            //Load the image metadata
            //See https://en.wikipedia.org/wiki/BMP_file_format for more details.
            //Texture data index pointer is stored at 0x0A in BMP images
            //We *should* also handle different types of compression
            int textureDataIndex = header[0x0A];
            //Calculates the width and height of the image. (4 bytes are used for width and height of images)
            //TODO: BytesToInt doesn't work
            //Width = ByteConverter.BytesToInt(header, 0x12, 2);
            //Height = ByteConverter.BytesToInt(header, 0x16, 2);
            Width = (header[0x12] * 4096) + (header[0x13] * 256) + (header[0x14] * 16) + (header[0x15]);
            Height = (header[0x16] * 4096) + (header[0x17] * 256) + (header[0x18] * 16) + (header[0x19]);

            BitmapCompressionMethod compressionMethod = (BitmapCompressionMethod)header[0x1E];

            //If the texture ata index was not included
            //we start reading from 54.
            if (textureDataIndex == 0)
            {
                textureDataIndex = 54;
                Log.WriteLine($"{fileName} did not provide a textureDataIndex, setting it to 54.", LogType.WARNING);
            }

            //Read the image data
            byte[] data = fileContents.Skip(textureDataIndex).ToArray();

            //Bind the texture
            //Hands the texture data to an openGL buffer to be stored on the GPU
            //DEBUG
            glActiveTexture(GL_TEXTURE0);
            TextureID = glGenTexture();
            glBindTexture(GL_TEXTURE_2D, TextureID);
            fixed (byte* dataPointer = &data[0])
            {
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, Width, Height, 0, GL_BGRA, GL_UNSIGNED_BYTE, dataPointer);
            }
            //Use nearest neighbor sampling to not blur the pixel-based graphics.
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);

            Log.WriteLine($"Texture {fileName} loaded successfully. (Mode: {compressionMethod}) (Thread: {Thread.CurrentThread.ManagedThreadId}) (Texture uint: {TextureID})", LogType.DEBUG);
        }

    }
}
