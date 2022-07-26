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
        IUserInterfaceComponent CreateGenericUserInterfaceComponent(IUserInterfaceComponent parent, IAnchor anchorDetails);

        /// <summary>
        /// Creates a generic user interface component with no parent.
        /// This will create a root node of a user interface.
        /// </summary>
        /// <param name="anchorDetails">The anchor details of this component.</param>
        /// <returns>The created blank user interface component.</returns>
        IUserInterfaceComponent CreateGenericUserInterfaceComponent(IAnchor anchorDetails);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="parent"></param>
        /// <param name="anchorDetails"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IUserInterfaceComponent parent, IAnchor anchorDetails, params KeyValuePair<string, string>[] arguments);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="anchorDetails"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IUserInterfaceComponent CreateUserInterfaceComponent(string componentType, IAnchor anchorDetails, params KeyValuePair<string, string>[] arguments);

    }
}
