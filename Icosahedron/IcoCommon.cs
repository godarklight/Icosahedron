using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public static class IcoCommon
    {
        internal static Vector3d[] verticies;
        internal static Face[][] faces;
        internal static int[][] neighbours;
        private static int calculateLevel = -1;
        private static int calculateLevelNeighbours = -1;

        public static void Precalculate(int level)
        {
            while (calculateLevel < level)
            {
                calculateLevel++;
                if (calculateLevel == 0)
                {
                    verticies = new Vector3d[1];
                    faces = new Face[1][];
                    neighbours = new int[1][];
                    verticies = GetInitialVerticies();
                    faces[0] = GetInitialFaces();
                    CalculateNeighbours(0);
                }
                else
                {
                    Vector3d[] oldVerticies = verticies;
                    Face[][] oldFaces = faces;
                    verticies = new Vector3d[VerticiesInLevel(calculateLevel)];
                    faces = new Face[calculateLevel + 1][];
                    Array.Copy(oldVerticies, 0, verticies, 0, oldVerticies.Length);
                    Array.Copy(oldFaces, 0, faces, 0, oldFaces.Length);
                    Subdivide(calculateLevel, oldVerticies.Length);
                }
            }
        }

        public static void PrecalculateNeighbours(int level)
        {
            while (calculateLevelNeighbours < level)
            {
                Precalculate(level);
                int[][] oldNeighbours = neighbours;
                neighbours = new int[calculateLevel + 1][];
                for (int i = 0; i < oldNeighbours.Length; i++)
                {
                    neighbours[i] = oldNeighbours[i];
                }
                CalculateNeighbours(calculateLevel);
                calculateLevelNeighbours++;
            }
        }

        public static Vector3d[] GetVerticesCopy(int level)
        {
            Precalculate(level);
            Vector3d[] retArray = new Vector3d[VerticiesInLevel(level)];
            Array.Copy(verticies, 0, retArray, 0, retArray.Length);
            return retArray;
        }

        public static Face[] GetFacesCopy(int level)
        {
            Precalculate(level);
            Face[] retArray = new Face[faces[level].Length];
            Array.Copy(faces[level], 0, retArray, 0, retArray.Length);
            return retArray;
        }

        public static int[] GetNeighboursCopy(int level)
        {
            PrecalculateNeighbours(level);
            int[] retArray = new int[neighbours[level].Length];
            Array.Copy(neighbours[level], 0, retArray, 0, retArray.Length);
            return retArray;
        }

        public static int[] GetNeighbours(int level)
        {
            PrecalculateNeighbours(level);
            return neighbours[level];
        }

        public static int VerticiesInLevel(int level)
        {
            return 10 * (int)Math.Pow(4, level) + 2;
        }

        public static int FacesInLevel(int level)
        {
            return 20 * (int)Math.Pow(4, level);
        }

        private static void Subdivide(int level, int oldVertexCount)
        {
            //<Vector3d>(verticies[level - 1]);
            Face[] newFaces = new Face[FacesInLevel(level)];
            int newVertexPos = oldVertexCount;
            int newFacePos = 0;
            //Copy new vertex array
            Dictionary<ulong, int> meshCache = new Dictionary<ulong, int>();
            Face[] oldLevel = faces[level - 1];
            for (int faceIndex = 0; faceIndex < oldLevel.Length; faceIndex++)
            {
                Face face = oldLevel[faceIndex];
                ulong cacheID12;
                ulong cacheID13;
                ulong cacheID23;
                if (face.point1 < face.point2)
                {
                    cacheID12 = (ulong)face.point1 << 32 | (uint)face.point2;
                }
                else
                {
                    cacheID12 = (ulong)face.point2 << 32 | (uint)face.point1;
                }
                if (face.point1 < face.point3)
                {
                    cacheID13 = (ulong)face.point1 << 32 | (uint)face.point3;
                }
                else
                {
                    cacheID13 = (ulong)face.point3 << 32 | (uint)face.point1;
                }
                if (face.point2 < face.point3)
                {
                    cacheID23 = (ulong)face.point2 << 32 | (uint)face.point3;
                }
                else
                {
                    cacheID23 = (ulong)face.point3 << 32 | (uint)face.point2;
                }
                int newPoint1;
                int newPoint2;
                int newPoint3;
                //Add or select the vertex between point 12
                if (!meshCache.TryGetValue(cacheID12, out newPoint1))
                {
                    verticies[newVertexPos] = (verticies[face.point1] + verticies[face.point2]).normalized;
                    newPoint1 = newVertexPos;
                    newVertexPos++;
                    meshCache[cacheID12] = newPoint1;
                }

                //Add or select the vertex between point 13
                if (!meshCache.TryGetValue(cacheID13, out newPoint2))
                {
                    verticies[newVertexPos] = (verticies[face.point1] + verticies[face.point3]).normalized;
                    newPoint2 = newVertexPos;
                    newVertexPos++;
                    meshCache[cacheID13] = newPoint2;
                }

                //Add or select the vertex between point 23
                if (!meshCache.TryGetValue(cacheID23, out newPoint3))
                {
                    verticies[newVertexPos] = (verticies[face.point2] + verticies[face.point3]).normalized;
                    newPoint3 = newVertexPos;
                    newVertexPos++;
                    meshCache[cacheID23] = newPoint3;
                }
                //Add the faces
                newFaces[newFacePos] = new Face(face.point1, newPoint1, newPoint2);
                newFaces[newFacePos + 1] = new Face(face.point2, newPoint3, newPoint1);
                newFaces[newFacePos + 2] = new Face(face.point3, newPoint2, newPoint3);
                newFaces[newFacePos + 3] = new Face(newPoint3, newPoint2, newPoint1);
                newFacePos += 4;
            }
            faces[level] = newFaces;
        }

        private static Vector3d[] GetInitialVerticies()
        {
            //Definition:
            //2 points at the poles
            //10 points located around the equator 36 degrees apart, that alternate +/- atan(0.5) inclination
            Vector3d[] returnPoints = new Vector3d[12];
            //Poles
            returnPoints[0] = new Vector3d(0, 1, 0);
            returnPoints[11] = new Vector3d(0, -1, 0);
            //Points

            //Get equator inclination and z value offset
            double equatorInc = Math.PI / 2d - Math.Atan(0.5);
            double equatorOffset = Math.Cos(equatorInc);

            double SinInc = Math.Sin(equatorInc);
            double Cos36 = Math.Sin(equatorInc) * Math.Cos(Math.PI / 10d);
            double Cos72 = Math.Sin(equatorInc) * Math.Cos(Math.PI / 5d);
            double Sin36 = Math.Sin(equatorInc) * Math.Sin(Math.PI / 10d);
            double Sin72 = Math.Sin(equatorInc) * Math.Sin(Math.PI / 5d);
            //Build points
            returnPoints[1] = new Vector3d(SinInc, equatorOffset, 0);
            returnPoints[2] = new Vector3d(Cos72, -equatorOffset, Sin72);
            returnPoints[3] = new Vector3d(Sin36, equatorOffset, Cos36);
            returnPoints[4] = new Vector3d(-Sin36, -equatorOffset, Cos36);
            returnPoints[5] = new Vector3d(-Cos72, equatorOffset, Sin72);
            returnPoints[6] = new Vector3d(-SinInc, -equatorOffset, 0);
            returnPoints[7] = new Vector3d(-Cos72, equatorOffset, -Sin72);
            returnPoints[8] = new Vector3d(-Sin36, -equatorOffset, -Cos36);
            returnPoints[9] = new Vector3d(Sin36, equatorOffset, -Cos36);
            returnPoints[10] = new Vector3d(Cos72, -equatorOffset, -Sin72);
            return returnPoints;
        }

        private static Face[] GetInitialFaces()
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

        private static void CalculateNeighbours(int level)
        {
            int maxVertex = 0;
            Face[] currentFaces = faces[level];
            foreach (Face f in currentFaces)
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
            int[] newNeighbours = new int[(maxVertex + 1) * 6];
            int[] storePos = new int[maxVertex + 1];
            for (int i = 0; i < newNeighbours.Length; i++)
            {
                newNeighbours[i] = -1;
            }
            for (int i = 0; i < currentFaces.Length; i++)
            {
                Face currentFace = currentFaces[i];
                LinkPoints(newNeighbours, storePos, currentFace.point1, currentFace.point2);
                LinkPoints(newNeighbours, storePos, currentFace.point1, currentFace.point3);
                LinkPoints(newNeighbours, storePos, currentFace.point2, currentFace.point1);
                LinkPoints(newNeighbours, storePos, currentFace.point2, currentFace.point3);
                LinkPoints(newNeighbours, storePos, currentFace.point3, currentFace.point1);
                LinkPoints(newNeighbours, storePos, currentFace.point3, currentFace.point2);
            }
            neighbours[level] = newNeighbours;
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

