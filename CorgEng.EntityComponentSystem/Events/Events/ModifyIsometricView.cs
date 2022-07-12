using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Events.Events
{
    public class ModifyIsometricView : IEvent
    {

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public ModifyIsometricView(double x, double y, double z, double width, double height)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Height = height;
        }
    }
}
