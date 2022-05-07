using CorgEng.GenericInterfaces.UtilityTypes;
using System;

namespace CorgEng.UtilityTypes.Matrices
{

    public class InvalidMatrixDimensionError : Exception
    {
        public InvalidMatrixDimensionError(string message) : base(message) { }
    }

    /// <summary>
    /// Simple identity matrix struct that represents the identity matrix of size infinity x infinity.
    /// </summary>
    public struct IdentityMatrix
    {

        public Matrix this[int dimensions]
        {
            get
            {
                Matrix newMatrix = new Matrix(dimensions, dimensions);
                for (int i = 1; i <= dimensions; i++)
                    newMatrix[i, i] = 1;
                return newMatrix;
            }
        }

        public float this[int x, int y]
        {
            get { return x == y ? 1 : 0; }
        }
    }

    /// <summary>
    /// Completely overly complicated and pointless matrix struct that works with multiple dimensions.
    /// The engine only uses 4x4 matrices lol, what a waste of time.
    /// </summary>
    public class Matrix : IMatrix
    {

        //The identity matrix struct (can only be read from, operations cannot be performed on it)
        public static IdentityMatrix Identity = new IdentityMatrix();

        //Width and height of the matrix
        public int X { get; private set; }
        public int Y { get; private set; }

        //Values of the matrix
        //This shouldn't be used directly due to being extremely confusing
        //Index 0 = Y, index 1 = X
        //OPEN GL HANDLES MATRICES AS:
        //m0 m4 m8 m12
        //m1 m5 m9  m13
        //m2 m6 m10 m14
        //m3 m7 m11 m15
        //WHILE WE DO IT X FIRST
        private float[] Values { get; set; }

        /// <summary>
        /// The change event handler
        /// </summary>
        public event EventHandler OnChange = null;

        //We do have to use it for reference though
        public unsafe float* GetPointer()
        {
            fixed (float* returnValue = &Values[0])
            {
                return returnValue;
            }
        }

        public Matrix(float[,] values)
        {
            Y = values.GetLength(0);
            X = values.GetLength(1);
            //Convert the parsed values
            // {{x1y1, x2y1}, {x1y2, x2y2}}
            Values = new float[X * Y];

            for (int x = 0; x < X; x++)
            {
                for (int y = 0; y < Y; y++)
                {
                    Values[y + x * Y] = values[y, x];
                }
            }

        }

        public Matrix(int width, int height)
        {
            X = width;
            Y = height;
            Values = new float[height * width];
        }

