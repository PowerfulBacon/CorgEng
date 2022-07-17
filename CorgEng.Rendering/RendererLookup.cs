using CorgEng.GenericInterfaces.Rendering.SharedRenderAttributes;
using CorgEng.GenericInterfaces.UtilityTypes.Batches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Rendering
{
    public static class RendererLookup
    {

        /// <summary>
        /// The renderer lookup table
        /// </summary>
        private static IDictionary<uint, object> rendererLookup = new Dictionary<uint, object>();

        internal static void AddRenderer<T, R>(InstancedRenderer<T, R> renderer)
            where T : ISharedRenderAttributes
            where R : IBatch<R>, new()
        {
            rendererLookup.Add(renderer.NetworkIdentifier, renderer);
        }

        public static T GetRendererByIdentifier<T>(uint identifier)
        {
            return (T)rendererLookup[identifier];
        }

    }
}
