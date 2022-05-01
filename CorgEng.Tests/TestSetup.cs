using CorgEng.Core;
using CorgEng.DependencyInjection.Injection;
using CorgEng.GenericInterfaces.UserInterface.Rendering;
using CorgEng.Tests.Stubs;
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
    public class TestSetup
    {

        [AssemblyInitialize]
        public static void TestInitialize(TestContext testContext)
        {
            Console.WriteLine(Assembly.GetEntryAssembly() == null ? "NULL" : "GOOD");
            //Load config
            CorgEngMain.LoadConfig("CorgEngConfig.xml", false, false);
            //Inject dependencies
            DependencyInjector.LoadDependencyInjection();
            //Replace any openGL dependant dependencies
            DependencyInjector.OverrideDependency<IUserInterfaceRenderCoreFactory>(new IRenderCoreStubFactory());
        }

    }
}
