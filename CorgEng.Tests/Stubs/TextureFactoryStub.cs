using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.Stubs
{
    public class TextureFactoryStub : ITextureFactory
    {

        public ITexture CreateTexture(string texturePath)
        {
            //Calculate the file extension
            string fileExtension = texturePath.Substring(texturePath.LastIndexOf(".") + 1);
            //Load the texture
            switch (fileExtension)
            {
                case "bmp":
                    return null;
            }
            //Not implemented
            throw new NotImplementedException($"Texture file extension .{fileExtension} is not supported.");
        }

        public bool GetIconStateTransparency(IIcon iconState)
        {
            return false;
        }

        public ITextureState GetTextureFromIconState(IIcon iconState)
        {
            return null;
        }

    }
}
