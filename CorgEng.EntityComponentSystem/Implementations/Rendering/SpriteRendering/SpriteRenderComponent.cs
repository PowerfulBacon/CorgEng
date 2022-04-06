using CorgEng.ContentLoading;
using CorgEng.ContentLoading.XmlDataStructures;
using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.GenericInterfaces.Rendering.RenderObjects.SpriteRendering;
using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SpriteRenderComponent : Component, IInstantiatable
    {

        public ISpriteRenderObject SpriteRenderObject;

        public override bool SetProperty(string name, IPropertyDef property)
        {
            switch (name)
            {
                case "SpriteRenderObject":
                    SpriteRenderObject = property.GetValue(Vector<float>.Zero) as ISpriteRenderObject;
                    return true;
            }
            return false;
        }
    }
}
