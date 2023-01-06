using AutoMap.Parser;
using AutoMap.Rendering;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Generators;
using CorgEng.GenericInterfaces.UserInterface.Popups;
using CorgEng.UserInterface.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoMap.Actions
{
    internal class MenuActions
    {

        [UsingDependency]
        private static ILogger Logger = null!;

        [UsingDependency]
        private static IPopupManager PopupManager = null!;

        [UsingDependency]
        private static IUserInterfaceXmlLoader UserInterfaceXmlLoader = null!;

        [UsingDependency]
        private static IAnchorFactory AnchorFactory = null!;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory = null!;

        [UserInterfaceCodeCallback("LoadEnvironment")]
        public static void LoadEnvironment(IUserInterfaceComponent component)
        {
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            IUserInterfaceComponent loadingBar = UserInterfaceXmlLoader.LoadUserInterface("Content/UserInterface/AutoMapLoadingBar.xml");
            IPopupWindow loadingPopup = PopupManager.DisplayPopup(loadingBar, 1920 /2  - 300, 1080 / 2 - 120, 600, 240);
            bool threadCompleted = true;
            Task.Run(() => {
                try
                {
                    //Start parsing folders
                    string[] files = Directory.GetFiles(openFileDialog.SelectedPath, "*.dm", SearchOption.AllDirectories);
                    Logger.WriteLine($"Located {files.Length} files to parse...", LogType.MESSAGE);
                    int i = 0;
                    ParsedEnvironment createdEnvironment = new ParsedEnvironment();
                    foreach (string file in files)
                    {
                        DmParser.ParseFile(createdEnvironment, file);
                        //Update
                        if (threadCompleted)
                        {
                            threadCompleted = false;
                            CorgEngMain.ExecuteOnRenderingThread(() =>
                            {
                                double completedAmount = ((double)i / files.Length) * 100;
                                loadingBar.Children[1].Children[0].Anchor = AnchorFactory.CreateAnchor(
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PERCENTAGE, 0),
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PERCENTAGE, completedAmount),
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PERCENTAGE, 0),
                                    AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.BOTTOM, AnchorUnits.PERCENTAGE, 0)
                                    );
                                threadCompleted = true;
                            });
                        }
                        i++;
                    }
                    //Environment successfully parsed, go to the main screen
                    CorgEngMain.ExecuteOnRenderingThread(() =>
                    {
                        AutoMapRenderCore.Singleton.OpenEnvironment(createdEnvironment);
                    });
                }
                finally
                {
                    CorgEngMain.ExecuteOnRenderingThread(() =>
                    {
                        loadingPopup?.ClosePopup();
                    });
                }
            });
        }

        [UserInterfaceCodeCallback("Quit")]
        public static void QuitTool(IUserInterfaceComponent component)
        {
            CorgEngMain.GameWindow.Close();
        }

    }
}
