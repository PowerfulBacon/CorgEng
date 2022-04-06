using JetBrains.Annotations;
using System;

namespace CorgEng.Core.Dependencies
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field)]
    public class UsingDependencyAttribute : Attribute
    {
    }
}