        /// <summary>
        /// Indexer for the matrix
        /// M[1,1] = Element in the 1st row and 1st column of the matrix
        /// </summary>
        public float this[int x, int y]
        {
            get { return Values[y - 1 + (x - 1) * Y]; }
            set {
                Values[y - 1 + (x - 1) * Y] = value;
                OnChange?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Equality operator overload.
        /// Equality should be based on the values of the matrices
        /// </summary>
        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y)
                return false;
            for (int x = 1; x <= a.X; x++)
            {
                for (int y = 1; y <= a.Y; y++)
                {
                    if (a[x, y] != b[x, y])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Runs the regular equality operator and inverts it
        /// Repeated code as it is optimised for non-equal matrices.
        /// </summary>
        public static bool operator !=(Matrix a, Matrix b)
        {
            if (a.X != b.X || a.Y != b.Y)
                return true;
            for (int x = 1; x <= a.X; x++)
            {
                for (int y = 1; y <= a.Y; y++)
                {
                    if (a[x, y] != b[x, y])
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Override for the equals object method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix)
                return this == (Matrix)obj;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Power operator on matrices
        /// </summary>
        public static Matrix operator ^(Matrix matrix, int pow)
        {
            //Invert the matrix m^-1 = inverse in common notation
            /*if (pow == -1)
            {
                return matrix.GetInverse();
            }
            else */
            if (pow < 0)
            {
                //I don't really see any valid use for this and it would get confusing.
                throw new NotImplementedException();
            }
            //Not done yet
            throw new NotImplementedException();
        }

        /// <summary>
        /// Multiplication between 2 matrices.
        /// </summary>
        public static Matrix operator *(Matrix a, Matrix b)
        {
            //Check to make sure the matrices can be multiplied together
            if (a.X != b.Y) throw new InvalidMatrixDimensionError($"Invalid matrix dimensions: a.X ({a.X}) != b.Y ({b.Y})");
            int dotProductLength = a.X;
            //Create the new output matrix
            Matrix outputMatrix = new Matrix(b.X, a.Y);

            //For each column in the output matrix
            for (int x = 1; x <= b.X; x++)
            {
                //And each row in the output matrix
                for (int y = 1; y <= a.Y; y++)
                {
                    float dotProductSum = 0;
                    //The result is the dot product of the rows
                    for (int i = 1; i <= dotProductLength; i++)
                    {
                        dotProductSum += a[i, y] * b[x, i];
                    }
                    //Calculate the result
                    outputMatrix[x, y] = dotProductSum;
                }
            }
            //Return the result
            return outputMatrix;
        }

        /// <summary>
        /// Returns the appropriate translation matrix
        /// </summary>
        public static Matrix GetTranslationMatrix(float x, float y, float z)
        {
            return new Matrix(new float[4, 4]{
                { 1, 0, 0, x },
                { 0, 1, 0, y },
                { 0, 0, 1, z },
                { 0, 0, 0, 1 },
            });
        }

        /// <summary>
        /// Returns the appropriate rotation matrix.
        /// Values are in radians.
        /// </summary>
        public static Matrix GetRotationMatrix(float xRot, float yRot, float zRot)
        {
            //Calculate cos and sin of x, y and z
            float cosX = (float)Math.Cos(xRot);
            float cosY = (float)Math.Cos(yRot);
            float cosZ = (float)Math.Cos(zRot);
            float sinX = (float)Math.Sin(xRot);
            float sinY = (float)Math.Sin(yRot);
            float sinZ = (float)Math.Sin(zRot);
            //Calculate the output matrix
            return new Matrix(new float[,]
            {
                { cosY * cosZ,                      -cosY * sinZ,                       -sinY,          0 },
                { sinX * sinY * cosZ + cosX * sinZ, -sinX * sinY * sinZ + cosX * cosZ,  sinX * cosY,    0 },
                { sinY * cosZ + sinX * sinZ,        -sinY * sinZ - sinX * cosZ,         cosX * cosY,    0 },
                { 0,                                0,                                  0,              1},
            });
        }

        /// <summary>
        /// Returns a matrix that represents the scale transformation
        /// </summary>
        public static Matrix GetScaleMatrix(float xScale, float yScale, float zScale)
        {
            return new Matrix(new float[4, 4] {
                { xScale, 0, 0, 0 },
                { 0, yScale, 0, 0 },
                { 0, 0, zScale, 0 },
                { 0, 0, 0, 1 }
            });
        }

        /// <summary>
        /// Returns a transformation that represents a perspective matrix with provided FOV, aspect ratio, near distance and far distance.
        /// FOV is in degrees.
        /// </summary>
        public static Matrix GetPerspectiveMatrix(float fov, float nearDistance, float farDistance)
        {
            float scale = 1f / (float)Math.Tan(fov * Math.PI / 360f);
            //Flips X and Z
            /*return new Matrix(new float[4, 4]
            {
                { 0,                                            0,      scale,  0 },
                { 0,                                            scale,  0,      0 },
                { -farDistance / (farDistance - nearDistance),  0,      0,      -farDistance * nearDistance / (farDistance - nearDistance) },
                { -1,                                           0,      0,      0 },
            });*/
            //ORIGINAL TRANSPOSED
            return new Matrix(new float[4, 4]
            {
                { scale,    0,      0,                                                          0 },
                { 0,        scale,  0,                                                          0 },
                { 0,        0,      -farDistance / (farDistance - nearDistance),                -farDistance * nearDistance / (farDistance - nearDistance) },
                { 0,        0,      -1,                                                         0 },
            });
            //ORIGINAL
            /*return new Matrix(new float[4, 4]
            {
                { scale,    0,      0,                                                          0 },
                { 0,        scale,  0,                                                          0 },
                { 0,        0,      -farDistance / (farDistance - nearDistance),                -1 },
                { 0,        0,      -farDistance * nearDistance / (farDistance - nearDistance),                                                         0 },
            });*/
            /*
            // THIS WORKS BUT Z AXIS IS INVERTED FOR SOME REASON
            return new Matrix(new float[4, 4] {
                { 2 * nearDistance / (right - left), 0, (right + left) / (right - left), 0 },
                { 0, 2 * nearDistance / (top - bottom), 0, 0 },
                { 0, (top + bottom) / (top - bottom), -(farDistance + nearDistance) / (farDistance - nearDistance), -2 * farDistance * nearDistance / (farDistance - nearDistance) },
                { 0, 0, -1, 0 }
            });
            */
        }

        public override string ToString()
        {
            string output = "{";
            for (int y = 1; y <= Y; y++)
            {
                output += "{";
                for (int x = 1; x <= X; x++)
                {
                    output += $"{this[x, y]},";
                }
                output += "},";
            }
            output += "}";
            return output;
        }

        /// <summary>
        /// Returns the inverse of this matrix
        /// </summary>
        /*public Matrix GetInverse()
        {
            int outputDimension = Y;
            //General theory:
            //IN THE THEORY ARRAY INDEXING STARTS AT 1
            //Let K = width of A
            //I[i,j] = SUM(n = 1 -> K){A[n,j]*(A^-1)[i,n]}
            //I[i,j] = {0: i!=j, 1: i==j}
            //If we set i as a value and go through all values of j, we get a set of simultaneous equations
            //which map to A^-1[i,j]
            //END THEORY

            //1 <= i <= Width
            for (int i = 1; i <= outputDimension; i++)
            {
                //Calculate the set of simultanous equations
                
            }
        }*/

    }
}
