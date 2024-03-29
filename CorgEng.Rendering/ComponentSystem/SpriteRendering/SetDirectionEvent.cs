﻿using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.GenericInterfaces.Rendering.Icons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Rendering.SpriteRendering
{
    public class SetDirectionEvent : INetworkedEvent
    {

        /// <summary>
        /// What directional state are we beign set to?
        /// </summary>
        public DirectionalState DirectionalState { get; private set; }

        public SetDirectionEvent(DirectionalState directionalState)
        {
            DirectionalState = directionalState;
        }

        /// <summary>
        /// This event is only raised by the server.
        /// </summary>
        public bool CanBeRaisedByClient => false;

        public void Deserialise(BinaryReader reader)
        {
            DirectionalState = (DirectionalState)reader.ReadInt32();
        }

        public void Serialise(BinaryWriter writer)
        {
            writer.Write((int)DirectionalState);
        }

        public int SerialisedLength()
        {
            return sizeof(int);
        }

    }
}
