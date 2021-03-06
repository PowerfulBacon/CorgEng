using CorgEng.GenericInterfaces.UtilityTypes;
using System;

namespace CorgEng.UtilityTypes.BindableProperties
{
    public class BindableProperty<T> : IBindableProperty<T>
    {

        private T _value = default;
        public T Value
        {
            get => _value;
            set {
                _value = value;
                TriggerChanged();
            }
        }

        public event EventHandler ValueChanged;

        public BindableProperty(T value)
        {
            _value = value;
        }

        public void TriggerChanged()
        {
            ValueChanged?.Invoke(_value, EventArgs.Empty);
        }
    }
}
