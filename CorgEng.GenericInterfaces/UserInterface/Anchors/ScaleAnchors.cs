using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UserInterface.Anchors
{
    /// <summary>
    /// Defines how a user interface element scales when a child element
    /// surparses its parent width/height.
    /// 
    /// For example a dropdown component should be contained within a scroll box.
    /// The scroll box should not be set to scale, but the dropdown component should scale
    /// to the size of its content, so when expanded all content is displayed within the scroll box.
    /// 
    /// <ScrollComponent scaleAnchor="none" left="left:0" right="right:0" top="top:0" bottom="bottom:0">
    ///     //By default will have a height of 0, but will expand so that children content can fit.
    ///     <Dropdown scaleAnchor="top_left" left="left:0" right="right:0" top="top:0" bottom="top:0">
    ///         <Text ... />
    ///         <DropdownContent ...>
    ///             <Text ... />
    ///             <Text ... />
    ///             <Text ... />
    ///             <Text ... />
    ///         </DropdownContent>
    ///     </Dropdown>
    /// </ScrollComponent>
    /// </summary>
    public enum ScaleAnchors
    {
        //Do not allow for user interface scaling.
        //Any child elements going out of the bounds of this element will be culled.
        NONE,
        //Expansion will be done from the top left corner.
        TOP_LEFT,
        //Expansion will be fron from the top right corner.
        TOP_RIGHT,
        //Expansion will be done from the bottom left corner
        BOTTOM_LEFT,
        //Expansion will be done from the bottom right corner.
        BOTTOM_RIGHT,
    }
}
