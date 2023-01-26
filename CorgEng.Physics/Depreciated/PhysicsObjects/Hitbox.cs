using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.Physics.Depreciated.Interfaces;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Physics.Depreciated.PhysicsObjects
{
    [Obsolete]
    public class Hitbox : IHitbox
    {

        public ILine[] HitboxLines { get; }

        public Hitbox(params ILine[] hitboxLines)
        {
            HitboxLines = hitboxLines;
        }

        public static IHitbox GetRectHitbox(float left, float down, float right, float up)
        {
            Vector<float> bl = new Vector<float>(-left, -down);
            Vector<float> br = new Vector<float>(right, -down);
            Vector<float> tl = new Vector<float>(-left, up);
            Vector<float> tr = new Vector<float>(right, up);
            return new Hitbox(
                new Line(bl, br),
                new Line(br, tr),
                new Line(tl, tr),
                new Line(bl, tl)
                );
        }

    }
}
