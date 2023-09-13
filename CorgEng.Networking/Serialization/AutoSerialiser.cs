using CorgEng.DependencyInjection.Dependencies;
using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.Serialization;
using CorgEng.Networking.VersionSync;
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

        private Type ConstructGenericType(Type startingType, BinaryReader binaryReader)
        {
            Type nextType = VersionGenerator.GetTypeFromNetworkedIdentifier(binaryReader.ReadUInt16());
            if (nextType.IsGenericType)
            {
                Type nextGenericType = ConstructGenericType(nextType, binaryReader);
                nextType = nextType.MakeGenericType(nextGenericType);
            }
            return nextType;
        }

        public object Deserialize(Type deserialisationType, BinaryReader binaryReader)
        {
            //Set the value based on the property type
            if (typeof(ICustomSerialisationBehaviour).IsAssignableFrom(deserialisationType))
            {
                // Wait, why do we need to do any of this if we already know the type we are deserialising??
                // ^ Because subtypes can be assigned to variables of a parent type.
                Type type = VersionGenerator.GetTypeFromNetworkedIdentifier(binaryReader.ReadUInt16());
                //Generic types need additional handling in order to determine the contained generic type
                if (type.IsGenericType)
                {
                    Type createdGenericType = ConstructGenericType(type, binaryReader);
                    type = type.MakeGenericType(createdGenericType);
                }
                ICustomSerialisationBehaviour createdObject = (ICustomSerialisationBehaviour)FormatterServices.GetUninitializedObject(type);
                createdObject.DeserialiseFrom(binaryReader);
                return createdObject;
            }
            else if (deserialisationType == typeof(string))
            {
                ushort stringLength = binaryReader.ReadUInt16();
                if (stringLength == 0)
                    return null;
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

        public int SerialisationLength(Type type, object value)
        {
            if (type == typeof(string))
            {
                if (value == null)
                {
                    return sizeof(ushort);
                }
                return sizeof(byte) * ((string)value).Length + sizeof(ushort);
            }
            if (value is ICustomSerialisationBehaviour serialisationBehaviour)
            {
                int genericSize = 0;
                Type current = value.GetType();
                while (current.IsGenericType)
                {
                    genericSize += sizeof(ushort);
                    current = current.GetGenericArguments()[0];
                }
                return sizeof(ushort) + genericSize + serialisationBehaviour.GetSerialisationLength();
            }
            if (type.IsPrimitive)
            {
                return Marshal.SizeOf(value);
            }
            return 0;
        }

        public void SerializeInto(Type type, object value, BinaryWriter binaryWriter)
        {
            try
            {
                Type objectType = type;
                if (typeof(ICustomSerialisationBehaviour).IsAssignableFrom(objectType))
                {
                    binaryWriter.Write(VersionGenerator.GetNetworkedIdentifier(value.GetType()));
                    Type current = value.GetType();
                    while (current.IsGenericType)
                    {
                        //TODO: Add embedded recursive types and multiple recursive type arguments.
                        current = current.GenericTypeArguments[0];
                        binaryWriter.Write(VersionGenerator.GetNetworkedIdentifier(current));
                    }
                    ((ICustomSerialisationBehaviour)value).SerialiseInto(binaryWriter);
                }
                else if (objectType == typeof(string))
                {
                    if (string.IsNullOrEmpty(value as string))
                    {
                        binaryWriter.Write((ushort)0);
                        return;
                    }
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
            catch (NotSupportedException e)
            {
                throw;
            }
        }

    }
}
