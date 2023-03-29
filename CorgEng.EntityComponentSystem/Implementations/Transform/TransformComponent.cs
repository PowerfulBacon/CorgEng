using CorgEng.EntityComponentSystem.Components;
using CorgEng.EntityComponentSystem.Components.ComponentVariables.Networking;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.UtilityTypes.Vectors;
using CorgEng.World.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    /// <summary>
    /// Represents the transform of the attached entity, allowing it to be rendered
    /// with different positions, scales or rotations.
    /// This component is not networked, for a networked transform variant, use
    /// NetworkedTransformComponent from CorgEng.Networking.
    /// Inherets from trackcomponent.
    /// </summary>
    public class TransformComponent : TrackComponent
    {

        public TransformComponent() : base("_world")
        { }

        /// <summary>
        /// Start with a zero value
        /// </summary>
        [NetworkSerialized(prototypeInclude = false)]
        public NetCVar<Vector<float>, TransformComponent> Position { get; internal set; } = new NetCVar<Vector<float>, TransformComponent>(new Vector<float>(0, 0))
            .SetPrototypeSerialised(false);

        /// <summary>
        /// Rotation property of this transform
        /// </summary>
        [NetworkSerialized(prototypeInclude = false)]
        public NetCVar<Vector<float>, TransformComponent> Rotation { get; internal set; } = new NetCVar<Vector<float>, TransformComponent>(new Vector<float>(0))
            .SetPrototypeSerialised(false);

        public override TransformComponent Transform => this;

    }
}
