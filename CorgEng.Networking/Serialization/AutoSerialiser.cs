using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CorgEng.Networking.Serialization
{
    [Dependency]
    internal class AutoSerialiser : IAutoSerialiser
    {
        public object Deserialize(Type deserialisationType, BinaryReader binaryReader)
        {
            //Set the value based on the property type
            if (typeof(ICustomSerialisationBehaviour).IsAssignableFrom(deserialisationType))
            {
                ICustomSerialisationBehaviour createdObject = (ICustomSerialisationBehaviour)FormatterServices.GetUninitializedObject(deserialisationType);
                createdObject.DeserialiseFrom(binaryReader);
                return createdObject;
            }
            else if (deserialisationType == typeof(string))
            {
                ushort stringLength = binaryReader.ReadUInt16();
                byte[] byteArray = binaryReader.ReadBytes(stringLength);
                return Encoding.ASCII.GetString(byteArray);
            }
            else if (deserialisationType == typeof(byte))
                return binaryReader.ReadByte();
            else if (deserialisationType == typeof(char))
                return binaryReader.ReadChar();
            else if (deserialisationType == typeof(int))
                return binaryReader.ReadInt32();
            else if (deserialisationType == typeof(float))
                return binaryReader.ReadSingle();
            else if (deserialisationType == typeof(double))
                return binaryReader.ReadDouble();
            else if (deserialisationType == typeof(long))
                return binaryReader.ReadInt64();
            else if (deserialisationType == typeof(short))
                return binaryReader.ReadInt16();
            else if (deserialisationType == typeof(uint))
                return binaryReader.ReadUInt32();
            else if (deserialisationType == typeof(ushort))
                return binaryReader.ReadUInt16();
            else if (deserialisationType == typeof(ulong))
                return binaryReader.ReadUInt64();
            else if (deserialisationType == typeof(decimal))
                return binaryReader.ReadDecimal();
            else
                throw new Exception($"Failed to deserialise object with type {deserialisationType}");
        }

        public int SerialisationLength(object value)
        {
            if (value is string)
            {
                return sizeof(byte) * ((string)value).Length + sizeof(ushort);
            }
            else if (value is ICustomSerialisationBehaviour serialisationBehaviour)
            {
                return serialisationBehaviour.GetSerialisationLength();
            }
            else if (value.GetType().IsPrimitive)
            {
                return Marshal.SizeOf(value);
            }
            else
            {
                return 0;
            }
        }

        public void SerializeInto(object value, BinaryWriter binaryWriter)
        {
            Type objectType = value.GetType();
            if (typeof(ICustomSerialisationBehaviour).IsAssignableFrom(objectType))
                ((ICustomSerialisationBehaviour)value).SerialiseInto(binaryWriter);
            else if (objectType == typeof(string))
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(value.ToString());
                binaryWriter.Write((ushort)byteArray.Length);
                binaryWriter.Write(byteArray);
            }
            else if (objectType == typeof(byte))
                binaryWriter.Write((byte)value);
            else if (objectType == typeof(char))
                binaryWriter.Write((char)value);
            else if (objectType == typeof(int))
                binaryWriter.Write((int)value);
            else if (objectType == typeof(float))
                binaryWriter.Write((float)value);
            else if (objectType == typeof(double))
                binaryWriter.Write((double)value);
            else if (objectType == typeof(long))
                binaryWriter.Write((long)value);
            else if (objectType == typeof(short))
                binaryWriter.Write((short)value);
            else if (objectType == typeof(uint))
                binaryWriter.Write((uint)value);
            else if (objectType == typeof(ushort))
                binaryWriter.Write((ushort)value);
            else if (objectType == typeof(ulong))
                binaryWriter.Write((ulong)value);
            else if (objectType == typeof(decimal))
                binaryWriter.Write((decimal)value);
            else
                throw new Exception($"Failed to serialise object of type {value.GetType()} with value {value}");
        }

    }
}
