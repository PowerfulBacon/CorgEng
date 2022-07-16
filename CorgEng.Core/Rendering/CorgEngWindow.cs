using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.InputHandler;
using GLFW;
using System;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    public class CorgEngWindow
    {

        private Window glWindowInstance;

        [UsingDependency]
        private static IInputHandler InputHandler;

        internal delegate void WindowResizeDelegate(int width, int height);
        internal WindowResizeDelegate OnWindowResized;

        public int Width { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// Open the main window
        /// </summary>
        public void Open()
        {
            SetWindowHints();
            SetupWindow();
        }

        public void SwapFramebuffers()
        {
            Glfw.SwapBuffers(glWindowInstance);
        }

        /// <summary>
        /// Set the GLFW window hints.
        /// Use GL version 3.3.Core
        /// Allow the window to be resized.
        /// </summary>
        private void SetWindowHints()
        {
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Resizable, true);
        }

        public bool ShouldClose()
        {
            return Glfw.WindowShouldClose(glWindowInstance);
        }

        private SizeCallback sizeCallbackAliveKeeper;

        /// <summary>
        /// Setup the main window.
        /// Ensure the program fits on the monitor
        /// </summary>
        private void SetupWindow()
        {
            //Ensure the window we create fits on the screen
            int width = Glfw.PrimaryMonitor.WorkArea.Width;
            int height = Glfw.PrimaryMonitor.WorkArea.Height;
            //Create the window instance
<<<<<<< Updated upstream
            glWindowInstance = Glfw.CreateWindow(Math.Min(width, 1920), Math.Min(height, 1080), "CorgEngApplication", Monitor.None, Window.None);
=======
            Height = Math.Min(height, 1080);
            Width = Math.Min(width, 1920);
            glWindowInstance = Glfw.CreateWindow(Width, Height, CorgEngMain.WindowName, Monitor.None, Window.None);
>>>>>>> Stashed changes
            //Set the current context to the window
            Glfw.MakeContextCurrent(glWindowInstance);
            //I don't actually know what this does
            Import(Glfw.GetProcAddress);
            //Setup the input handler
            InputHandler?.SetupInputHandler(glWindowInstance);
            //Set the window size callback
            sizeCallbackAliveKeeper = WindowResizeHandler;
            Glfw.SetWindowSizeCallback(glWindowInstance, sizeCallbackAliveKeeper);
        }

        private void WindowResizeHandler(IntPtr window, int width, int height)
        {
            //Update the render core
            Width = width;
            Height = height;
            OnWindowResized?.Invoke(width, height);
        }

        public void Update()
        {
            InputHandler?.WindowUpdate(glWindowInstance);
        }

    }
}
