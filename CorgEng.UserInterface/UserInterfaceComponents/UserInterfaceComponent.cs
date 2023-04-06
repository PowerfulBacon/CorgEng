using CorgEng.Constants;
using CorgEng.Core;
using CorgEng.Core.Dependencies;
using CorgEng.Core.Rendering;
using CorgEng.GenericInterfaces.Core;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Logging;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using CorgEng.GenericInterfaces.UserInterface.Components;
using CorgEng.GenericInterfaces.UserInterface.Rendering;
using CorgEng.GenericInterfaces.UserInterface.Rendering.Renderer;
using CorgEng.UserInterface.Hooks;
using CorgEng.UserInterface.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenGL.Gl;

namespace CorgEng.UserInterface.Components
{
    internal class UserInterfaceComponent : RenderCore, IUserInterfaceComponent
    {

        private static int IdCounter = 0;

        [UsingDependency]
        private static ILogger Logger;

        /// <summary>
        /// The anchor details for this component
        /// </summary>
        private IAnchor _anchor;
        public IAnchor Anchor
        {
            get
            {
                return _anchor;
            }
            set
            {
                _anchor = value;
                // Give warnings if the anchor units are mismatched.
                // Mismatched anchor units could result in the right position being
                // further left than the left position.
                if (_anchor.LeftDetails.AnchorUnits != _anchor.RightDetails.AnchorUnits)
                    Logger?.WriteLine($"User interface component's left anchor's units ({_anchor.LeftDetails.AnchorUnits}) " +
                        $"does not match the component's right anchor's units ({_anchor.RightDetails.AnchorUnits}). " +
                        $"This may result in unexpected behaviour in regards to scaling UI components.", LogType.WARNING);
                if (_anchor.TopDetails.AnchorUnits != _anchor.BottomDetails.AnchorUnits)
                    Logger?.WriteLine($"User interface component's top anchor's units ({_anchor.TopDetails.AnchorUnits}) " +
                        $"does not match the component's bottom anchor's units ({_anchor.BottomDetails.AnchorUnits}). " +
                        $"This may result in unexpected behaviour in regards to scaling UI components.", LogType.WARNING);
                //Check for other maximum size limits, maximum size isn't supported.
                if (_anchor.LeftDetails.AnchorSide == AnchorDirections.RIGHT && _anchor.RightDetails.AnchorSide == AnchorDirections.LEFT)
                    Logger?.WriteLine($"User interface component's left anchor is right, and the right anchor is left. This will result in a maximum size which isn't supported.", LogType.WARNING);
                if (_anchor.BottomDetails.AnchorSide == AnchorDirections.TOP && _anchor.TopDetails.AnchorSide == AnchorDirections.BOTTOM)
                    Logger?.WriteLine($"User interface component's bottom anchor is top, and the top anchor is bottom. This will result in a maximum size which isn't supported.", LogType.WARNING);
                //Calculate minimum scales
                CalculateMinimumScales();
                //Trigger resize
                //Recalculate minimum UI scale
                GetTopLevelParent(this).CalculateMinimumScales();
                //Recalculate child component's scale
                //TODO: This didnt work
                //foreach (IUserInterfaceComponent child in Children)
                //{
                //    OnParentResized();
                //}
                //Check for expansion
                CheckExpansion(this);
            }
        }
        /// <summary>
        /// The parent component for this user interface component
        /// </summary>
        public IUserInterfaceComponent Parent { get; set; }

        /// <summary>
        /// An entity to hold the components for this interface component,
        /// so we can have a component based user interface model despite
        /// the oversight of not implementing it this way initially
        /// </summary>
        public IEntity ComponentHolder { get; }

        /// <summary>
        /// Is this component fullscreen?
        /// If so, it will stretch its width and height to match the screen's width and height.
        /// Only valid for root components.
        /// </summary>
        public bool Fullscreen { get; set; } = false;

        /// <summary>
        /// The width of this component in pixels.
        /// </summary>
        public double PixelWidth { get; private set; }

        /// <summary>
        /// The height of this component in pixels.
        /// </summary>
        public double PixelHeight { get; private set; }

        /// <summary>
        /// The minimum width this component can be.
        /// </summary>
        public double MinimumPixelWidth { get; private set; }

        /// <summary>
        /// The minimum height this component can be.
        /// </summary>
        public double MinimumPixelHeight { get; private set; }

        /// <summary>
        /// The left offset from the parent
        /// </summary>
        public double LeftOffset { get; private set; }

        /// <summary>
        /// The bottom offset from the parent
        /// </summary>
        public double BottomOffset { get; private set; }

        /// <summary>
        /// Include self in screencasting?
        /// if not, this element cannot be clicked, but its children can be clicked.
        /// </summary>
        protected virtual bool ScreencastInclude { get; } = true;

        /// <summary>
        /// Should this component be rendered?
        /// </summary>
        protected virtual bool RenderInclude { get; } = true;

        /// <summary>
        /// A list of all of the user interface components contained within outself
        /// </summary>
        public List<IUserInterfaceComponent> Children { get; } = new List<IUserInterfaceComponent>();

