using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceComponent : IUserInterfaceComponent
    {

        [UsingDependency]
        private static ILogger Logger;

        public IAnchor Anchor { get; }

        public IUserInterfaceComponent Parent { get; }

        public double PixelWidth { get; private set; }

        public double PixelHeight { get; private set; }

        public double MinimumPixelWidth { get; private set; }

        public double MinimumPixelHeight { get; private set; }

        private List<UserInterfaceComponent> Children { get; } = new List<UserInterfaceComponent>();

        //TODO: Create this constructor
        public UserInterfaceComponent()
        {
            // Give warnings if the anchor units are mismatched.
            // Mismatched anchor units could result in the right position being
            // further left than the left position.
            if (Anchor.LeftDetails.AnchorUnits != Anchor.RightDetails.AnchorUnits)
                Logger?.WriteLine($"User interface component's left anchor's units ({Anchor.LeftDetails.AnchorUnits}) " +
                    $"does not match the component's right anchor's units ({Anchor.RightDetails.AnchorUnits}). " +
                    $"This may result in unexpected behaviour in regards to scaling UI components.", LogType.WARNING);
            if (Anchor.TopDetails.AnchorUnits != Anchor.BottomDetails.AnchorUnits)
                Logger?.WriteLine($"User interface component's top anchor's units ({Anchor.TopDetails.AnchorUnits}) " +
                    $"does not match the component's bottom anchor's units ({Anchor.BottomDetails.AnchorUnits}). " +
                    $"This may result in unexpected behaviour in regards to scaling UI components.", LogType.WARNING);
            //Check for other maximum size limits, maximum size isn't supported.
            if (Anchor.LeftDetails.AnchorSide == AnchorDirections.RIGHT && Anchor.RightDetails.AnchorSide == AnchorDirections.LEFT)
                Logger?.WriteLine($"User interface component's left anchor is right, and the right anchor is left. This will result in a maximum size which isn't supported.", LogType.WARNING);
            if (Anchor.BottomDetails.AnchorSide == AnchorDirections.TOP && Anchor.TopDetails.AnchorSide == AnchorDirections.BOTTOM)
                Logger?.WriteLine($"User interface component's bottom anchor is top, and the top anchor is bottom. This will result in a maximum size which isn't supported.", LogType.WARNING);
        }

        public void Render()
        {
            throw new NotImplementedException();
        }

        public void AddChild(IUserInterfaceComponent userInterfaceComponent)
        {
            throw new NotImplementedException();
        }

        public List<IUserInterfaceComponent> GetChildren()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parent component was resized.
        /// Recalculate our scale if required.
        /// </summary>
        public void OnParentResized()
        {
            //Calculate our new pixel scale
            
        }

        private void UpdatePixelScale(long pixelWidth, long pixelHeight)
        {

        }

        private void CalculateMinimumScales()
        {
            //Calculate our basic minimum scale
            MinimumPixelWidth = 0;
            MinimumPixelHeight = 0;
            //Calculate the minimum widths of children recursively.
            foreach (UserInterfaceComponent childComponent in Children)
            {
                //Calclate the minimum scale
                childComponent.CalculateMinimumScales();
                //Determine our minimum scales based on this child component (Account for child min width + child offset)
            }
            //Calculate our minimum width alone
            if (Anchor.LeftDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.RightDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.LeftDetails.AnchorSide == Anchor.RightDetails.AnchorSide)
            {
                MinimumPixelWidth = Math.Min(MinimumPixelWidth, Math.Abs(Anchor.RightDetails.AnchorOffset - Anchor.LeftDetails.AnchorOffset));
            }
            //Calculate our minimum height alone
            if (Anchor.TopDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.BottomDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.TopDetails.AnchorSide == Anchor.BottomDetails.AnchorSide)
            {
                MinimumPixelHeight = Math.Min(MinimumPixelHeight, Math.Abs(Anchor.BottomDetails.AnchorOffset - Anchor.TopDetails.AnchorOffset));
            }
        }

    }
}
