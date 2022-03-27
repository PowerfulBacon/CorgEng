using CorgEng.DependencyInjection.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.BindableProperties
{
    [Dependency(defaultDependency = true)]
    public class BindableProperty<T> : IBindableProperty<T>
    {

        private T _value = default;
        public T Value
        {
            get => _value;
            set {
                _value = value;
                ValueChanged?.Invoke(_value, EventArgs.Empty);
            }
        }

        public event EventHandler ValueChanged;

        public BindableProperty(T value)
        {
            _value = value;
        }
    }
}
