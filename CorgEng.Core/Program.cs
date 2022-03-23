using CorgEng.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Core
{

    internal class ExampleRenderCore : RenderCore
    {
        public override void PerformRender()
        {
            //Do nothing for now
            return;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            CorgEng.Initialize();
            ExampleRenderCore erc = new ExampleRenderCore();
            CorgEng.SetRenderCore(erc);
            CorgEng.TransferToRenderingThread();
            CorgEng.Shutdown();
        }
    }
}
