using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Serialization
{
    public interface IAutoSerialiser
    {

        /// <summary>
        /// Automatically serialize a value into a byte array
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="byteArray">The byte array to serialize into</param>
        /// <param name="startPointer">
        /// The index of the byte array that references where to start serializing into.
        /// Will be the value of the next free cell once serialization is completed.
        /// </param>
        void SerializeInto(object value, BinaryWriter binaryWriter);

        /// <summary>
        /// Automatically deserialize a byte array
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="startPointer"></param>
        /// <returns></returns>
        object Deserialize(Type deserialisationType, BinaryReader binaryReader);

        /// <summary>
        /// Determine the length of the serialised version of an object.
        /// </summary>
        /// <param name="value">The value to determine the length of</param>
        /// <returns></returns>
        int SerialisationLength(object value);

    }
}
