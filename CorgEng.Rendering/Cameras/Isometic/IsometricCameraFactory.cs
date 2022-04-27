using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Cameras.Isometic
{
    [Dependency]
    public class IsometricCameraFactory : IIsometricCameraFactory
    {

        public IIsometricCamera CreateCamera()
        {
            return new IsometricCamera();
        }

    }
}
