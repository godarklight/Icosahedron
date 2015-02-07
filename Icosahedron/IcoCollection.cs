using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public class IcoCollection<T>
    {
        T[] values;
        private int points = 0;
        private int level;
        private int[] neighbours = null;

        public IcoCollection(int level)
        {
            this.level = level;
            points = IcoCommon.VerticiesInLevel(level);
            values = new T[points];
        }

        public int Length
        {
            get
            {
                return points;
            }
        }

        public int Level
        {
            get
            {
                return level;
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
            if (neighbours == null)
            {
                neighbours = IcoCommon.GetNeighbours(level);
            }
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
            if (neighbours == null)
            {
                neighbours = IcoCommon.GetNeighbours(level);
            }
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
    }

    public class IcoEnumerator<T> : IEnumerator<T>
    {
        T[] values;
        int pos = -1;

        public IcoEnumerator(T[] inputArray)
        {
            values = inputArray;
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