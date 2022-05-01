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

        private List<IUserInterfaceComponent> Children { get; } = new List<IUserInterfaceComponent>();

        public UserInterfaceComponent(IUserInterfaceComponent parent, IAnchor anchorDetails) : this(anchorDetails)
        {
            //Set the parent
            Parent = parent;
            //Set children
            Parent?.AddChild(this);
        }

        public UserInterfaceComponent(IAnchor anchorDetails)
        {
            // Set the anchor details
            Anchor = anchorDetails;
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
            //Calculate minimum scales
            CalculateMinimumScales();
        }

        public void Render()
        {
            throw new NotImplementedException();
        }

        public void AddChild(IUserInterfaceComponent userInterfaceComponent)
        {
            //Add the child
            Children.Add(userInterfaceComponent);
            //Recalculate minimum UI scale
            GetTopLevelParent(this).CalculateMinimumScales();
            //Recalculate child component's scale
            userInterfaceComponent.OnParentResized();
            //Check for expansion
            CheckExpansion(this);
        }

        /// <summary>
        /// Get the parent component
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private IUserInterfaceComponent GetTopLevelParent(IUserInterfaceComponent current)
        {
            if (current.Parent == null)
                return current;
            return GetTopLevelParent(current.Parent);
        }

        /// <summary>
        /// If we don't have a parent, we are a root and scale cannot change.
        /// If we aren't set to resize, maintain size based on parent component and don't contain subcomponent
        /// If we are set to resize, expand our size to fit our children
        /// </summary>
        public void CheckExpansion(IUserInterfaceComponent current)
        {
            //If we have no parent, we do not expand to fit our children (Don't change parent UI scales).
            if (current.Parent == null)
                return;
            //Check if we need to expand
            current.SetWidth(current.PixelWidth, current.PixelHeight);
            //Expand parent
            CheckExpansion(current.Parent);
        }

        public List<IUserInterfaceComponent> GetChildren()
        {
            return Children;
        }

        /// <summary>
        /// Parent component was resized.
        /// Recalculate our scale if required.
        /// </summary>
        public void OnParentResized()
        {
            //Calculate our new pixel scale
            double desiredWidth;
            double desiredHeight;
            //Calculate our left position relative to the left
            double leftSideOffset;
            if (Anchor.LeftDetails.AnchorUnits == AnchorUnits.PERCENTAGE)
            {
                if (Anchor.LeftDetails.AnchorSide == AnchorDirections.LEFT)
                    leftSideOffset = Anchor.LeftDetails.AnchorOffset * (Parent.PixelWidth / 100.0);
                else
                    leftSideOffset = (100.0 - Anchor.LeftDetails.AnchorOffset) * (Parent.PixelWidth / 100.0);
            }
            else
            {
                if (Anchor.LeftDetails.AnchorSide == AnchorDirections.LEFT)
                    leftSideOffset = Anchor.LeftDetails.AnchorOffset;
                else
                    leftSideOffset = Parent.PixelWidth - Anchor.LeftDetails.AnchorOffset;
            }
            //Calculate our right position relative to the right
            double rightSideOffset;
            if (Anchor.RightDetails.AnchorUnits == AnchorUnits.PERCENTAGE)
            {
                if (Anchor.RightDetails.AnchorSide == AnchorDirections.LEFT)
                    rightSideOffset = Anchor.RightDetails.AnchorOffset * (Parent.PixelWidth / 100.0);
                else
                    rightSideOffset = (100.0 - Anchor.RightDetails.AnchorOffset) * (Parent.PixelWidth / 100.0);
            }
            else
            {
                if (Anchor.RightDetails.AnchorSide == AnchorDirections.LEFT)
                    rightSideOffset = Anchor.RightDetails.AnchorOffset;
                else
                    rightSideOffset = Parent.PixelWidth - Anchor.RightDetails.AnchorOffset;
            }
            //Calculate our bottom position relative to the bottom
            double bottomSideOffset;
            if (Anchor.BottomDetails.AnchorUnits == AnchorUnits.PERCENTAGE)
            {
                if (Anchor.BottomDetails.AnchorSide == AnchorDirections.BOTTOM)
                    bottomSideOffset = Anchor.BottomDetails.AnchorOffset * (Parent.PixelHeight / 100.0);
                else
                    bottomSideOffset = (100.0 - Anchor.BottomDetails.AnchorOffset) * (Parent.PixelHeight / 100.0);
            }
            else
            {
                if (Anchor.BottomDetails.AnchorSide == AnchorDirections.BOTTOM)
                    bottomSideOffset = Anchor.BottomDetails.AnchorOffset;
                else
                    bottomSideOffset = Parent.PixelHeight - Anchor.BottomDetails.AnchorOffset;
            }
            //Calculate our right position relative to the right
            double topSideOffset;
            if (Anchor.TopDetails.AnchorUnits == AnchorUnits.PERCENTAGE)
            {
                if (Anchor.TopDetails.AnchorSide == AnchorDirections.BOTTOM)
                    topSideOffset = Anchor.TopDetails.AnchorOffset * (Parent.PixelHeight / 100.0);
                else
                    topSideOffset = (100.0 - Anchor.TopDetails.AnchorOffset) * (Parent.PixelHeight / 100.0);
            }
            else
            {
                if (Anchor.TopDetails.AnchorSide == AnchorDirections.BOTTOM)
                    topSideOffset = Anchor.TopDetails.AnchorOffset;
                else
                    topSideOffset = Parent.PixelHeight - Anchor.TopDetails.AnchorOffset;
            }
            //The desired width
            desiredWidth = rightSideOffset - leftSideOffset;
            desiredHeight = topSideOffset - bottomSideOffset;
            //Set our new scale
            SetWidth(desiredWidth, desiredHeight);
        }

        public void CalculateMinimumScales()
        {
            //Calculate our basic minimum scale
            MinimumPixelWidth = 0;
            MinimumPixelHeight = 0;
            //If we don't expand our minimum width inherits from children components
            //Otherwise it is soley based on our parent component
            //Calculate the minimum widths of children recursively.
            //Skip if all are strict
            if (!Anchor.LeftDetails.Strict || !Anchor.RightDetails.Strict || !Anchor.TopDetails.Strict || !Anchor.BottomDetails.Strict)
            {
                foreach (IUserInterfaceComponent childComponent in Children)
                {
                    //Calclate the minimum scale
                    childComponent.CalculateMinimumScales();
                    //Determine our minimum scales based on this child component (Account for child min width + child offset)
                    if (!Anchor.LeftDetails.Strict || !Anchor.RightDetails.Strict)
                    {
                        MinimumPixelWidth = Math.Max(
                            MinimumPixelWidth,
                            childComponent.MinimumPixelWidth
                            + (childComponent.Anchor.LeftDetails.AnchorSide == AnchorDirections.LEFT && childComponent.Anchor.LeftDetails.AnchorUnits == AnchorUnits.PIXELS
                                ? childComponent.Anchor.LeftDetails.AnchorOffset
                                : 0)
                            + (childComponent.Anchor.RightDetails.AnchorSide == AnchorDirections.RIGHT && childComponent.Anchor.RightDetails.AnchorUnits == AnchorUnits.PIXELS
                                ? childComponent.Anchor.RightDetails.AnchorOffset
                                : 0));
                    }
                    //Determine minimum pixel height
                    if (!Anchor.TopDetails.Strict || !Anchor.BottomDetails.Strict)
                    {
                        MinimumPixelHeight = Math.Max(
                            MinimumPixelHeight,
                            childComponent.MinimumPixelHeight
                            + (childComponent.Anchor.BottomDetails.AnchorSide == AnchorDirections.BOTTOM && childComponent.Anchor.BottomDetails.AnchorUnits == AnchorUnits.PIXELS
                                ? childComponent.Anchor.BottomDetails.AnchorOffset
                                : 0)
                            + (childComponent.Anchor.TopDetails.AnchorSide == AnchorDirections.TOP && childComponent.Anchor.TopDetails.AnchorUnits == AnchorUnits.PIXELS
                                ? childComponent.Anchor.TopDetails.AnchorOffset
                                : 0));
                    }
                }
            }
            //Calculate our minimum width alone
            if (Anchor.LeftDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.RightDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.LeftDetails.AnchorSide == Anchor.RightDetails.AnchorSide)
            {
                MinimumPixelWidth = Math.Max(MinimumPixelWidth, Math.Abs(Anchor.RightDetails.AnchorOffset - Anchor.LeftDetails.AnchorOffset));
            }
            //Calculate our minimum height alone
            if (Anchor.TopDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.BottomDetails.AnchorUnits == AnchorUnits.PIXELS
                && Anchor.TopDetails.AnchorSide == Anchor.BottomDetails.AnchorSide)
            {
                MinimumPixelHeight = Math.Max(MinimumPixelHeight, Math.Abs(Anchor.BottomDetails.AnchorOffset - Anchor.TopDetails.AnchorOffset));
            }
        }

        public void SetWidth(double width, double height)
        {
            //Set our width
            PixelWidth = Math.Max(MinimumPixelWidth, width);
            PixelHeight = Math.Max(MinimumPixelHeight, height);
            //Update child components
            foreach (IUserInterfaceComponent childComponent in Children)
            {
                childComponent.OnParentResized();
            }
            //Update our render core size
            
        }
    }
}
