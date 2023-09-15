using CorgEng.GenericInterfaces.EntityComponentSystem;
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
        /// Setting this anchor should dynamically update the UI component.
        /// </summary>
        IAnchor Anchor { get; set; }

        /// <summary>
        /// The parent user interface component
        /// </summary>
        IUserInterfaceComponent Parent { get; set; }

        /// <summary>
        /// An entity to hold the components for this interface component,
        /// so we can have a component based user interface model despite
        /// the oversight of not implementing it this way initially
        /// </summary>
        IEntity ComponentHolder { get; } 

        /// <summary>
        /// If true, the user interface will automatically resize to always be fullscreen.
        /// </summary>
        bool Fullscreen { get; set; }

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
        /// Offset from the left of the parent component
        /// </summary>
        double LeftOffset { get; }

        /// <summary>
        /// Offset from the bottom of the parent component
        /// </summary>
        double BottomOffset { get; }

        /// <summary>
        /// The world relevant to this user interface component
        /// </summary>
        IWorld World { get; }

        /// <summary>
        /// Raw list of children
        /// </summary>
        List<IUserInterfaceComponent> Children { get; }

        /// <summary>
        /// The parameters
        /// </summary>
        IDictionary<string, string> Parameters { get; }

        /// <summary>
        /// Draw the user interface component to the screen
        /// </summary>
        void DrawToFramebuffer(uint frameBuffer);

        /// <summary>
        /// Add a user interface component to the children list
        /// </summary>
        /// <param name="userInterfaceComponent">The user interface component to add to the children of this one.</param>
        void AddChild(IUserInterfaceComponent userInterfaceComponent);

        /// <summary>
        /// Remove a user interface component from the list of children.
        /// </summary>
        /// <param name="userInterfaceComponent"></param>
        void RemoveChild(IUserInterfaceComponent userInterfaceComponent);

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

        /// <summary>
        /// Enable screencasting on this component.
        /// Even if not rendering, this will
        /// </summary>
        void EnableScreencast();

        /// <summary>
        /// Disable screencasting on this component.
        /// </summary>
        void DisableScreencast();

        /// <summary>
        /// Get the component that was at the screen position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        IUserInterfaceComponent Screencast(int relativeX, int relativeY);

        /// <summary>
        /// Add a new argument to the component with the specified value.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        void AddArgument(string argumentName, string argumentValue);

        /// <summary>
        /// Store an object in the user interface component
        /// </summary>
        /// <param name="dataName"></param>
        /// <param name="dataValue"></param>
        void AddData(string dataName, object dataValue);

        /// <summary>
        /// Unstore an object from the user interface component
        /// </summary>
        /// <param name="dataName"></param>
        void ClearData(string dataName);

        /// <summary>
        /// Get some stored data from the user interface component
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        object GetData(string dataName);

    }
}
