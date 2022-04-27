using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Textures
{
    [Dependency]
    public class TextureFactory : ITextureFactory
    {

        public ITexture CreateTexture(string texturePath)
        {
            //Calculate the file extension
            string fileExtension = texturePath.Substring(texturePath.LastIndexOf(".") + 1);
            //Load the texture
            switch (fileExtension)
            {
                case "bmp":
                    TextureBitmap bitmap = new TextureBitmap();
                    bitmap.ReadTexture(texturePath);
                    return bitmap;
            }
            //Not implemented
            throw new NotImplementedException($"Texture file extension .{fileExtension} is not supported.");
        }

        public ITextureState GetTextureFromIconState(string iconState)
        {
            return TextureCache.GetTexture(iconState);
        }
    }
}
