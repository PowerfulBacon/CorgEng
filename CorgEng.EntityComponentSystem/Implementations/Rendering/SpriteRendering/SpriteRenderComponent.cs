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

        public EntityDef TypeDef { get; set; }

        public void Initialize(IVector<float> initializePosition)
        { }

        public void PreInitialize(IVector<float> initializePosition)
        { }

        public void SetProperty(string name, PropertyDef property)
        {
            switch (name)
            {
                case "SpriteRenderObject":
                    SpriteRenderObject = property.GetValue(Vector<float>.Zero) as ISpriteRenderObject;
                    return;
            }
            throw new NotImplementedException($"Unknown property name {name}. Check configuration file for {TypeDef.Name}");
        }
    }
}
