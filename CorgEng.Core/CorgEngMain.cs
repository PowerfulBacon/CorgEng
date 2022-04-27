﻿using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.Core.Rendering;
using CorgEng.Core.Rendering.Exceptions;
using CorgEng.GenericInterfaces.InputHandler;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering;
using GLFW;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CorgEng.Core
{
    public static class CorgEngMain
    {

        /// <summary>
        /// The internal render master.
        /// Handles transfering iamges generated by the RenderCore to 
        /// the screen.
        /// </summary>
        private static RenderMaster InternalRenderMaster { get; set; }

        /// <summary>
        /// The main render core currently in use by the renderer
        /// </summary>
        public static RenderCore MainRenderCore { get; private set; }

        /// <summary>
        /// The window associated with the CorgEng application
        /// </summary>
        private static CorgEngWindow GameWindow { get; set; }

        /// <summary>
        /// The main camera for the game
        /// </summary>
        public static ICamera MainCamera { get; private set; }

        /// <summary>
        /// Create a logger
        /// </summary>
        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// Time of the last frame
        /// </summary>
        private static double lastFrameTime;

        /// <summary>
        /// The time delta between the last frame and the current frame.
        /// Time taken for rendering to occur.
        /// Note: Due to multi-threading DeltaTime is not the same across all processing threads.
        /// </summary>
        public static double DeltaTime { get; private set; }

        public static bool Terminated { get; private set; }

        /// <summary>
        /// Initializes the CorgEng game engine.
        /// Will call initialization on all CorgEng modules.
        /// </summary>
        public static void Initialize()
        {
            //Load priority modules (Logging)
            PriorityModuleInit();
            Logger?.WriteLine("Starting CorgEng Application", LogType.DEBUG);
            //Create a new window
            GameWindow = new CorgEngWindow();
            GameWindow.Open();
            Logger?.WriteLine("Successfully created primary window", LogType.DEBUG);
            //Create the internal render master
            InternalRenderMaster = new RenderMaster();
            InternalRenderMaster.Initialize();
            Logger?.WriteLine("Successfully initialized render master", LogType.DEBUG);
            //Load non-priority modules
            ModuleInit();
        }

        /// <summary>
        /// Transfers control of the main thread to the internal
        /// CorgEng rendering pipeline.
        /// </summary>
        public static void TransferToRenderingThread()
        {
            //While the window shouldn't close
            while (!GameWindow.ShouldClose())
            {
                lastFrameTime = Glfw.Time;
                //Swap the framebuffers
                GameWindow.SwapFramebuffers();
                //Poll for system events to prevent the program from showing as hanging
                Glfw.PollEvents();
                //Handle key events
                GameWindow.Update();
                //Check to ensure we have a render core
                if (MainRenderCore == null)
                    throw new NullRenderCoreException("The main CorgEng render core is not set! Use CorgEng.SetRenderCore(RenderCore) to set the primary render core.");
                //Process the main render core
                MainRenderCore.PreRender();
                MainRenderCore.PerformRender();
                //Pass the output image from the render core to the internal renderer
                InternalRenderMaster.RenderImageToScreen(MainRenderCore.RenderTextureUint);
                DeltaTime = Glfw.Time - lastFrameTime;
            }
        }

        public static void SetMainCamera(ICamera camera)
        {
            MainCamera = camera;
        }

        /// <summary>
        /// Sets the CorgEng program's render core.
        /// </summary>
        public static void SetRenderCore(RenderCore newRenderCore)
        {
            MainRenderCore = newRenderCore;
            MainRenderCore.Initialize();
        }

        /// <summary>
        /// Shuts down and cleans up all resources used by CorgEng
        /// at the end of the program.
        /// </summary>
        public static void Shutdown()
        {
            //Terinate the engine's subthreads.
            Terminated = true;
            //Call terination
            TriggerTerminateMethods();
            //Terminate GLFW
            Glfw.Terminate();
        }

        /// <summary>
        /// An enumerably containing assemblies loaded from the CorgEngConfig.
        /// </summary>
        private static IEnumerable<Assembly> LoadedAssemblyModules;

        /// <summary>
        /// Loads a CorgEng config file
        /// TODO: Whitelist/blacklist types and add sandboxing
        /// </summary>
        /// <param name="filePath"></param>
        public static void LoadConfig(string filePath)
        {
            try
            {
                //Locate the config resources file
                string resourceName = Assembly.GetEntryAssembly().GetManifestResourceNames().Single(str => str.EndsWith(filePath));
                using (Stream resourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName))
                {
                    XElement configElement = XElement.Load(resourceStream);
                    foreach (XElement childElement in configElement.Elements())
                    {
                        switch (childElement.Name.ToString())
                        {
                            case "DependencyModules":
                                List<Assembly> loadedAssemblies = new List<Assembly>();
                                foreach (XElement dependency in childElement.Elements())
                                {
                                    try
                                    {
                                        Assembly loadedModule = Assembly.LoadFile($"{Path.GetFullPath(dependency.Value)}.dll");
                                        loadedAssemblies.Add(loadedModule);
                                    }
                                    catch (FileNotFoundException)
                                    {
                                        Console.Error.WriteLine($"[CorgEng Config Error]: Couldn't located module {dependency.Value}.dll");
                                    }
                                }
                                LoadedAssemblyModules = loadedAssemblies;
                                break;
                            default:
                                Console.Error.WriteLine($"[CorgEng Config Error]: Error parsing config, unknown config attribute {childElement.Name}.");
                                break;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine("CorgEng: A fatal exception has occured during configuration loading!");
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine("The program may be corrupted or incorrectly configured.");
                Console.Error.WriteLine("Please reinstall the program and ensure that if developing a CorgEng game, the config is set as an embedded resource.");
                Console.Error.WriteLine("The application will now be terminated.");
                Console.Error.WriteLine("Press any key to continue...");
                Console.ReadKey();
                throw;
            }
            //Console
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"
 _____                 _____            
/  __ \               |  ___|           
| /  \/ ___  _ __ __ _| |__ _ __   __ _ 
| |    / _ \| '__/ _` |  __| '_ \ / _` |
| \__/| (_) | | | (_| | |__| | | | (_| |
 \____/\___/|_|  \__, \____|_| |_|\__, |
                  __/ |            __/ |
                 |___/            |___/ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("@ 2022 CorgEng | MIT License\n");
            Console.WriteLine("Succesfully loaded and parsed config!");
        }

        private static IEnumerable<MethodInfo> ModuleLoadAttributes;

        /// <summary>
        /// Calls priority method modules
        /// </summary>
        private static void PriorityModuleInit()
        {
            ModuleLoadAttributes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<ModuleLoadAttribute>() != null )));
            Parallel.ForEach(ModuleLoadAttributes, (MethodInfo) => {
                Console.WriteLine(MethodInfo.Name);
                if(MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().priority
                    && !MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().mainThread)
                    MethodInfo.Invoke(null, new object[] { });
            });
            foreach (MethodInfo methodToInvoke in ModuleLoadAttributes)
            {
                if (methodToInvoke.GetCustomAttribute<ModuleLoadAttribute>().priority
                    && methodToInvoke.GetCustomAttribute<ModuleLoadAttribute>().mainThread)
                    methodToInvoke.Invoke(null, new object[] { });
            }
        }

        /// <summary>
        /// Loads non-priority modules
        /// </summary>
        private static void ModuleInit()
        {
            Parallel.ForEach(ModuleLoadAttributes, (MethodInfo) => {
                if (!MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().priority
                    && !MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().mainThread)
                {
                    MethodInfo.Invoke(null, new object[] { });
                    Logger?.WriteLine($"Successfully loaded module {MethodInfo.DeclaringType.Name}");
                }
            });
            foreach (MethodInfo methodToInvoke in ModuleLoadAttributes)
            {
                if (!methodToInvoke.GetCustomAttribute<ModuleLoadAttribute>().priority
                    && methodToInvoke.GetCustomAttribute<ModuleLoadAttribute>().mainThread)
                    methodToInvoke.Invoke(null, new object[] { });
            }
            ModuleLoadAttributes = null;
        }

        /// <summary>
        /// Triggers termination methods
        /// </summary>
        private static void TriggerTerminateMethods()
        {
            IEnumerable<MethodInfo> TerminateAttributes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<ModuleTerminateAttribute>() != null)));
            foreach (MethodInfo methodToInvoke in TerminateAttributes)
            {
                methodToInvoke.Invoke(null, new object[] { });
            }
        }

    }
}
