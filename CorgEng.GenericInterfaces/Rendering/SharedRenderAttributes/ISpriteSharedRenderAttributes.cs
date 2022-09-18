using CorgEng.GenericInterfaces.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes
{
    public interface ISpriteSharedRenderAttributes : ISharedRenderAttributes
    {

        uint SpriteTextureUint { get; set; }

        //float Layer { get; set; }

        int VertexCount { get; }

        IModel Model { get; }

    }
}
