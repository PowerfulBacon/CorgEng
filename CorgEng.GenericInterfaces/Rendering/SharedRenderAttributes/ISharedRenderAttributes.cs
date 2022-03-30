using CorgEng.GenericInterfaces.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes
{
    public interface ISharedRenderAttributes
    {

        /// <summary>
        /// The model used by this set of objects with shared render attributes.
        /// </summary>
        IModel Model { get; }

    }
}
