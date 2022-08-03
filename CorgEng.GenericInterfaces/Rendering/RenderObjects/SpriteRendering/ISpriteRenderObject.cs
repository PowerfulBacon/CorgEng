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
        IBindableProperty<float> TextureFileWidth { get; set; }

        /// <summary>
        /// The texture detail wrapper
        /// </summary>
        IBindablePropertyGroup TextureDetails { get; }

        /// <summary>
        /// Add an overlay to be rendered on top of this object
        /// </summary>
        /// <param name="overlay">The overlay to be rendered along with this object</param>
        void AddOverlay(ISpriteRenderObject overlay);

        /// <summary>
        /// Removes an overlay that is currently applied to this render object.
        /// </summary>
        /// <param name="overlay">The overlay to be removed</param>
        void RemoveOverlay(ISpriteRenderObject overlay);

    }
}
