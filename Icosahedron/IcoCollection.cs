using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public class IcoCollection<T>
    {
        T[] values;
        private int points = 12;
        private List<Face> faces;
        private int[] neighbours;

        public IcoCollection(int subdivides)
        {
            faces = new List<Face>(IcoCommon.GetInitialFaces());
            for (int i = 0; i < subdivides; i++)
            {
                Subdivide();
            }
            values = new T[points];
            neighbours = IcoCommon.CalculateNeighbours(faces.ToArray());
            faces = null;
        }

        public int Length
        {
            get
            {
                return points;
            }
        }

        public T this [int i]
        {
            get
            {
                return values[i];
            }
            set
            {
                values[i] = value;
            }
        }

        public int[] GetNeighboursIndex(int index)
        {
            int[] returnIndex;
            if (neighbours[index + 6] == -1)
            {
                returnIndex = new int[5];
                Array.Copy(neighbours, index * 6, returnIndex, 0, 5);
            }
            else
            {
                returnIndex = new int[6];
                Array.Copy(neighbours, index * 6, returnIndex, 0, 6);
            }
            return returnIndex;
        }

        public IEnumerable<T> GetNeighbours(int index)
        {
            yield return values[neighbours[index * 6]];
            yield return values[neighbours[index * 6 + 1]];
            yield return values[neighbours[index * 6 + 2]];
            yield return values[neighbours[index * 6 + 3]];
            yield return values[neighbours[index * 6 + 4]];
            if (neighbours[index * 6 + 5] != -1)
            {
                yield return values[neighbours[index * 6 + 5]];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)new IcoEnumerator<T>(values);
        }

        private void Subdivide()
        {
            List<Face> newFaces = new List<Face>();
            Dictionary<ulong, int> meshCache = new Dictionary<ulong, int>();
            foreach (Face face in faces)
            {
                ulong cacheID1 = IcoCommon.GetCacheID(face.point1, face.point2);
                ulong cacheID2 = IcoCommon.GetCacheID(face.point1, face.point3);
                ulong cacheID3 = IcoCommon.GetCacheID(face.point2, face.point3);
                int newPoint1;
                int newPoint2;
                int newPoint3;
                //Add or select the vertex between point 12
                if (meshCache.ContainsKey(cacheID1))
                {
                    newPoint1 = meshCache[cacheID1];
                }
                else
                {
                    newPoint1 = points++;
                    meshCache[cacheID1] = newPoint1;
                }
                //Add or select the vertex between point 13
                if (meshCache.ContainsKey(cacheID2))
                {
                    newPoint2 = meshCache[cacheID2];
                }
                else
                {
                    newPoint2 = points++;
                    meshCache[cacheID2] = newPoint2;
                }
                //Add or select the vertex between point 23
                if (meshCache.ContainsKey(cacheID3))
                {
                    newPoint3 = meshCache[cacheID3];
                }
                else
                {
                    newPoint3 = points++;
                    meshCache[cacheID3] = newPoint3;
                }
                //Add the faces
                newFaces.Add(new Face(face.point1, newPoint2, newPoint1));
                newFaces.Add(new Face(face.point2, newPoint1, newPoint3));
                newFaces.Add(new Face(face.point3, newPoint3, newPoint2));
                newFaces.Add(new Face(newPoint3, newPoint1, newPoint2));
            }
            faces = newFaces;
        }
    }

    public class IcoEnumerator<T> : IEnumerator<T>
    {
        T[] values;
        int pos = -1;

        public IcoEnumerator(T[] inputArray)
        {
            values = inputArray;
            Console.WriteLine("Length: " + values.Length);
        }

        public T Current
        {
            get
            {
                return values[pos];
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get
            {
                return values[pos];
            }
        }

        public bool MoveNext()
        {
            if (pos < values.Length - 1)
            {
                pos++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            pos = -1;
        }

        public void Dispose()
        {
        }
    }
}

