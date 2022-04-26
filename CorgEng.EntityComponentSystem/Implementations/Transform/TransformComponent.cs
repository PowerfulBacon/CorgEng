﻿using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.ContentLoading;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public sealed class TransformComponent : Component
    {

        /// <summary>
        /// Start with a zero value
        /// </summary>
        public Vector<float> Position { get; internal set; } = new Vector<float>(0, 0);

        public override bool SetProperty(string name, IPropertyDef property)
        {
            return false;
        }

    }
}
