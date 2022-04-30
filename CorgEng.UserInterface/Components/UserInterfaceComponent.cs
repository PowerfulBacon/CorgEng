using CorgEng.Core.Rendering;
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

        public IAnchor Anchor { get; }

        public IUserInterfaceComponent Parent { get; }

        public double PixelWidth { get; private set; }

        public double PixelHeight { get; private set; }

        public double MinimumPixelWidth { get; private set; }

        public double MinimumPixelHeight { get; private set; }

        private List<UserInterfaceComponent> Children { get; } = new List<UserInterfaceComponent>();

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
                //Determine our minimum scales.
            }
            //Calculate our minimum width alone
            if (Anchor.LeftDetails.AnchorUnits == AnchorUnits.PIXELS && Anchor.RightDetails.AnchorUnits == AnchorUnits.PIXELS)
            {

            }
        }

    }
}
