using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IColour
    {

        /// <summary>
        /// Red value.
        /// Ranges from 0 to 1
        /// </summary>
        public float Red { get; set; }

        /// <summary>
        /// Green value.
        /// Ranges from 0 to 1
        /// </summary>
        public float Green { get; set; }

        /// <summary>
        /// Blue value.
        /// Ranges from 0 to 1
        /// </summary>
        public float Blue { get; set; }

    }
}
