using CorgEng.GenericInterfaces.Rendering.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject
{
    public interface IUserInterfaceRenderObjectFactory
    {

        IUserInterfaceRenderObject CreateUserInterfaceRenderObject(uint textureFile, float textureX, float textureY, float textureWidth, float textureHeight);

    }
}
