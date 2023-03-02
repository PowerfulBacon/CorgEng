using CorgEng.UtilityTypes.BindableProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components
{
    /// <summary>
    /// TODO:
    /// - Automatic networking update
    /// - Updating this variable on the proper threads
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CVar<T> : BindableProperty<T>
    {

        public CVar() : this(default)
        {
        }

        public CVar(T value) : base(value)
        {
            // Determine our component type and prepare signal reaction handlers
        }

        public CVar<T> InitialValue(T value)
        {
            Value = value;
            return this;
        }

    }
}
