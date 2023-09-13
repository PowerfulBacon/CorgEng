using CorgEng.Core;
using CorgEng.DependencyInjection.Injection;
using CorgEng.GenericInterfaces.Rendering.Textures;
using CorgEng.GenericInterfaces.UserInterface.Rendering;
using CorgEng.Networking.Components;
using CorgEng.Networking.EntitySystems;
using CorgEng.Networking.VersionSync;
using CorgEng.Tests.Stubs;
using CorgEng.UserInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Tests
{
    [TestClass]
    public class TestSetup : TestBase
    {

        [AssemblyInitialize]
        public static void TestInitialize(TestContext testContext)
        {
            //Load config
            CorgEngMain.LoadConfig("CorgEngConfig.xml", false, false);
            //Inject dependencies
            DependencyInjector.LoadDependencyInjection();
            //Replace any openGL dependant dependencies
            //DependencyInjector.OverrideDependency<IUserInterfaceRenderCoreFactory>(new IRenderCoreStubFactory());
            //DependencyInjector.OverrideDependency<ITextureFactory>(new TextureFactoryStub());
            //Initialize networked IDs
            VersionGenerator.CreateNetworkedIDs();
            //Load any module loads that we HAVE to load
            ComponentExtensions.LoadPropertyInfoCache();
            UserInterfaceModule.OnModuleLoad();
        }

    }
}
