using CorgEng.EntityComponentSystem.Events;
using CorgEng.GenericInterfaces.EntityComponentSystem;
using CorgEng.UtilityTypes.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.EntityComponentSystem.Implementations.Transform
{
    public class MoveEvent : INetworkedEvent
    {

        public bool CanBeRaisedByClient => false;

        public Vector<float> OldPosition { get; set; }

        public Vector<float> NewPosition { get; set; }

        public MoveEvent(Vector<float> oldPosition, Vector<float> newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public unsafe byte[] Serialize()
        {
            //Get the old position pointers
            float oldPositionX = OldPosition.X;
            float* oldPositionXPointer = &oldPositionX;
            float oldPositionY = OldPosition.Y;
            float* oldPositionYPointer = &oldPositionY;
            float oldPositionZ = OldPosition.Z;
            float* oldPositionZPointer = &oldPositionZ;
            //Get the new position pointers
            float newPositionX = NewPosition.X;
            float* newPositionXPointer = &newPositionX;
            float newPositionY = NewPosition.Y;
            float* newPositionYPointer = &newPositionY;
            float newPositionZ = NewPosition.Z;
            float* newPositionZPointer = &newPositionZ;
            //Convert pointers to byte array
            return new byte[] {
                *(byte*)oldPositionXPointer,
                *(((byte*)oldPositionXPointer)+1),
                *(((byte*)oldPositionXPointer)+2),
                *(((byte*)oldPositionXPointer)+3),
                *(byte*)oldPositionYPointer,
                *(((byte*)oldPositionYPointer)+1),
                *(((byte*)oldPositionYPointer)+2),
                *(((byte*)oldPositionYPointer)+3),
                *(byte*)oldPositionZPointer,
                *(((byte*)oldPositionZPointer)+1),
                *(((byte*)oldPositionZPointer)+2),
                *(((byte*)oldPositionZPointer)+3),
                *(byte*)newPositionXPointer,
                *(((byte*)newPositionXPointer)+1),
                *(((byte*)newPositionXPointer)+2),
                *(((byte*)newPositionXPointer)+3),
                *(byte*)newPositionYPointer,
                *(((byte*)newPositionYPointer)+1),
                *(((byte*)newPositionYPointer)+2),
                *(((byte*)newPositionYPointer)+3),
                *(byte*)newPositionZPointer,
                *(((byte*)newPositionZPointer)+1),
                *(((byte*)newPositionZPointer)+2),
                *(((byte*)newPositionZPointer)+3)
            };
        }

        public unsafe void Deserialize(byte[] data)
        {
            fixed (byte* arrayStart = data)
            {
                float* floatArray = (float*)arrayStart;
                //Deserialise the data
                OldPosition = new Vector<float>(
                    *floatArray,
                    *(floatArray + 1),
                    *(floatArray + 2)
                    );
                NewPosition = new Vector<float>(
                    *(floatArray + 3),
                    *(floatArray + 4),
                    *(floatArray + 5)
                    );
            }
        }
    }
}
