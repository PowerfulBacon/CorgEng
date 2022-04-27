using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Rendering.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.Shaders
{
    [Dependency]
    public class ShaderFactory : IShaderFactory
    {

        public IShaderSet CreateShaderSet(string shaderName)
        {
            ShaderSet set = new ShaderSet();
            set.LoadShaders(shaderName);
            return set;
        }

    }
}
