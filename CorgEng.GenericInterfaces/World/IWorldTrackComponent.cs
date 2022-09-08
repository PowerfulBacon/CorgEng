using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.World
{
    public interface IWorldTrackComponent : IComponent
    {

        /// <summary>
        /// The position of this component in the
        /// array at the current position.
        /// </summary>
        int ContentsIndexPosition { get; set; }

        /// <summary>
        /// The tracked location of this component.
        /// </summary>
        IVector<int> ContentsLocation { get; set; }

    }
}
