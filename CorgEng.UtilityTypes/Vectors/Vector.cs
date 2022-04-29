using CorgEng.GenericInterfaces.UtilityTypes;
using System;

namespace CorgEng.UtilityTypes.Vectors
{

    public class Vector<T> : IVector<T>
    {

        public static Vector<float> Zero => new Vector<float>(0, 0);

        public Vector(params T[] values)
        {
            Values = values;
            OnChange = null;
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

        public static bool operator ==(Vector<T> a, Vector<T> b)
        {
            if (Equals(a, null) || Equals(b, null)) return Equals(a, b);
            if (a.Dimensions != b.Dimensions) return false;
            for (int i = 0; i < a.Dimensions; i++) if (!a[i].Equals(b[i]))return false;
            return true;
        }

        public static bool operator !=(Vector<T> a, Vector<T> b)
        {
            if (Equals(a, null) || Equals(b, null)) return !Equals(a, b);
            if (a.Dimensions != b.Dimensions) return true;
            for (int i = 0; i < a.Dimensions; i++) if (!a[i].Equals(b[i])) return true;
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
        public static Vector<T> operator +(Vector<T> a, Vector<T> b)
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
        public static Vector<T> operator -(Vector<T> a, Vector<T> b)
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
        public static Vector<T> operator *(Vector<T> a, Vector<T> b)
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
        public static Vector<T> operator /(Vector<T> a, Vector<T> b)
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
        public static implicit operator Vector<int>(Vector<T> a) => Convert<int>(a);
        public static implicit operator Vector<float>(Vector<T> a) => Convert<float>(a);
        public static implicit operator Vector<double>(Vector<T> a) => Convert<double>(a);
        public static implicit operator Vector<long>(Vector<T> a) => Convert<long>(a);

        private static Vector<L> Convert<L>(Vector<T> a)
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
            return (float)Math.Sqrt((dynamic)DotProduct(this, this));
        }

        public override string ToString()
        {
            return $"{{{string.Join(", ", this)}}}";
        }

        public override bool Equals(object obj)
        {
            return obj is Vector<T> && (Vector<T>)obj == this;
        }

        public override int GetHashCode()
        {
            int hashCode = -1466858141;
            foreach (dynamic value in Values)
            {
                hashCode = unchecked(hashCode * 17 + (int)value);
            }
            hashCode = hashCode * -1521134295 + Dimensions.GetHashCode();
            return hashCode;
        }

    }
}
