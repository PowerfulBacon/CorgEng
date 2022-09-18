using CorgEng.GenericInterfaces.Rendering.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Textures
{
    public interface ITextureFactory
    {

        ITexture CreateTexture(string texturePath);

        ITextureState GetTextureFromIconState(IIcon iconState);

        /// <summary>
        /// Determins if an icon state has transparency
        /// </summary>
        /// <param name="iconState"></param>
        /// <returns></returns>
        bool GetIconStateTransparency(IIcon iconState);

    }
}
