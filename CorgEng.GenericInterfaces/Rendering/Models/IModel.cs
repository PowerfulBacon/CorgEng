using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Models
{
    public interface IModel
    {

        uint VertexBuffer { get; }

        uint UvBuffer { get; }

    }
}
