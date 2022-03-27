using CorgEng.Core;
using CorgEng.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example
{
    class Program
    {
        internal class ExampleRenderCore : RenderCore
        {
            public override void Initialize()
            {
                return;
            }

            public override void PerformRender()
            {
                //Do nothing for now
                return;
            }
        }

        static void Main(string[] args)
        {
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize();
            //Set the render core
            ExampleRenderCore erc = new ExampleRenderCore();
            CorgEngMain.SetRenderCore(erc);
            //Transfer control of the main thread to the CorgEng
            //rendering thread
            CorgEngMain.TransferToRenderingThread();
            //Shut down the program once it has been closed
            //and clean everything up.
            CorgEngMain.Shutdown();
        }
    }
}
