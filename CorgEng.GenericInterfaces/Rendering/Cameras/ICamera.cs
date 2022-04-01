using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering
{
    public interface ICamera
    {

        IMatrix GetProjectionMatrix(float windowWidth, float windowHeight);

        IMatrix GetViewMatrix(float windowWidth, float windowHeight);

    }
}
