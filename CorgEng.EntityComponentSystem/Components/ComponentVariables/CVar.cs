using CorgEng.UtilityTypes.BindableProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components.ComponentVariables
{
    /// <summary>
    /// TODO:
    /// - Automatic networking update
    /// - Updating this variable on the proper threads
    /// </summary>
    /// <typeparam name="TValueType"></typeparam>
    public class CVar<TValueType, TComponentType> : BindableProperty<TValueType>, IComponentVariable
        where TComponentType : Component
    {

        public Component Parent { get; set; }

        public CVar() : this(default)
        {
        }

        public CVar(TValueType value) : base(value)
        {
            // Determine our component type and prepare signal reaction handlers
        }

        public void AssociateTo(Component component)
        {
            Parent = component;
        }

        public CVar<TValueType, TComponentType> InitialValue(TValueType value)
        {
            Value = value;
            return this;
        }

    }
}
