using CorgEng.Core;
using System;
using System.Windows.Forms;

namespace GJ2022.DmiIconConversionUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Execute CorgEng Start
            //Load the application config
            CorgEngMain.LoadConfig("CorgEngConfig.xml");
            //Initialize CorgEng in renderless mode
            //This creates the window and loads all
            //modules that are dependencies
            CorgEngMain.Initialize(true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
