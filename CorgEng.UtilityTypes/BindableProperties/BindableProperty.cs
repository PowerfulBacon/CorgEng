﻿using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.BindableProperties
{
    public class BindableProperty<T> : IBindableProperty<T>
    {

        protected T _value = default;
        public virtual T Value
        {
            get => _value;
            set {
                if (value == null && (!typeof(T).IsGenericType || typeof(T).GetGenericTypeDefinition() != typeof(Nullable<>)))
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

        public virtual void TriggerChanged()
        {
            ValueChanged?.Invoke(_value, EventArgs.Empty);
        }
    }
}
