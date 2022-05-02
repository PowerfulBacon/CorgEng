using CorgEng.GenericInterfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests.Stubs
{
    internal class RenderCoreStub : IRenderCore
    {
        public void DrawBufferToScreen()
        {
            return;
        }

        public void Initialize()
        {
            return;
        }

        public void PerformRender()
        {
            return;
        }

        public void PreRender()
        {
            throw new NotImplementedException();
        }

        public void Resize(int width, int height)
        {
            return;
        }
    }
}
