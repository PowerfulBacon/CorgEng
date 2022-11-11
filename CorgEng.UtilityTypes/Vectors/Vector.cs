using CorgEng.GenericInterfaces.Networking.Serialisation;
using CorgEng.GenericInterfaces.UtilityTypes;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace CorgEng.UtilityTypes.Vectors
{

    public struct Vector<T> : IVector<T>, ICustomSerialisationBehaviour
        where T : unmanaged
    {

        public static Vector<float> Zero => new Vector<float>(0, 0);

        private int sizeOfT;

        public Vector(params T[] values)
        {
            Values = values;
            OnChange = null;
            sizeOfT = Marshal.SizeOf(typeof(T));
        }

        private T[] Values;

        public event EventHandler OnChange;

        /// <summary>
        /// Overload for the [] operator, returns the element of the vector.
        /// </summary>
        public T this[int x]
        {
            get { return Values[x]; }
            set {
                Values[x] = value;
                OnChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public T X
        {
            get => this[0];
            set => this[0] = value;
        }

        public T Y
        {
            get => this[1];
            set => this[1] = value;
        }

        public T Z
        {
            get => this[2];
            set => this[2] = value;
        }

        /// <summary>
        /// Gets the dimensions of the vector
        /// </summary>
        public int Dimensions
        {
            get { return Values.Length; }
        }

        public Vector<T> IgnoreZ()
        {
            return new Vector<T>(this[0], this[1]);
        }

        public Vector<T> SetZ(T zValue)
        {
            return new Vector<T>(this[0], this[1], zValue);
        }

        public IVector<T> Copy()
        {
            T[] valuesCopy = new T[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                valuesCopy[i] = this[i];
            }
            return new Vector<T>(valuesCopy);
        }

        //TODO: REFACTOR THE LAYERING SYSTEM
        public Vector<T> MoveTowards(Vector<T> target, float speed, float deltaTime, out float extraDistance, bool ignoreZ = true)
        {
            if (speed == 0 || deltaTime == 0)
            {
                extraDistance = 0;
                return this;
            }
            Vector<T> thisCopy = (Vector<T>)Copy();
            Vector<T> trueTarget = target;
            Vector<T> trueThis = this;
            if (ignoreZ)
            {
                trueTarget = trueTarget.IgnoreZ();
                trueThis = trueThis.IgnoreZ();
            }
            float totalDistance = (trueTarget - trueThis).Length();
            float distanceMoved = speed * deltaTime;
            if (totalDistance < distanceMoved)
            {
                //Dimensional safe copy
                for (int i = 0; i < Math.Min(trueTarget.Dimensions, trueThis.Dimensions); i++)
                {
                    thisCopy[i] = trueTarget[i];
                }
                extraDistance = distanceMoved - totalDistance;
                return thisCopy;
            }
            for (int i = 0; i < Math.Min(trueTarget.Dimensions, trueThis.Dimensions); i++)
            {
                float dist = (dynamic)trueTarget[i] - trueThis[i];
                thisCopy[i] += (dynamic)(dist / totalDistance * (speed * deltaTime));
            }
            extraDistance = 0;
            return thisCopy;
        }

        public static bool operator ==(Vector<T> a, IVector<T> b)
        {
            if (Equals(a, null) || Equals(b, null))
                return Equals(a, b);
            if (a.Dimensions != b.Dimensions)
                return false;
            for (int i = 0; i < a.Dimensions; i++)
                if (!a[i].Equals(b[i]))
                    return false;
            return true;
        }

        public static bool operator !=(Vector<T> a, IVector<T> b)
        {
            if (Equals(a, null) || Equals(b, null))
                return !Equals(a, b);
            if (a.Dimensions != b.Dimensions)
                return true;
            for (int i = 0; i < a.Dimensions; i++)
                if (!a[i].Equals(b[i]))
                    return true;
            return false;
        }

        /// <summary>
        /// Returns the vector multiplied by -1
        /// </summary>
        public static Vector<T> operator -(Vector<T> input) => (dynamic)input * -1;

        /// <summary>
        /// Returns the vector.
        /// </summary>
        public static Vector<T> operator +(Vector<T> input) => input;

        /// <summary>
        /// Adds 2 vectors together
        /// </summary>
        public static Vector<T> operator +(Vector<T> a, IVector<T> b)
        {
            //Calculate the resulting dimensions of the new vector
            int resultingDimensions = Math.Max(a.Dimensions, b.Dimensions);
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[resultingDimensions]);
            //Add the values
            for (int i = 0; i < resultingDimensions; i++)
                vector[i] = (i < a.Dimensions ? (dynamic)a[i] : 0) + (i < b.Dimensions ? (dynamic)b[i] : 0);
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Adds a constant value b to all values of a
        /// </summary>
        public static Vector<T> operator +(Vector<T> a, T b)
        {
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[a.Dimensions]);
            //Add the values
            for (int i = 0; i < a.Dimensions; i++)
                vector[i] = (dynamic)a[i] + b;
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Takes the difference of 2 vectors
        /// </summary>
        public static Vector<T> operator -(Vector<T> a, IVector<T> b)
        {
            //Calculate the resulting dimensions of the new vector
            int resultingDimensions = Math.Max(a.Dimensions, b.Dimensions);
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[resultingDimensions]);
            //Add the values
            for (int i = 0; i < resultingDimensions; i++)
                vector[i] = (i < a.Dimensions ? (dynamic)a[i] : 0) - (i < b.Dimensions ? (dynamic)b[i] : 0);
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Subtracts a constant value of b from all values of a.
        /// </summary>
        public static Vector<T> operator -(Vector<T> a, T b)
        {
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[a.Dimensions]);
            //Add the values
            for (int i = 0; i < a.Dimensions; i++)
                vector[i] = (dynamic)a[i] - b;
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Static wrapper for the dot product.
        /// Simply calls the dot product method on A with parameter B.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static T DotProduct(IVector<T> a, IVector<T> b)
        {
            return a.DotProduct(b);
        }

        /// <summary>
        /// Calculates the dot product of this vector with another vector
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public T DotProduct(IVector<T> other)
        {
            dynamic result = 0;
            //Add the values
            for (int i = 0; i < Math.Max(Dimensions, other.Dimensions); i++)
                result += (dynamic)this[i] * other[i];
            //Return the resulting vector
            return (T)result;
        }

        /// <summary>
        /// Calculates the dot product of a and b
        /// </summary>
        public static Vector<T> operator *(Vector<T> a, IVector<T> b)
        {
            //Calculate the resulting dimensions of the new vector
            int resultingDimensions = Math.Max(a.Dimensions, b.Dimensions);
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[resultingDimensions]);
            //Add the values
            for (int i = 0; i < resultingDimensions; i++)
                vector[i] = (i < a.Dimensions ? (dynamic)a[i] : 0) * (i < b.Dimensions ? (dynamic)b[i] : 0);
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Multiplies a by a scalar b
        /// </summary>
        public static Vector<T> operator *(T b, Vector<T> a) => a * b;
        public static Vector<T> operator *(Vector<T> a, T b)
        {
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[a.Dimensions]);
            //Add the values
            for (int i = 0; i < a.Dimensions; i++)
                vector[i] = (dynamic)a[i] * b;
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Divides the vector a by the values of the vector b
        /// </summary>
        public static Vector<T> operator /(Vector<T> a, IVector<T> b)
        {
            //Calculate the resulting dimensions of the new vector
            int resultingDimensions = Math.Max(a.Dimensions, b.Dimensions);
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[resultingDimensions]);
            //Add the values
            for (int i = 0; i < resultingDimensions; i++)
                vector[i] = (i < a.Dimensions ? (dynamic)a[i] : 0) / (i < b.Dimensions ? (dynamic)b[i] : 0);
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Divides the values of a by the scalar b
        /// </summary>
        public static Vector<T> operator /(Vector<T> a, T b)
        {
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[a.Dimensions]);
            //Add the values
            for (int i = 0; i < a.Dimensions; i++)
                vector[i] = (dynamic)a[i] / b;
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Raises a to the power of b
        /// </summary>
        public static Vector<T> operator ^(Vector<T> a, T b)
        {
            //Create the new vector
            Vector<T> vector = new Vector<T>(new T[a.Dimensions]);
            //Add the values
            for (int i = 0; i < a.Dimensions; i++)
                vector[i] = (T)Math.Pow((dynamic)a[i], (dynamic)b);
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Casting
        /// </summary>
        public static implicit operator Vector<int>(Vector<T> a) => ChangeType<int>(a);
        public static implicit operator Vector<float>(Vector<T> a) => ChangeType<float>(a);
        public static implicit operator Vector<double>(Vector<T> a) => ChangeType<double>(a);
        public static implicit operator Vector<long>(Vector<T> a) => ChangeType<long>(a);

        private static Vector<L> ChangeType<L>(Vector<T> a)
            where L : unmanaged
        {
            //Create the new vector
            Vector<L> vector = new Vector<L>(new L[a.Dimensions]);
            //Add the values
            for (int i = 0; i < a.Dimensions; i++)
                vector[i] = (L)(dynamic)a[i];
            //Return the resulting vector
            return vector;
        }

        /// <summary>
        /// Calculates the length of the vector
        /// </summary>
        public float Length()
        {
            return (float)Math.Sqrt((float)(object)DotProduct(this, this));
        }

        public override string ToString()
        {
            return $"{{{string.Join(", ", Values)}}}";
        }

        public override bool Equals(object obj)
        {
            return obj is Vector<T> && (Vector<T>)obj == this;
        }

        public override int GetHashCode()
        {
            int hashCode = -1466858141;
            foreach (T value in Values)
            {
                hashCode = unchecked(hashCode * 17 + (int)(object)value);
            }
            hashCode = hashCode * -1521134295 + Dimensions.GetHashCode();
            return hashCode;
        }

        public int GetSerialisationLength()
        {
            return 4 + sizeOfT * Values.Length;
        }

        public void DeserialiseFrom(BinaryReader binaryReader)
        {
            int length = binaryReader.ReadInt32();
            //Construct
            //new T[length] causes a stack overflow exception.
            Values = new T[length];
            OnChange = null;
            sizeOfT = Marshal.SizeOf(typeof(T));
            //Read Ts
            for (int i = length - 1; i >= 0; i--)
            {
                if (typeof(ICustomSerialisationBehaviour).IsAssignableFrom(typeof(T)))
                {
                    ICustomSerialisationBehaviour thing = (ICustomSerialisationBehaviour)FormatterServices.GetUninitializedObject(typeof(T));
                    thing.DeserialiseFrom(binaryReader);
                    Values[i] = (T)thing;
                }
                //Assuming its a string
                else if (typeof(T) == typeof(string))
                {
                    ushort stringLength = binaryReader.ReadUInt16();
                    Values[i] = (T)System.Convert.ChangeType(Encoding.ASCII.GetString(binaryReader.ReadBytes(stringLength)), typeof(T));
                }
                else if (typeof(T) == typeof(byte))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadByte(), typeof(T));
                else if (typeof(T) == typeof(char))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadChar(), typeof(T));
                else if (typeof(T) == typeof(int))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadInt32(), typeof(T));
                else if (typeof(T) == typeof(float))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadSingle(), typeof(T));
                else if (typeof(T) == typeof(double))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadDouble(), typeof(T));
                else if (typeof(T) == typeof(long))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadInt64(), typeof(T));
                else if (typeof(T) == typeof(short))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadInt16(), typeof(T));
                else if (typeof(T) == typeof(uint))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadUInt32(), typeof(T));
                else if (typeof(T) == typeof(ushort))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadUInt16(), typeof(T));
                else if (typeof(T) == typeof(ulong))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadUInt64(), typeof(T));
                else if (typeof(T) == typeof(decimal))
                    Values[i] = (T)System.Convert.ChangeType(binaryReader.ReadDecimal(), typeof(T));
                else
                    binaryReader.ReadBytes(sizeOfT);
            }
        }

        public void SerialiseInto(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Values.Length);
            for (int i = Values.Length - 1; i >= 0; i--)
            {
                object value = Values[i];
                if (value is ICustomSerialisationBehaviour customSerialisationBehaviour)
                    customSerialisationBehaviour.SerialiseInto(binaryWriter);
                else if (value is byte[] byteArray)
                    binaryWriter.Write(byteArray);
                else if (value is byte valueByte)
                    binaryWriter.Write(valueByte);
                else if (value is char valueChar)
                    binaryWriter.Write(valueChar);
                else if (value is int valueInt)
                    binaryWriter.Write(valueInt);
                else if (value is float valueFloat)
                    binaryWriter.Write(valueFloat);
                else if (value is double valueDouble)
                    binaryWriter.Write(valueDouble);
                else if (value is long valueLong)
                    binaryWriter.Write(valueLong);
                else if (value is short valueShort)
                    binaryWriter.Write(valueShort);
                else if (value is uint valueUint)
                    binaryWriter.Write(valueUint);
                else if (value is ushort valueUshort)
                    binaryWriter.Write(valueUshort);
                else if (value is ulong valueUlong)
                    binaryWriter.Write(valueUlong);
                else if (value is decimal valueDecimal)
                    binaryWriter.Write(valueDecimal);
                else
                    binaryWriter.Seek(Marshal.SizeOf(value), SeekOrigin.Current);
            }
        }

        public float DistanceTo(IVector<T> other)
        {
            Vector<T> otherCopy = new Vector<T>(new T[other.Dimensions]);
            for (int i = 0; i < other.Dimensions; i++)
            {
                otherCopy[i] = other[i];
            }
            return (otherCopy - this).Length();
        }

        public IVector<G> CastCopy<G>()
            where G : unmanaged
        {
            G[] valuesCopy = new G[Dimensions];
            for (int i = 0; i < Dimensions; i++)
            {
                valuesCopy[i] = (G)(object)this[i];
            }
            return new Vector<G>(valuesCopy);
        }

    }
}
