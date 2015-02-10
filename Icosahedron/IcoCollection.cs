using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public class IcoCollection<T>
    {
        T[] values;
        private int points = 0;
        private int level;

        public IcoCollection(int level)
        {
            this.level = level;
            points = (int)IcoCommon.VerticiesInLevel(level);
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
            return IcoCommon.GetNeighboursVertexIndex(level, index);
        }

        public IEnumerable<T> GetNeighbours(int index)
        {
            yield return values[IcoCommon.GetNeighboursVertexRaw(level)[index * 6]];
            yield return values[IcoCommon.GetNeighboursVertexRaw(level)[index * 6 + 1]];
            yield return values[IcoCommon.GetNeighboursVertexRaw(level)[index * 6 + 2]];
            yield return values[IcoCommon.GetNeighboursVertexRaw(level)[index * 6 + 3]];
            yield return values[IcoCommon.GetNeighboursVertexRaw(level)[index * 6 + 4]];
            if (IcoCommon.GetNeighboursVertexRaw(level)[index * 6 + 5] != -1)
            {
                yield return values[IcoCommon.GetNeighboursVertexRaw(level)[index * 6 + 5]];
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