using CorgEng.Core.Dependencies;
using CorgEng.GenericInterfaces.Font.Characters;
using CorgEng.GenericInterfaces.Font.Fonts;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.Rendering.Text;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.BindableProperties;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering.TextRendering
{
    /// <summary>
    /// Abstract text object
    /// Interface between rendering and the entity that contains a text component
    /// </summary>
    internal class TextObject : ITextObject
    {

        //We require a sprite render object factory for creating our renderable characters
        [UsingDependency]
        private static ISpriteRenderObjectFactory SpriteRenderObjectFactory;

        //The font we are using for rendering
        private IFont font;

        /// <summary>
        /// The text property 
        /// </summary>
        public IBindableProperty<string> TextProperty { get; }

        //TODO
        public IBindableProperty<IVector<float>> ScreenPositionProperty { get; }

        public IBindableProperty<float> Scale { get; } = new BindableProperty<float>(0.1f);

        //Collect a sprite renderer dependency so that we can draw our glyphs.
        private ISpriteRenderer spriteRenderer;

        public TextObject(ISpriteRenderer spriteRenderer, IFont font, string text)
        {
            this.font = font;
            this.spriteRenderer = spriteRenderer;
            //Create the text property
            TextProperty = new BindableProperty<string>(text);
            ScreenPositionProperty = new BindableProperty<IVector<float>>(new Vector<float>(0, 0));
        }

        public void StartRendering()
        {
            //Begin drawing initially
            Draw(null, null);
            //Begin listening for updates to the text and screen position
            TextProperty.ValueChanged += Draw;
            ScreenPositionProperty.ValueChanged += Draw;
            Scale.ValueChanged += Draw;
        }

        /// <summary>
        /// Draw the text object, clearing this text object if it was already being drawn.
        /// </summary>
        private void Draw(object sender, EventArgs eventArgs)
        {
            //Setup the pointers for where we should be drawing the characters
            double xPointer = ScreenPositionProperty.Value.X;
            double yPointer = ScreenPositionProperty.Value.Y + (Scale.Value * font.Base / font.FontHeight);
            //Get each character in the text property that needs rendering
            foreach (char textCharcter in TextProperty.Value)
            {
                //Fetch the required character
                IFontCharacter fontCharacter = font.GetCharacter(textCharcter);
                if (fontCharacter == null)
                    continue;
                //Create the character
                ISpriteRenderObject renderObject = SpriteRenderObjectFactory.CreateSpriteRenderObject(
                    fontCharacter.TextureFile.TextureID,
                    (float)fontCharacter.TextureXPosition / fontCharacter.TextureFile.Width,
                    (float)fontCharacter.TextureYPosition / fontCharacter.TextureFile.Height,
                    (float)fontCharacter.TextureWidth / fontCharacter.TextureFile.Width,
                    (float)fontCharacter.TextureHeight / fontCharacter.TextureFile.Height,
                    10);
                renderObject.CombinedTransform.Value[3, 1] = (float)xPointer + Scale.Value * 4 * fontCharacter.CharacterXOffset / fontCharacter.TextureFile.Width;
                renderObject.CombinedTransform.Value[3, 2] = (float)yPointer - Scale.Value * 4 * fontCharacter.CharacterYOffset / fontCharacter.TextureFile.Height;
                renderObject.CombinedTransform.Value[1, 1] = Scale.Value * 8 * fontCharacter.TextureWidth / fontCharacter.TextureFile.Width;
                renderObject.CombinedTransform.Value[2, 2] = Scale.Value * 8 * fontCharacter.TextureHeight / fontCharacter.TextureFile.Height;
                //Start rendering the character
                spriteRenderer.StartRendering(renderObject);
                //Move forward the pointers (Account for the font width)
                xPointer += fontCharacter.CharacterXAdvance / (double)font.FontWidth * 8 * Scale.Value;
                //xPointer += 1;
            }
        }

        public void StopRendering()
        {
            throw new NotImplementedException();
        }

    }
}
