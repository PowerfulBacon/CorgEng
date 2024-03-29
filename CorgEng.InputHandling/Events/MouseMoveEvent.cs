﻿using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.InputHandling.Events
{
    [Obsolete("Standard input events have been replaced with an improved input handling system with rebindable events. See IInputHandler.")]
    public class MouseMoveEvent : IEvent
    {

        public double X { get; set; }

        public double Y { get; set; }

        public MouseMoveEvent(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
