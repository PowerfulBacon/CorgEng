using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Anchors
{
    public interface IAnchorDetails
    {

        /// <summary>
        /// The side that this anchor is relative to
        /// </summary>
        AnchorDirections AnchorSide { get; }

        /// <summary>
        /// The unit system to use
        /// </summary>
        AnchorUnits AnchorUnits { get; }

        /// <summary>
        /// The distance from the specified position
        /// </summary>
        double AnchorOffset { get; }

    }
}
