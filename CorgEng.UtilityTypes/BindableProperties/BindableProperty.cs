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
                if (value == null && (!typeof(T).IsGenericType || typeof(T).GetGenericTypeDefinition() != typeof(Nullable<>)))
                    throw new ArgumentNullException("Attempting to set a bindable property of non-nullable type to null.");
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
