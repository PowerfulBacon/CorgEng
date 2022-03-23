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
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEng.Initialize();
            //Set the render core
            ExampleRenderCore erc = new ExampleRenderCore();
            CorgEng.SetRenderCore(erc);
            //Transfer control of the main thread to the CorgEng
            //rendering thread
            CorgEng.TransferToRenderingThread();
            //Shut down the program once it has been closed
            //and clean everything up.
            CorgEng.Shutdown();
        }
    }
}
