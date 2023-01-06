using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoMap.Actions
{
    internal class EditorActions
    {

        [UserInterfaceCodeCallback("Openmap")]
        public static void OpenMap(IUserInterfaceComponent userInterfaceComponent)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Dmm Files (*.dmm)|*.dmm";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
        }

    }
}
