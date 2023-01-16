using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.AiBehaviours
{
    public interface IBehaviourManager
    {

        /// <summary>
        /// Are we currently thinking?
        /// If true then process will not start again until it has completed.
        /// </summary>
        bool Thinking { get; set; }

        /// <summary>
        /// The position of our pawn
        /// </summary>
        IVector<float> Position { get; }

        /// <summary>
        /// The pawn that we belong to
        /// </summary>
        IEntity PawnEntity { get; set; }

        /// <summary>
        /// Process the behaviour manager node.
        /// </summary>
        Task Process();



        /// <summary>
        /// The currently performing action
        /// </summary>
        IBehaviourAction? CurrentAction { get; set; }

        /// <summary>
        /// Set the memory value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memory"></param>
        /// <param name="value"></param>
        void SetMemory<T>(string memory, T? value);

        /// <summary>
        /// Get the result of a memory
        /// </summary>
        /// <typeparam name="T">The type to get from the memory</typeparam>
        /// <param name="memory">The memory key to access</param>
        /// <returns>The value stored in the memory, or default if there is no value in the memory stored.</returns>
        T? GetMemory<T>(string memory);

        /// <summary>
        /// Set the memory value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memory"></param>
        /// <param name="value"></param>
        void SetPermanentMemory<T>(string memory, T? value);

        /// <summary>
        /// Get the result of a memory
        /// </summary>
        /// <typeparam name="T">The type to get from the memory</typeparam>
        /// <param name="memory">The memory key to access</param>
        /// <returns>The value stored in the memory, or default if there is no value in the memory stored.</returns>
        T? GetPermanentMemory<T>(string memory);

    }
}
