using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IMatrix
    {

        event EventHandler OnChange;

        int X { get; }
        int Y { get; }

        float this[int x, int y] { get; set; }

        unsafe float* GetPointer();

    }
}
