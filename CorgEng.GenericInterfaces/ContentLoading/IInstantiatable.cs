using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents anything that can be created by the XML creator
/// </summary>
namespace CorgEng.GenericInterfaces.ContentLoading
{
    public interface IInstantiatable
    {

        IEntityDef TypeDef { get; set; }

        /// <summary>
        /// Set the property of this object
        /// </summary>
        void SetProperty(string name, IPropertyDef property);

        /// <summary>
        /// Called after the object has been instantiated and all the properties have been set.
        /// </summary>
        void Initialize(IVector<float> initializePosition);

        void PreInitialize(IVector<float> initializePosition);

    }
}
