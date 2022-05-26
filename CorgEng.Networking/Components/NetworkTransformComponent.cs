using CorgEng.EntityComponentSystem.Implementations.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Components
{
    /// <summary>
    /// A derivative of transform component that will allow for the transform
    /// to be synced to clients.
    /// </summary>
    public sealed class NetworkTransformComponent : TransformComponent
    {
    }
}
