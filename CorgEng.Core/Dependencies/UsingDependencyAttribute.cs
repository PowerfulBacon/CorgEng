using JetBrains.Annotations;
using System;

namespace CorgEng.Core.Dependencies
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [UsedImplicitly(ImplicitUseKindFlags.Assign | ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [AttributeUsage(AttributeTargets.Field)]
    public class UsingDependencyAttribute : Attribute
    {
    }
}
