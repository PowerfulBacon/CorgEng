using CorgEng.Physics.Depreciated.PhysicsObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.Managers
{
    [Obsolete]
    public class PhysicsMap
    {

        // Unoptimised, list all physics object
        public List<PhysicsObject> PhysicsObjects { get; set; } = new List<PhysicsObject>();

        public void Reset()
        {
            PhysicsObjects.Clear();
        }

    }
}
