using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UserInterface.Anchors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Components
{
    public interface IUserInterfaceComponentFactory
    {

        /// <summary>
        /// Creates a generic user interface component as a child of some component.
        /// </summary>
        /// <param name="parent">The parent of this component.</param>
        /// <param name="anchorDetails">The anchor of this component. Defines where the user interface component will be rendered relative to the parent.</param>
        /// <returns>The created blank user interface component.</returns>
        IUserInterfaceComponent CreateGenericUserInterfaceComponent(IWorld world, IUserInterfaceComponent parent, IAnchor anchorDetails, Action<IUserInterfaceComponent> preInitialiseAction);

        /// <summary>
        /// Creates a generic user interface component with no parent.
        /// This will create a root node of a user interface.
        /// </summary>
        /// <param name="anchorDetails">The anchor details of this component.</param>
        /// <returns>The created blank user interface component.</returns>
        IUserInterfaceComponent CreateGenericUserInterfaceComponent(IWorld world, IAnchor anchorDetails, Action<IUserInterfaceComponent> preInitialiseAction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="parent"></param>
        /// <param name="anchorDetails"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IUserInterfaceComponent CreateUserInterfaceComponent(IWorld world, string componentType, IUserInterfaceComponent parent, IAnchor anchorDetails, IDictionary<string, string> arguments, Action<IUserInterfaceComponent> preInitialiseAction);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="anchorDetails"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IUserInterfaceComponent CreateUserInterfaceComponent(IWorld world, string componentType, IAnchor anchorDetails, IDictionary<string, string> arguments, Action<IUserInterfaceComponent> preInitialiseAction);

    }
}
