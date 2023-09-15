using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.UserInterfaceComponents
{
    /// <summary>
    /// Will hide everything in the children of this element until it is pressed.
    /// The first element will be used as the expansion button and is always visible.
    /// This is really jankilly coded!
    /// </summary>
    internal class UserInterfaceDropdown : UserInterfaceBox
    {

        /// <summary>
        /// If not expanded, the children will not be considered in the contents.
        /// </summary>
        public bool IsExpanded { get; set; } = true;

        /// <summary>
        /// Don't include this component in screencasts.
        /// </summary>
        protected override bool ScreencastInclude => false;

        protected override bool RenderInclude => false;

        private List<IUserInterfaceComponent> expandButton = new List<IUserInterfaceComponent>();

        public UserInterfaceDropdown(IWorld world, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world, anchorDetails, arguments)
        {
            ToggleExpansion();
        }

        public UserInterfaceDropdown(IWorld world, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world, parent, anchorDetails, arguments)
        {
            ToggleExpansion();
        }

        private void AddClickAction()
        {
            //Add click action to the first child.
            IUserInterfaceComponent firstChild = Children.FirstOrDefault();
            if (firstChild != null)
            {
                firstChild.ComponentHolder.AddComponent(new UserInterfaceClickActionComponent(ToggleExpansion));
                expandButton.Add(firstChild);
            }
        }

        public override void AddChild(IUserInterfaceComponent userInterfaceComponent)
        {
            base.AddChild(userInterfaceComponent);
            if (Children.Count == 1)
            {
                AddClickAction();
            }
        }

        private void ToggleExpansion()
        {
            IsExpanded = !IsExpanded;
        }

        public override List<IUserInterfaceComponent> GetChildren()
        {
            if (!IsExpanded)
            {
                return expandButton;
            }
            return base.GetChildren();
        }

        /// <summary>
        /// Don't include self in raycasting
        /// </summary>
        /// <param name="relativeX"></param>
        /// <param name="relativeY"></param>
        /// <returns></returns>
        public override IUserInterfaceComponent Screencast(int relativeX, int relativeY)
        {
            //If outside bounds, return nothing
            if (relativeX < 0 || relativeY < 0 || relativeX > PixelWidth || relativeY > PixelHeight)
            {
                return null;
            }
            //Screencast children first (Children are above us)
            foreach (IUserInterfaceComponent childComponent in GetChildren())
            {
                //Determine new relative positions
                int newRelativeX = relativeX - (int)childComponent.LeftOffset;
                int newRelativeY = relativeY - (int)childComponent.BottomOffset;
                IUserInterfaceComponent selected = childComponent.Screencast(newRelativeX, newRelativeY);
                if (selected != null)
                    return selected;
            }
            //Nothing was hit
            return null;
        }

    }
}
