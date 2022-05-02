using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.UserInterface.Rendering.RenderObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Rendering.UserinterfaceRenderer
{
    [Dependency]
    internal class UserInterfaceRenderObjectFactory : IUserInterfaceRenderObjectFactory
    {
        public IUserInterfaceRenderObject CreateUserInterfaceRenderObject(uint textureFile, float textureX, float textureY, float textureWidth, float textureHeight)
        {
            return new UserInterfaceRenderObject(textureFile, textureX, textureY, textureWidth, textureHeight);
        }

    }
}
