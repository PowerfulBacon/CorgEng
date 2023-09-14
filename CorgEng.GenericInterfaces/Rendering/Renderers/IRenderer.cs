using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Rendering.Renderers
{
    public interface IRenderer
    {

		/// <summary>
		/// The width in pixels of the renderer
		/// </summary>
		int Width { get; }

		/// <summary>
		/// The height in pixels of the renderer
		/// </summary>
		int Height { get; }

		/// <summary>
		/// The uint of the frame buffer
		/// </summary>
		uint FrameBufferUint { get; }

		/// <summary>
		/// The uint of our render texture
		/// </summary>
		uint RenderTextureUint { get; }

		/// <summary>
		/// Initialise the renderer
		/// </summary>
		void Initialize();

		/// <summary>
		/// Render the context of the renderer
		/// </summary>
		/// <param name="camera"></param>
        void Render(ICamera camera);

		/// <summary>
		/// Draws the rendered frame buffer to the screen (Framebuffer 0)
		/// </summary>
		void DrawToBuffer(uint buffer, int drawX, int drawY, int bufferWidth, int bufferHeight);

		/// <summary>
		/// Resize the render core to the specified size.
		/// </summary>
		/// <param name="width">The width in pixels of the new render core.</param>
		/// <param name="height">The height in pixels of the new render core.</param>
		void Resize(int width, int height);

	}
}
