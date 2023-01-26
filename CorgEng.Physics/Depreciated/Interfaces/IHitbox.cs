using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.Interfaces
{
    [Obsolete]
    public interface IHitbox
    {

        /// <summary>
        /// The lines that make up the hitbox.
        /// Relative to the position of the physics object.
        /// </summary>
        ILine[] HitboxLines { get; }

    }
}
