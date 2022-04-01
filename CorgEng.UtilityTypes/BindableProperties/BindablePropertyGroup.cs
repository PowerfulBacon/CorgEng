using CorgEng.GenericInterfaces.UtilityTypes;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.UtilityTypes.BindableProperties
{
    public class BindablePropertyGroup : IBindablePropertyGroup
    {

        private IBindableProperty<float>[] boundProperties;

        private IVector<float> _value;
        public IVector<float> Value
        {
            get => _value;
            set
            {
                _value = value;
                TriggerChanged();
            }
        }

        public event EventHandler ValueChanged;

        public BindablePropertyGroup(params IBindableProperty<float>[] bindableProperties)
        {
            //Firstly, monitor what we have bound
            boundProperties = bindableProperties;
            //Create our _value
            float[] vectorVals = new float[boundProperties.Length];
            for (int i = 0; i < vectorVals.Length; i++)
            {
                //Store the value in our vector
                vectorVals[i] = boundProperties[i].Value;
                //Bind for changes
                boundProperties[i].ValueChanged += RecalculateValue;
            }
            //Create the vector
            _value = new Vector<float>(vectorVals);
        }

        public void Unbind()
        {
            for (int i = 0; i < boundProperties.Length; i++)
            {
                //Unbind the recalculate value property
                boundProperties[i].ValueChanged -= RecalculateValue;
            }
        }

        private void RecalculateValue(object src, EventArgs args)
        {
            for (int i = 0; i < boundProperties.Length; i++)
            {
                _value[i] = boundProperties[i].Value;
            }
        }

        public void TriggerChanged()
        {
            ValueChanged?.Invoke(_value, EventArgs.Empty);
        }

    }
}
