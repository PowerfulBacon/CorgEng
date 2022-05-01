using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Core
{
    public interface IRenderCore
    {

        void Initialize();

        void PerformRender();

        void Resize(int width, int height);

    }
}
