using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Cameras.Isometric
{
    public interface IIsometricCamera : ICamera
    {

        float X { get; set; }

        float Y { get; set; }

        float Width { get; set; }

        float Height { get; set; }

    }
}
