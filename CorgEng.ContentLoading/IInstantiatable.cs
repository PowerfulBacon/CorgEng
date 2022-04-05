using CorgEng.ContentLoading.XmlDataStructures;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.ContentLoading
{
    public interface IInstantiatable
    {

        EntityDef TypeDef { get; set; }

        /// <summary>
        /// Set the property of this object
        /// </summary>
        void SetProperty(string name, object property);

        /// <summary>
        /// Called after the object has been instantiated and all the properties have been set.
        /// </summary>
        void Initialize(IVector<float> initializePosition);

        void PreInitialize(IVector<float> initializePosition);

    }
}
