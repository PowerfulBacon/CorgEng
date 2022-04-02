using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Textures
{
    public interface ITexture
    {

        //The width of the texture files in pixels
        int Width { get; }

        //The height in pixels of the texture file
        int Height { get; }

        //The reference to the loaded texture
        uint TextureID { get; }

        /// <summary>
        /// Read a texture file and store it in openGL.
        /// </summary>
        /// <param name="fileName">The filename of the texture to load</param>
        unsafe void ReadTexture(string fileName);

    }
}
