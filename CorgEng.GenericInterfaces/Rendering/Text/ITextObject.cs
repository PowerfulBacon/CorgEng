using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Text
{
    public interface ITextObject
    {

        /// <summary>
        /// The text property that represents the text for this object.
        /// Bindable, so that when changed, the renderable can automatically update.
        /// </summary>
        IBindableProperty<string> TextProperty { get; }

        /// <summary>
        /// The screen position of the text object.
        /// </summary>
        IBindableProperty<IVector<float>> ScreenPositionProperty { get; }

        /// <summary>
        /// Start rendering this text object
        /// </summary>
        void StartRendering();

        /// <summary>
        /// Stop rendering this text object
        /// </summary>
        void StopRendering();

    }
}
