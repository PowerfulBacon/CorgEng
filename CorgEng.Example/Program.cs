using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.DependencyInjection;
using CorgEng.DependencyInjection.Injection;
using CorgEng.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering;
using CorgEng.GenericInterfaces.Rendering.Cameras.Isometric;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.Logging;
using CorgEng.UtilityTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Example
{
    class Program
    {
        internal class ExampleRenderCore : RenderCore
        {

            [UsingDependency]
            private static ISpriteRenderer spriteRenderer;

            [UsingDependency]
            private static ISpriteRenderObjectFactory spriteRenderObjectFactory;

            [UsingDependency]
            private static ITextureFactory textureFactory;

            public override void Initialize()
            {
                spriteRenderer?.Initialize();
                //Create a textured sprite object
                ITexture texture = textureFactory.CreateTexture("Content/Textures/Example/Example.bmp");
                //Here is an example of just using a basic texture:
                //We load the texture and don't use the json types
                ISpriteRenderObject spriteRenderObject = spriteRenderObjectFactory?.CreateSpriteRenderObject(texture.TextureID, 0, 0, 1, 1);
                spriteRenderer?.StartRendering(spriteRenderObject);
            }

            public override void PerformRender()
            {
                spriteRenderer?.Render(CorgEngMain.MainCamera);
            }
        }

        [UsingDependency]
        private static IIsometricCameraFactory isometricCameraFactory;

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize();
            //Set the main camera
            ICamera camera = isometricCameraFactory.CreateCamera();
            CorgEngMain.SetMainCamera(camera);
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
