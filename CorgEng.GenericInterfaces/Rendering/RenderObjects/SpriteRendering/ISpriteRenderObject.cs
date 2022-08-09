using CorgEng.GenericInterfaces.Rendering.Icons;
using CorgEng.GenericInterfaces.Rendering.Renderers.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering
{
    public interface ISpriteRenderObject : IRenderObject
    {

        IBindableProperty<uint> TextureFile { get; set; }

        IBindableProperty<float> TextureFileX { get; set; }
        IBindableProperty<float> TextureFileY { get; set; }

        IBindableProperty<float> TextureFileHeight { get; set; }

        /// <summary>
        /// A bindable property representing the width of this icon within the parent texture file.
        /// Will automatically update the renderer if changed.
        /// </summary>
        IBindableProperty<float> TextureFileWidth { get; set; }

        /// <summary>
        /// If we are an overlay, this is the object we are being rendered inside of.
        /// </summary>
        ISpriteRenderObject Container { get; set; }

        /// <summary>
        /// The renderer that is currently rendering us.
        /// 
        /// This is managed internally by the renderer and shouldn't be
        /// assigned to outside of the renderer.
        /// </summary>
        ISpriteRenderer CurrentRenderer { get; set; }

        /// <summary>
        /// The transform of this, ignoring the parent transform.
        /// Required for overlays.
        /// 
        /// Updating the value of the transform will automatically
        /// </summary>
        IBindableProperty<IMatrix> SelfTransform { get; set; }

        /// <summary>
        /// The texture detail wrapper
        /// </summary>
        IBindablePropertyGroup TextureDetails { get; }

        /// <summary>
        /// Add an overlay to be rendered on top of this object
        /// </summary>
        /// <param name="overlay">The overlay to be rendered along with this object</param>
        void AddOverlay(IIcon overlay);

        /// <summary>
        /// Removes an overlay that is currently applied to this render object.
        /// </summary>
        /// <param name="overlay">The overlay to be removed</param>
        void RemoveOverlay(IIcon overlay);

    }
}
