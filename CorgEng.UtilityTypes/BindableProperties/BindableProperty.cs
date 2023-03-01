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
            set
            {
                // Type validation
                if (value == null && typeof(T).GetGenericTypeDefinition() != typeof(Nullable<>))
                    throw new ArgumentNullException("Attempting to set a bindable property of non-nullable type to null.");
                // Stop listening for old value changes
                if (_value != null && value is IListenable listener)
                    listener.ValueChanged -= ChangeReact;
                // Update the value
                _value = value;
                // Start listening for value changes
                if (_value != null && value is IListenable listener2)
                    listener2.ValueChanged += ChangeReact;
                // Indicate that we have changed
                TriggerChanged();
            }
        }

        public event EventHandler ValueChanged;

        public BindableProperty(T value)
        {
            _value = value;
            // If the value is a listenable type, start listening
            if (value is IListenable listener)
                listener.ValueChanged += ChangeReact;
        }

        private void ChangeReact(object? sender, EventArgs e)
        {
            TriggerChanged();
        }

        public void TriggerChanged()
        {
            ValueChanged?.Invoke(_value, EventArgs.Empty);
        }
    }
}
