using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Textures
{
    public interface ITextureOffsetCalculator
    {

        /// <summary>
        /// Get the texture offset based off of the provided parameters
        /// </summary>
        /// <param name="textureState"></param>
        /// <param name="directionalState"></param>
        /// <returns></returns>
        IVector<float> GetTextureOffset(ITextureState textureState, DirectionalState directionalState);

    }
}
