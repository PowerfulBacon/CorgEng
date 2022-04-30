using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Components
{
    /// <summary>
    /// A user interface component.
    /// The coordinate space for a user interface component is:
    /// 0 - Left or bottom of parent component
    /// 1 - Top or right of parent component.
    /// </summary>
    public interface IUserInterfaceComponent
    {

        /// <summary>
        /// The anchor data for this component.
        /// </summary>
        IAnchor Anchor { get; }

        /// <summary>
        /// The scale anchor information
        /// </summary>
        ScaleAnchors ScaleAnchor { get; }

        /// <summary>
        /// The parent user interface component
        /// </summary>
        IUserInterfaceComponent Parent { get; }

        /// <summary>
        /// The width of the user interface component in pixels
        /// </summary>
        double PixelWidth { get; }

        /// <summary>
        /// The height of the user interface component in pixels
        /// </summary>
        double PixelHeight { get; }

        /// <summary>
        /// The minimum possible width of the user interface component in pixels.
        /// Determined based on strictness & children minimum pixel widths.
        /// </summary>
        double MinimumPixelWidth { get; }

        /// <summary>
        /// The minimum possible height of the user inetrface component in pixels.
        /// </summary>
        double MinimumPixelHeight { get; }

        /// <summary>
        /// Render the user interface component
        /// </summary>
        void Render();

        /// <summary>
        /// Add a user interface component to the children list
        /// </summary>
        /// <param name="userInterfaceComponent">The user interface component to add to the children of this one.</param>
        void AddChild(IUserInterfaceComponent userInterfaceComponent);

        /// <summary>
        /// Gets a list of all children components
        /// </summary>
        /// <returns>A list of all children user interface components.</returns>
        List<IUserInterfaceComponent> GetChildren();

        /// <summary>
        /// Try and set the height of this user interface component.
        /// Will not be smaller than the minimum UI height.
        /// </summary>
        /// <param name="width">The width to set this UI component to. Must be greater than the minimum pixel width.</param>
        /// <param name="height">The height to set this UI component to. Must be greater than the minimum pixel width.</param>
        void SetWidth(double width, double height);

        /// <summary>
        /// Executed when the parent user interface component is resized.
        /// Used to recalculate our user interface scale.
        /// </summary>
        void OnParentResized();

        /// <summary>
        /// Recalculate the minimum pixel width and height
        /// </summary>
        void CalculateMinimumScales();

    }
}
