using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.AiBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.AiBehaviour.Components
{
    /// <summary>
    /// Class that holds the behaviour controller
    /// for pawns.
    /// This is basically the brain of the pawn,
    /// it decides what to do and then other components
    /// will handle it.
    /// </summary>
    public class AiBehaviourComponent : Component
    {

        /// <summary>
        /// The behaviour manager for this entity.
        /// </summary>
        public IBehaviourManager BehaviourManager { get; set; }

    }

}
