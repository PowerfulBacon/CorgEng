using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Networking.Networking.Server;
using CorgEng.InputHandling.Events;
using GLFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorgEng.Example.Server
{
    class Program
    {

        [UsingDependency]
        private static ILogger Logger;

        [UsingDependency]
        private static INetworkingServer NetworkingServer;

        static void Main(string[] args)
        {
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng in headless mode
            CorgEngMain.Initialize(true);

            //Start networking server
            NetworkingServer.StartHosting(5000);

            while (true)
            {
                new KeyHeldEvent(Keys.W).RaiseGlobally();
                Thread.Sleep(100);
            }
        }
    }
}
