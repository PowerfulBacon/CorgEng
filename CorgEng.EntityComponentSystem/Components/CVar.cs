using CorgEng.UtilityTypes.BindableProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Components
{
    public class CVar<T> : BindableProperty<T>
    {

        public CVar(T value) : base(value)
        {
        }

    }
}
