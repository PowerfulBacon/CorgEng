using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Font.Characters;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Text;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.BindableProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.TextRendering
{
    internal class TextObject : ITextObject
    {

        private IFont font;

        public IBindableProperty<string> TextProperty { get; }

        public IBindableProperty<IVector<float>> ScreenPositionProperty { get; }

        public IBindableProperty<float> WidthProperty { get; }

        public IBindableProperty<float> HeightProperty { get; }

        public IBindableProperty<float> Scale { get; }

        //Collect a sprite renderer dependency so that we can draw our glyphs.
        [UsingDependency]
        private static ISpriteRenderer SpriteRenderer;

        public TextObject(string text)
        {
            //Create the text property
            TextProperty = new BindableProperty<string>(text);
        }

        /// <summary>
        /// Start rendering this text object.
        /// </summary>
        public void StartRendering()
        {
            double xPointer = ScreenPositionProperty.Value.X;
            double yPointer = ScreenPositionProperty.Value.Y;
            foreach (char character in TextProperty.Value)
            {
                //Get the character
                IFontCharacter fontCharacter = font.GetCharacter(character);
                //Render the character
                //Increment the pointers
                xPointer += fontCharacter.CharacterXAdvance * Scale.Value;
                //Moved past specified width, wrap around
                if (xPointer >= ScreenPositionProperty.Value.X + WidthProperty.Value)
                {
                    xPointer = ScreenPositionProperty.Value.X;
                    yPointer += Scale.Value;
                }
            }
        }

        public void StopRendering()
        {
            throw new NotImplementedException();
        }

    }
}