        public IDictionary<string, string> Parameters { get; private set; } = null;

        public IWorld World => world;

        /// <summary>
        /// A unique identifier for this component.
        /// </summary>
        private int uniqueId = IdCounter++;

        /// <summary>
        /// have we initialized yet?
        /// </summary>
        private bool initialized = false;

        private static object lockObject = new object();

        public UserInterfaceComponent(IWorld world, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments) : this(world, anchorDetails, arguments)
        {
            //Set the parent
            Parent = parent;
            //Set children
            Parent?.AddChild(this);
        }

        public UserInterfaceComponent(IWorld world, IAnchor anchorDetails, IDictionary<string, string> arguments) : base(world)
        {
            Parameters = arguments;
            RegisterArguments();
            ParseArguments(arguments);
            // Set the anchor details
            Anchor = anchorDetails;
            ComponentHolder = world.EntityManager.CreateEmptyEntity(null);
        }

        //====================================
        // Argument Parsing
        //====================================

        private Dictionary<string, Action<string>> argumentHandlers = new Dictionary<string, Action<string>>();

        protected void AddArgument(string argumentName, Action<string> argumentHandler)
        {
            argumentHandlers.Add(argumentName, argumentHandler);
        }

        /// <summary>
        /// Add a new argument to the component with the specified value.
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="argumentValue"></param>
        public void AddArgument(string argumentName, string argumentValue)
        {
            if (Parameters.ContainsKey(argumentName))
                Parameters[argumentName] = argumentValue;
            else
                Parameters.Add(argumentName, argumentValue);
            if (argumentHandlers.ContainsKey(argumentName))
            {
                argumentHandlers[argumentName].Invoke(argumentValue);
            }
        }

        private string? initialiseCallbackFunction = null;

        /// <summary>
        /// Register arguments. Provide default arguments
        /// </summary>
        /// <exception cref="Exception"></exception>
        protected virtual void RegisterArguments()
        {
            //Add onInitialise code block
            AddArgument("onInitialise", methodName =>
            {
                if (!UserInterfaceModule.KeyMethods.ContainsKey(methodName))
                {
                    throw new Exception($"No static method with the key '{methodName}' exists.");
                }
                //Immediately call the function
                initialiseCallbackFunction = methodName;
            });
            //Add the onClick thing
            AddArgument("onClick", methodName =>
            {
                if (!UserInterfaceModule.KeyMethods.ContainsKey(methodName))
                {
                    throw new Exception($"No static method with the key '{methodName}' exists.");
                }
                if (ComponentHolder.TryGetComponent(out UserInterfaceClickerComponent clickerComponent))
                    ComponentHolder.RemoveComponent(clickerComponent, false);
                ComponentHolder.AddComponent(new UserInterfaceClickerComponent(UserInterfaceModule.KeyMethods[methodName], this));
            });
        }

        private void ParseArguments(IDictionary<string, string> arguments)
        {
            foreach (KeyValuePair<string, string> argument in arguments)
            {
                if (argumentHandlers.ContainsKey(argument.Key))
                {
                    argumentHandlers[argument.Key].Invoke(argument.Value);
                }
                else
                {
                    Logger.WriteLine($"A user interface component added the parameter '{argument.Key}'='{argument.Value}', but it was not handled by the component of type {GetType()}.", LogType.WARNING);
                }
            }
        }

        //====================================
        // Component Rendering Interfaces
        //====================================

        private static void Render(UserInterfaceComponent userInterfaceComponent, uint buffer, bool drawOnTop = false)
        {
            //Not initalized yet
            if (!userInterfaceComponent.initialized)
                return;
            //Resize if necessary
            if (userInterfaceComponent.Fullscreen && userInterfaceComponent.Parent == null && (userInterfaceComponent.PixelWidth != CorgEngMain.MainRenderCore.Width || userInterfaceComponent.PixelHeight != CorgEngMain.MainRenderCore.Height))
            {
                userInterfaceComponent.SetWidth(CorgEngMain.MainRenderCore.Width, CorgEngMain.MainRenderCore.Height);
            }
            //Switch to the correct render core and draw it to the framebuffer
            if (userInterfaceComponent.RenderInclude)
            {
                userInterfaceComponent.DoRender();
            }
            else
            {
                //Clear the buffer
                userInterfaceComponent.PreRender();
            }
            //Draw children
            lock (lockObject)
            {
                foreach (IUserInterfaceComponent childComponent in userInterfaceComponent.GetChildren())
                {
                    //Render the child component to our buffer
                    Render(childComponent as UserInterfaceComponent, userInterfaceComponent.FrameBufferUint);
                }
            }
            if (drawOnTop)
            {
                glEnable(GL_DEPTH_TEST);
            }
            userInterfaceComponent.DrawToBuffer(
                buffer,
                (int)userInterfaceComponent.LeftOffset,
                (int)userInterfaceComponent.BottomOffset,
                (int)userInterfaceComponent.PixelWidth,
                (int)userInterfaceComponent.PixelHeight
            );
            if (drawOnTop)
            {
                glDisable(GL_DEPTH_TEST);
            }
        }

