using System;

namespace Icosahedron
{
    public static class IcoCommon
    {
        public static Face[] GetInitialFaces()
        {
            Face[] returnFaces = new Face[20];
            //Point 0
            returnFaces[0] = new Face(0, 3, 1);
            returnFaces[1] = new Face(0, 5, 3);
            returnFaces[2] = new Face(0, 7, 5);
            returnFaces[3] = new Face(0, 9, 7);
            returnFaces[4] = new Face(0, 1, 9);

            //Middle
            returnFaces[5] = new Face(1, 3, 2);
            returnFaces[6] = new Face(2, 3, 4);
            returnFaces[7] = new Face(3, 5, 4);
            returnFaces[8] = new Face(4, 5, 6);
            returnFaces[9] = new Face(5, 7, 6);
            returnFaces[10] = new Face(6, 7, 8);
            returnFaces[11] = new Face(7, 9, 8);
            returnFaces[12] = new Face(8, 9, 10);
            returnFaces[13] = new Face(9, 1, 10);
            returnFaces[14] = new Face(10, 1, 2);

            //Point 11
            returnFaces[15] = new Face(11, 2, 4);
            returnFaces[16] = new Face(11, 4, 6);
            returnFaces[17] = new Face(11, 6, 8);
            returnFaces[18] = new Face(11, 8, 10);
            returnFaces[19] = new Face(11, 10, 2);

            return returnFaces;
        }

        public static ulong GetCacheID(int left, int right)
        {
            ulong shifted;
            if (left < right)
            {
                shifted = (ulong)left;
                shifted = (shifted << 32);
                return shifted | (uint)right;
            }
            shifted = (ulong)right;
            shifted = (shifted << 32);
            return shifted | (uint)left;
        }

        public static int[] CalculateNeighbours(Face[] inputFaces)
        {
            int maxVertex = 0;
            foreach (Face f in inputFaces)
            {
                if (f.point1 > maxVertex)
                {
                    maxVertex = f.point1;
                }
                if (f.point2 > maxVertex)
                {
                    maxVertex = f.point2;
                }
                if (f.point3 > maxVertex)
                {
                    maxVertex = f.point3;
                }
            }
            int[] neighbours = new int[(maxVertex + 1) * 6];
            int[] storePos = new int[maxVertex + 1];
            for (int i = 0; i < neighbours.Length; i++)
            {
                neighbours[i] = -1;
            }
            for (int i = 0; i < inputFaces.Length; i++)
            {
                Face currentFace = inputFaces[i];
                LinkPoints(neighbours, storePos, currentFace.point1, currentFace.point2);
                LinkPoints(neighbours, storePos, currentFace.point1, currentFace.point3);
                LinkPoints(neighbours, storePos, currentFace.point2, currentFace.point1);
                LinkPoints(neighbours, storePos, currentFace.point2, currentFace.point3);
                LinkPoints(neighbours, storePos, currentFace.point3, currentFace.point1);
                LinkPoints(neighbours, storePos, currentFace.point3, currentFace.point2);
            }
            return neighbours;
        }

        private static void LinkPoints(int[] neighbours, int[] storePos, int pointSrc, int pointDst)
        {
            bool pointLinked = false;
            for (int i = pointSrc * 6; i < pointSrc * 6 + 6; i++)
            {
                if (neighbours[i] == pointDst)
                {
                    pointLinked = true;
                    break;
                }
            }
            if (!pointLinked)
            {
                int storeIndex = storePos[pointSrc];
                neighbours[pointSrc * 6 + storeIndex] = pointDst;
                storePos[pointSrc] = storeIndex + 1;
            }
        }
    }
}

