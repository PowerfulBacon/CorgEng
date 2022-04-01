using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Shaders
{
    public interface IShaderSet
    {

        void LoadShaders(string name);

        void AttachShaders(uint programUint);

        void DetatchShaders(uint programUint);

        void DeleteShaders();

    }
}
