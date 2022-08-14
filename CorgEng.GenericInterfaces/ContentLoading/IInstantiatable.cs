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

        IEntityDefinition TypeDef { get; set; }

    }
}
