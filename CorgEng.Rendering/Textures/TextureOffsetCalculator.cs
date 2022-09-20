using CorgEng.Core.Dependencies;
using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Textures
{
    [Dependency]
    internal class TextureOffsetCalculator : ITextureOffsetCalculator
    {

        public IVector<float> GetTextureOffset(ITextureState textureState, DirectionalState directionalState)
        {
            IVector<float> returnValue = new Vector<float>(textureState.OffsetX, textureState.OffsetY);
            //Increment the offset
            switch (textureState.DirectionalMode)
            {
                //Up, Down, Right, Left
                case DirectionalModes.CARDINAL:
                    switch (directionalState)
                    {
                        case DirectionalState.SOUTH:
                            returnValue.X += textureState.OffsetWidth;
                            break;
                        case DirectionalState.EAST:
                            returnValue.X += 2 * textureState.OffsetWidth;
                            break;
                        case DirectionalState.WEST:
                            returnValue.X += 3 * textureState.OffsetWidth;
                            break;
                    }
                    break;
                //Unconfirmed
                case DirectionalModes.CARDINAL_DIAGONAL:
                    throw new NotImplementedException("Cardinal diagonals hasn't been implemented yet.");
                //None = 0
                //Up = 1
                //Down = 2
                //Right = 4
                //left = 8
                case DirectionalModes.FULL:
                    returnValue.X += (int)directionalState * textureState.OffsetWidth;
                    break;
            }
            //Calculate the Y value
            float xOverflow = returnValue.X;
            returnValue.Y += ((int)Math.Floor(xOverflow)) * textureState.OffsetHeight;
            returnValue.X = returnValue.X % 1;
            //Calculate the actual correct position
            return returnValue;
        }

    }
}