        public void DrawToFramebuffer(uint frameBuffer)
        {
            try
            {
                Render(this, frameBuffer, drawOnTop: true);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e, LogType.ERROR);
            }
        }

        //====================================
        // Component Handling
        //====================================

        public virtual void AddChild(IUserInterfaceComponent userInterfaceComponent)
        {
            lock (lockObject)
            {
                //Add the child
                Children.Add(userInterfaceComponent);
                //Recalculate minimum UI scale
                GetTopLevelParent(this).CalculateMinimumScales();
                //Recalculate child component's scale
                userInterfaceComponent.Parent = this;
                userInterfaceComponent.OnParentResized();
                //Check for expansion
                CheckExpansion(this);
            }
        }

        public void RemoveChild(IUserInterfaceComponent userInterfaceComponent)
        {
            lock (lockObject)
            {
                //Remove the interface component
                Children.Remove(userInterfaceComponent);
                //Recalculate minimum UI scale
                GetTopLevelParent(this).CalculateMinimumScales();
                //Recalculate child component's scale
                userInterfaceComponent.OnParentResized();
                //Check for expansion
                CheckExpansion(this);
            }
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

        public virtual List<IUserInterfaceComponent> GetChildren()
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
            if (Anchor.LeftDetails.AnchorUnits == AnchorUnits.PERCENTAGE)
            {
                if (Anchor.LeftDetails.AnchorSide == AnchorDirections.LEFT)
                    LeftOffset = Anchor.LeftDetails.AnchorOffset * (Parent.PixelWidth / 100.0);
                else
                    LeftOffset = (100.0 - Anchor.LeftDetails.AnchorOffset) * (Parent.PixelWidth / 100.0);
            }
            else
            {
                if (Anchor.LeftDetails.AnchorSide == AnchorDirections.LEFT)
                    LeftOffset = Anchor.LeftDetails.AnchorOffset;
                else
                    LeftOffset = Parent.PixelWidth - Anchor.LeftDetails.AnchorOffset;
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
            if (Anchor.BottomDetails.AnchorUnits == AnchorUnits.PERCENTAGE)
            {
                if (Anchor.BottomDetails.AnchorSide == AnchorDirections.BOTTOM)
                    BottomOffset = Anchor.BottomDetails.AnchorOffset * (Parent.PixelHeight / 100.0);
                else
                    BottomOffset = (100.0 - Anchor.BottomDetails.AnchorOffset) * (Parent.PixelHeight / 100.0);
            }
            else
            {
                if (Anchor.BottomDetails.AnchorSide == AnchorDirections.BOTTOM)
                    BottomOffset = Anchor.BottomDetails.AnchorOffset;
                else
                    BottomOffset = Parent.PixelHeight - Anchor.BottomDetails.AnchorOffset;
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
            desiredWidth = rightSideOffset - LeftOffset;
            desiredHeight = topSideOffset - BottomOffset;
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
                lock (lockObject)
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
            //Update our render core size
            Resize((int)PixelWidth, (int)PixelHeight);
            //Update child components
            lock (lockObject)
            {
                foreach (IUserInterfaceComponent childComponent in Children)
                {
                    childComponent.OnParentResized();
                }
            }
        }

        public override void Initialize()
        {
            if (initialiseCallbackFunction != null)
            {
                UserInterfaceModule.KeyMethods[initialiseCallbackFunction].Invoke(null, new object[] { this });
                initialiseCallbackFunction = null;
            }
            initialized = true;
        }

        public override void PerformRender()
        { }

        public virtual IUserInterfaceComponent Screencast(int relativeX, int relativeY)
        {
            //If outside bounds, return nothing
            if (relativeX < 0 || relativeY < 0 || relativeX > PixelWidth || relativeY > PixelHeight)
            {
                return null;
            }
            //Screencast children first (Children are above us)
            lock (lockObject)
            {
                foreach (IUserInterfaceComponent childComponent in Children)
                {
                    //Determine new relative positions
                    int newRelativeX = relativeX - (int)childComponent.LeftOffset;
                    int newRelativeY = relativeY - (int)childComponent.BottomOffset;
                    IUserInterfaceComponent selected = childComponent.Screencast(newRelativeX, newRelativeY);
                    if (selected != null)
                        return selected;
                }
            }
            //Nothing was hit
            return ScreencastInclude ? this : null;
        }

        public void EnableScreencast()
        {
            UserInterfaceClickHook.ScreencastingComopnents.Add(this);
        }

        public void DisableScreencast()
        {
            UserInterfaceClickHook.ScreencastingComopnents.Remove(this);
        }

        private Dictionary<string, object> dataStore = new Dictionary<string, object>();

        public void AddData(string dataName, object dataValue)
        {
            dataStore.Add(dataName, dataValue);
        }

        public void ClearData(string dataName)
        {
            if (dataStore.ContainsKey(dataName))
                dataStore.Remove(dataName);
        }

        public object GetData(string dataName)
        {
            return dataStore[dataName];
        }
    }
}
