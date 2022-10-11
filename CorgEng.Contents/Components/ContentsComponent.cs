using CorgEng.EntityComponentSystem.Components;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Contents.Components
{
    public class ContentsComponent : Component
    {

        /// <summary>
        /// The entities that are currently stored inside of us.
        /// </summary>
        public List<IEntity> EntitiesInContents { get; } = new List<IEntity>();

        /// <summary>
        /// What to do with the children when we are destroyed.
        /// </summary>
        public ParentDestroyReaction DestroyReaction { get; set; } = ParentDestroyReaction.DESTROY_CHILDREN;

    }
}
