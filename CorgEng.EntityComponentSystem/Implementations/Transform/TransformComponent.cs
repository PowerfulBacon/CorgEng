using CorgEng.EntityComponentSystem.Components;
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
        public CVar<Vector<float>> Position { get; internal set; } = new CVar<Vector<float>>(new Vector<float>(0, 0));

        /// <summary>
        /// Rotation property of this transform
        /// </summary>
        [NetworkSerialized(prototypeInclude = false)]
        public CVar<Vector<float>> Rotation { get; internal set; } = new CVar<Vector<float>>(new Vector<float>(0));

        public override TransformComponent Transform => this;

    }
}
