﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.GenericInterfaces.Networking.Serialisation
{
    public interface ICustomSerialisationBehaviour
    {

        /// <summary>
        /// Get the length, in bytes, required to serialise this object.
        /// </summary>
        int GetSerialisationLength();

        /// <summary>
        /// Serialize this object into the provided array, at the specified
        /// index.
        /// Only needs to serialise data, the shared serialiser will automatically
        /// inject the length and type of serialisable object into byte array.
        /// </summary>
        void SerialiseInto(byte[] array, int index);

        /// <summary>
        /// Deserialize the byte array into this object.
        /// </summary>
        /// <param name="array">The array we are deserialising from.</param>
        /// <param name="index">The index that we want to start deserializing from</param>
        /// <param name="length">The length of bytes that we occupy in the array.</param>
        void DeserialiseFrom(byte[] array, int index, int length);

    }
}
