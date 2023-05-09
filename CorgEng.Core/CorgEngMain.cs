﻿//#define PERFORMANCE

using CorgEng.Core.Dependencies;
using CorgEng.Core.Modules;
using CorgEng.Core.Rendering;
using CorgEng.Core.Rendering.Exceptions;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.Rendering;
using GLFW;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
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
        public static CorgEngWindow GameWindow { get; set; }

        /// <summary>
        /// The main camera for the game
        /// </summary>
        public static ICamera MainCamera { get; private set; }

        /// <summary>
        /// The name of the window
        /// </summary>
        public static string WindowName { get; set; } = "CorgEngApplication";

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
        /// Get the current application time in seconds
        /// </summary>
        public static double Time => (IsRendering ? Glfw.Time : (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

        /// <summary>
        /// The time delta between the last frame and the current frame.
        /// Time taken for rendering to occur.
        /// Milliseconds.
        /// Note: Due to multi-threading DeltaTime is not the same across all processing threads.
        /// </summary>
        public static double DeltaTime { get; private set; }

        public static bool Terminated { get; private set; }

        public static bool IsRendering { get; private set; } = false;

        public static event Action OnReadyEvents = null;

        private static IWorld primaryWorld = null;

        /// <summary>
        /// The main world to use for the game for when one isn't accessible
        /// </summary>
        public static IWorld PrimaryWorld
        {
            get => primaryWorld;
            set {
                primaryWorld = value;
                WorldInit(primaryWorld);
            }
        }
        /// <summary>
        /// All current active worlds
        /// </summary>
        public static ConcurrentBag<IWorld> WorldList { get; } = new ConcurrentBag<IWorld>();

        /// <summary>
        /// List of actions queued to be execuetd on the main thread
        /// </summary>
        private static ConcurrentQueue<Action> queuedActions = new ConcurrentQueue<Action>();

        /// <summary>
        /// Initializes the CorgEng game engine.
        /// Will call initialization on all CorgEng modules.
        /// </summary>
        public static void Initialize(bool disableRendering = false)
        {
            try
            {
                // Reset the world list
                WorldList.Clear();
                //Load priority modules (Logging)
                PriorityModuleInit();
                Logger?.WriteLine("Starting CorgEng Application", LogType.DEBUG);
                if (disableRendering)
                {
                    //Load non priority modules
                    ModuleInit();
                    return;
                }
                //Enable rendering functionality
                IsRendering = true;
                //Create a new window
                GameWindow = new CorgEngWindow();
                GameWindow.Open();
                Logger?.WriteLine("Successfully created primary window", LogType.DEBUG);
                //Create the internal render master
                InternalRenderMaster = new RenderMaster();
                InternalRenderMaster.Initialize();
                Logger?.WriteLine("Successfully initialized render master", LogType.DEBUG);
                //Bind the render master size to the game window size
                GameWindow.OnWindowResized += InternalRenderMaster.SetWindowRenderSize;
                //Load non-priority modules
                ModuleInit();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("A fatal exception occured during module load. Execution can no longer continue.");
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Called when everything is full intialised
        /// </summary>
        private static void Ready()
        {
            OnReadyEvents?.Invoke();
            OnReadyEvents = null;
        }

        /// <summary>
        /// Transfers control of the main thread to the internal
        /// CorgEng rendering pipeline.
        /// </summary>
        public static void TransferToRenderingThread()
        {
#if PERFORMANCE
            int i = 0;
            double total = 0;
#endif
            //Call the ready callbacks
            Ready();
            //Use the FPS cap
            Glfw.SwapInterval(1);
            //While the window shouldn't close
            while (!GameWindow.ShouldClose())
            {
                lastFrameTime = Glfw.Time;
                //Trigger any render thread code
                if (!queuedActions.IsEmpty)
                {
                    Action action;
                    while (queuedActions.TryDequeue(out action))
                    {
                        try
                        {
                            action.Invoke();
                        }
                        catch (System.Exception e)
                        {
                            Logger.WriteLine($"Exception invoked from deferred action: {e}", LogType.ERROR);
                        }
                    }
                }
                // Trigger any deferred rendering thread code
                CheckQueuedExecutions();
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
                MainRenderCore.DoRender();
                //Pass the output image from the render core to the internal renderer
                InternalRenderMaster.RenderImageToScreen(MainRenderCore);
                DeltaTime = Glfw.Time - lastFrameTime;
#if PERFORMANCE
                total += DeltaTime;
                i++;
                if (i >= 200)
                {
                    total /= i;
                    Logger.WriteLine($"Average delta time: {total} seconds Avg FPS: ({1/total})", LogType.TEMP);
                    i = 0;
                    total = 0;
                }
#endif
            }
        }

        public static void SetMainCamera(ICamera camera)
        {
            MainCamera = camera;
        }

        private static CorgEngWindow.WindowResizeDelegate activeSizeDelegate;

        /// <summary>
        /// Sets the CorgEng program's render core.
        /// </summary>
        public static void SetRenderCore(RenderCore newRenderCore)
        {
            MainRenderCore = newRenderCore;
            MainRenderCore.Initialize();
            GameWindow.OnWindowResized -= activeSizeDelegate;
            activeSizeDelegate = MainRenderCore.Resize;
            GameWindow.OnWindowResized += activeSizeDelegate;
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
            // Reset the world bag
            WorldList.Clear();
        }

        /// <summary>
        /// An enumerably containing assemblies loaded from the CorgEngConfig.
        /// </summary>
        public static IEnumerable<Assembly> LoadedAssemblyModules;

        /// <summary>
        /// Loads a CorgEng config file
        /// TODO: Whitelist/blacklist types and add sandboxing
        /// </summary>
        /// <param name="filePath"></param>
        public static void LoadConfig(string filePath, bool embeddedResource = true, bool awaitOnError = true)
        {
            try
            {
                string resourceName;
                Stream resourceStream;
                //Locate the config resources file
                if (embeddedResource)
                {
                    resourceName = Assembly.GetEntryAssembly().GetManifestResourceNames().Single(str => str.EndsWith(filePath));
                    resourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(resourceName);
                }
                else
                {
                    resourceName = filePath;
                    resourceStream = new FileStream(filePath, FileMode.Open);
                }
                XElement configElement = XElement.Load(resourceStream);
                foreach (XElement childElement in configElement.Elements())
                {
                    switch (childElement.Name.ToString())
                    {
                        case "DependencyModules":
                            List<Assembly> loadedAssemblies = new List<Assembly>();
                            //Add the system assembly
                            loadedAssemblies.Add(typeof(string).Assembly);
                            //Add the entry assembly
                            if (Assembly.GetEntryAssembly() != null)
                                loadedAssemblies.Add(Assembly.GetEntryAssembly());
                            //Unit test support.
                            if (Assembly.GetCallingAssembly() != null && Assembly.GetCallingAssembly() != Assembly.GetEntryAssembly())
                                loadedAssemblies.Add(Assembly.GetCallingAssembly());
                            if (Assembly.GetExecutingAssembly() != null)
                                loadedAssemblies.Add(Assembly.GetExecutingAssembly());
                            foreach (XElement dependency in childElement.Elements())
                            {
                                try
                                {
                                    Assembly loadedModule = AssemblyLoadContext.Default.LoadFromAssemblyPath($"{Path.GetFullPath(dependency.Value)}.dll");
                                    //Assembly loadedModule = Assembly.LoadFile($"{Path.GetFullPath(dependency.Value)}.dll");
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
                resourceStream.Close();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine("CorgEng: A fatal exception has occured during configuration loading!");
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                Console.Error.WriteLine("The program may be corrupted or incorrectly configured.");
                Console.Error.WriteLine("Please reinstall the program and ensure that if developing a CorgEng game, the config is set as an embedded resource.");
                Console.Error.WriteLine("The application will now be terminated.");
                Console.Error.WriteLine("Press any key to continue...");
                if (awaitOnError)
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
            ModuleLoadAttributes = LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<ModuleLoadAttribute>() != null)));
            Parallel.ForEach(ModuleLoadAttributes, (MethodInfo) =>
            {
                Console.WriteLine(MethodInfo.Name);
                if (MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().priority
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
            Parallel.ForEach(ModuleLoadAttributes, (MethodInfo) =>
            {
                if (!MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().priority
                    && !MethodInfo.GetCustomAttribute<ModuleLoadAttribute>().mainThread)
                {
                    MethodInfo.Invoke(null, new object[] { });
                    Logger?.WriteLine($"Successfully loaded module {MethodInfo.DeclaringType.Name}", LogType.LOG);
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
        /// Calls priority method modules
        /// </summary>
        private static void WorldInit(IWorld world)
        {
            IEnumerable<MethodInfo> worldMethods = LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<WorldInitialiseAttribute>() != null)));
            foreach (MethodInfo methodToInvoke in worldMethods)
            {
                methodToInvoke.Invoke(null, new object[] { world });
            }
        }

        /// <summary>
        /// Triggers termination methods
        /// </summary>
        private static void TriggerTerminateMethods()
        {
            IEnumerable<MethodInfo> TerminateAttributes = LoadedAssemblyModules
                .SelectMany(assembly => assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttribute<ModuleTerminateAttribute>() != null)));
            foreach (MethodInfo methodToInvoke in TerminateAttributes)
            {
                methodToInvoke.Invoke(null, new object[] { });
            }
        }

        /// <summary>
        /// Queue something to be executed on the rendering thread
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteOnRenderingThread(Action action)
        {
            if (IsRendering)
            {
                //Rendering thread exists, queue the action
                queuedActions.Enqueue(action);
            }
            else
            {
                //Headless mode, immedaitely execute
                action.Invoke();
            }
        }

        /// <summary>
        /// The lowest time of the thing that we want to fire
        /// </summary>
        private static double nextBucketFireTime = double.PositiveInfinity;

        /// <summary>
        /// A heap of the things that we want to execute on this thread and the
        /// time of when we want to execute them.
        /// </summary>
        private static PriorityQueue<Action, double> executeInQueue = new PriorityQueue<Action, double>();

        private static Thread headlessExecutionThread = null;

        /// <summary>
        /// Add something to a bucket
        /// </summary>
        /// <param name="action"></param>
        /// <param name="executeTime">The time to wait in milliseconds</param>
        public static void ExecuteIn(Action action, double executeTime)
        {
            if (!IsRendering && headlessExecutionThread == null)
            {
                // Lock something random so that we can only enter this once
                lock (executeInQueue)
                {
                    if (headlessExecutionThread == null)
                    {
                        // This is horrible
                        headlessExecutionThread = new Thread(() => {
                            while (!Terminated)
                            {
                                CheckQueuedExecutions();
                                Thread.Yield();
                            }
                            lock (executeInQueue)
                            {
                                headlessExecutionThread = null;
                            }
                        });
                        headlessExecutionThread.Start();
                    }
                }
            }
            double timeToFire = Time + executeTime * 0.001;
            Logger.WriteLine($"Action queued to fire at {timeToFire}", LogType.WARNING);
            // 0 or negative execution time
            if (timeToFire <= Time)
            {
                //Rendering thread exists, queue the action
                queuedActions.Enqueue(action);
                return;
            }
            // Queue the action to be fired
            lock (executeInQueue)
            {
                nextBucketFireTime = Math.Min(nextBucketFireTime, timeToFire);
                executeInQueue.Enqueue(action, timeToFire);
            }
        }

        /// <summary>
        /// Check the queue of things that we want to fire on this thread and fire them if
        /// we are ready for them.
        /// </summary>
        internal static void CheckQueuedExecutions()
        {
            if (nextBucketFireTime > Time)
                return;
            lock (executeInQueue)
            {
                while (nextBucketFireTime <= Time)
                {
                    // Invoke the action
                    Action lowest = executeInQueue.Dequeue();
                    //Logger.WriteLine($"Action invoked at {Time}. NEXT BUCKET TIME: {nextBucketFireTime}", LogType.WARNING);
                    lowest.Invoke();
                    // Move to the next
                    if (executeInQueue.TryPeek(out Action _, out double nextFireTime))
                    {
                        nextBucketFireTime = nextFireTime;
                    }
                    else
                    {
                        nextBucketFireTime = double.PositiveInfinity;
                        return;
                    }
                }
            }
        }

        /**
         * Fully cleanup the CorgEng application, resetting it into a state
         * of startup.
         */
        public static void Cleanup()
        {
            MainCamera = null;
            primaryWorld = null;
            foreach (IWorld world in WorldList)
            {
                world.Cleanup();
            }
            WorldList.Clear();
            queuedActions.Clear();
            MainRenderCore = null;
            Logger.WriteLine("Full cleanup of CorgEng application completed.", LogType.DEBUG);
        }

    }
}
