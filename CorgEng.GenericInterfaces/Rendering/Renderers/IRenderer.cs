using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Renderers
{
    public interface IRenderer
    {

        uint NetworkIdentifier { get; }

        void Initialize();

        void Render(ICamera camera);

    }
}
