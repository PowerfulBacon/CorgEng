using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Popups
{
    internal class PopupWindow : IPopupWindow
    {

        [UsingDependency]
        private static IAnchorFactory AnchorFactory = null!;

        [UsingDependency]
        private static IAnchorDetailFactory AnchorDetailFactory = null!;

        /// <summary>
        /// The user interface component that we are manipulating.
        /// Should be attached to the popup manager and shouldn't be used
        /// more than once.
        /// </summary>
        public IUserInterfaceComponent AttachedComponent { get; }

        public double X
        {
            get => throw new NotImplementedException();
            set => AttachedComponent.Anchor = AnchorFactory.CreateAnchor(
                AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, value, true),
                AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, value + Width, true),
                AttachedComponent.Anchor.TopDetails,
                AttachedComponent.Anchor.BottomDetails
                );
        }

        public double Y
        {
            get => throw new NotImplementedException();
            set => AttachedComponent.Anchor = AnchorFactory.CreateAnchor(
                AttachedComponent.Anchor.LeftDetails,
                AttachedComponent.Anchor.RightDetails,
                AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, value, true),
                AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, value + Height, true)
                );
        }

        public double Width
        {
            get => AttachedComponent.PixelWidth;
            set => AttachedComponent.Anchor = AnchorFactory.CreateAnchor(
                AttachedComponent.Anchor.LeftDetails,
                AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.LEFT, AnchorUnits.PIXELS, AttachedComponent.Anchor.LeftDetails.AnchorOffset + value, true),
                AttachedComponent.Anchor.TopDetails,
                AttachedComponent.Anchor.BottomDetails
                );
        }

        public double Height
        {
            get => AttachedComponent.PixelHeight;
            set => AttachedComponent.Anchor = AnchorFactory.CreateAnchor(
                AttachedComponent.Anchor.LeftDetails,
                AttachedComponent.Anchor.RightDetails,
                AttachedComponent.Anchor.TopDetails,
                AnchorDetailFactory.CreateAnchorDetails(AnchorDirections.TOP, AnchorUnits.PIXELS, AttachedComponent.Anchor.TopDetails.AnchorOffset + value, true)
                );
        }
        public PopupWindow(IUserInterfaceComponent containedComponent)
        {
            //Set the contained component so we can update it later on.
            AttachedComponent = containedComponent;
        }

        public void ClosePopup()
        {
            //Remove this
            AttachedComponent.Parent.RemoveChild(AttachedComponent);
        }
    }
}
