using GLFW;
using System;
using static OpenGL.Gl;

namespace CorgEng.Core.Rendering
{
    internal class CorgEngWindow
    {

        private Window glWindowInstance;

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
            glWindowInstance = Glfw.CreateWindow(Math.Min(width, 1920), Math.Min(height, 1080), "CorgEngApplication", Monitor.None, Window.None);
            //Set the current context to the window
            Glfw.MakeContextCurrent(glWindowInstance);
            //I don't actually know what this does
            Import(Glfw.GetProcAddress);
        }

    }
}
