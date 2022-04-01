using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.UtilityTypes
{
    public interface IBindablePropertyGroup : IBindableProperty<IVector<float>>
    {

        void Unbind();

    }
}
