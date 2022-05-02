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
        public uint FrameBufferUint => 0;

        public uint RenderTextureUint => 0;

        public void DoRender()
        {
            return;
        }

        public void DrawToBuffer()
        {
            return;
        }

        public void DrawToBuffer(uint buffer)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            return;
        }

        public void Resize(int width, int height)
        {
            return;
        }
    }
}
