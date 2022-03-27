using System;

namespace CorgEng.UtilityTypes.BindableProperties
{
    public interface IBindableProperty<T>
    {

        //Callback for when the value is changed
        event EventHandler ValueChanged;

        //The actual value of the bindable property
        T Value { get; set; }

    }
}
