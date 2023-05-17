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

        public int Width => 0;

        public int Height => 0;

        public void DoRender(Action preRenderAction = null)
        {
            return;
        }

        public void DrawToBuffer()
        {
            return;
        }

        public void DrawToBuffer(uint buffer, int drawX, int drawY, int bufferWidth, int bufferHeight)
        {
            return;
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
