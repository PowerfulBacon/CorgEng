using System;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IBindableProperty<T> : IListenable
    {

        //The actual value of the bindable property
        T Value { get; set; }

        void TriggerChanged();

    }
}
