using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Networking.Attributes;
using CorgEng.UtilityTypes.Vectors;
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
    /// </summary>
    public class TransformComponent : Component
    {

        /// <summary>
        /// Start with a zero value
        /// </summary>
        [NetworkSerialized]
        public Vector<float> Position { get; internal set; } = new Vector<float>(0, 0);

        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }

    }
}
