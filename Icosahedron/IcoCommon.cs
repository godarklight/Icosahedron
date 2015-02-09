using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public static class IcoCommon
    {
        private static List<Vector3d> verticies = new List<Vector3d>();
        private static List<List<Face>> faces = new List<List<Face>>();
        private static List<List<int>> neighbours = new List<List<int>>();
        private static int calculateLevel = -1;
        private static int calculateLevelNeighbours = -1;

        public static void Precalculate(int level)
        {
            while (calculateLevel < level)
            {
                calculateLevel++;
                if (calculateLevel == 0)
                {
                    verticies = GetInitialVerticies();
                    faces.Add(GetInitialFaces());
                }
                else
                {
                    Subdivide(calculateLevel);
                }
            }
        }

        public static void PrecalculateNeighbours(int level)
        {
            while (calculateLevelNeighbours < level)
            {
                Precalculate(level);
                CalculateNeighbours(calculateLevel);
                calculateLevelNeighbours++;
            }
        }

        public static Vector3d[] GetVerticesCopy(int level)
        {
            Precalculate(level);
            Vector3d[] retArray = new Vector3d[VerticiesInLevel(level)];
            for (int i = 0; i < retArray.Length; i++)
            {
                retArray[i] = verticies[i];
            }
            return retArray;
        }

        public static Face[] GetFacesCopy(int level)
        {
            Precalculate(level);
            return faces[level].ToArray();
        }

        public static int[] GetNeighboursCopy(int level)
        {
            PrecalculateNeighbours(level);
            return neighbours[level].ToArray();
        }

        public static List<Vector3d> GetVerticiesRaw(int level)
        {
            Precalculate(level);
            return verticies;
        }

        public static List<Face> GetFacesRaw(int level)
        {
            Precalculate(level);
            return faces[level];
        }

        public static List<int> GetNeighboursRaw(int level)
        {
            PrecalculateNeighbours(level);
            return neighbours[level];
        }

        public static int[] GetNeighboursIndex(int level, int index)
        {
            PrecalculateNeighbours(level);
            int[] returnIndex;
            if (neighbours[level][index * 6 + 5] == -1)
            {
                returnIndex = new int[5];
                for (int i = 0; i < 5; i++)
                {
                    returnIndex[i] = neighbours[level][index * 6 + i];
                }
            }
            else
            {
                returnIndex = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    returnIndex[i] = neighbours[level][index * 6 + i];
                }
            }
            return returnIndex;
        }

        public static long VerticiesInLevel(int level)
        {
            return 10 * ipow(4, level) + 2;
        }

        public static long FacesInLevel(int level)
        {
            return 20 * ipow(4, level);
        }

        public static long EdgesInLevel(int level)
        {
            return 30 * ipow(4, level);
        }

        private static long ipow(long baseVal, long exp)
        {
            long result = 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    result *= baseVal;
                }
                exp >>= 1;
                baseVal *= baseVal;
            }
            return result;
        }

        public static int Raycast(Vector3d direction, int level, bool normalize)
        {
            if (normalize)
            {
                direction = direction.normalized;
            }
            Precalculate(level);
            PrecalculateNeighbours(level);
            int currentVertexID = 0;
            Vector3d currentVertex = verticies[currentVertexID];
            double currentDot = Vector3d.Dot(direction, currentVertex);
            if (normalize)
            {
                currentVertex = currentVertex.normalized;
            }
            //Search level
            for (int searchLevel = 0; searchLevel <= level; searchLevel++)
            {
                //Walking state
                int newVertexID = currentVertexID;
                Vector3d newVertex = currentVertex;
                double newDot = currentDot;
                bool searchComplete = false;
                while (!searchComplete)
                {
                    for (int searchNeighbour = 0; searchNeighbour < 6; searchNeighbour++)
                    {
                        int neighbourID = neighbours[searchLevel][currentVertexID * 6 + searchNeighbour];
                        if (neighbourID == -1)
                        {
                            break;
                        }
                        if (neighbourID == currentVertexID)
                        {
                            continue;
                        }
                        //Compare Dot product, bigger == closer.
                        Vector3d neighbourVertex = verticies[neighbourID];
                        if (normalize)
                        {
                            neighbourVertex = neighbourVertex.normalized;
                        }
                        double neighbourDot = Vector3d.Dot(direction, neighbourVertex);
                        if (neighbourDot > newDot)
                        {
                            newVertex = neighbourVertex;
                            newVertexID = neighbourID;
                            newDot = neighbourDot;
                        }
                    }
                    //Set next search state
                    if (currentVertexID == newVertexID)
                    {
                        searchComplete = true;
                    }
                    else
                    {
                        currentVertex = newVertex;
                        currentVertexID = newVertexID;
                        currentDot = newDot;
                    }
                }
            }
            return currentVertexID;
        }

        private static void Subdivide(int level)
        {
            List<Face> newFaces = new List<Face>((int)FacesInLevel(level));
            Dictionary<CachePair, int> meshCache = new Dictionary<CachePair, int>((int)EdgesInLevel(level));
            List<Face> oldLevel = faces[level - 1];
            for (int faceIndex = 0; faceIndex < oldLevel.Count; faceIndex++)
            {
                Face face = oldLevel[faceIndex];
                Vector3d point1Vector = verticies[face.point1];
                Vector3d point2Vector = verticies[face.point2];
                Vector3d point3Vector = verticies[face.point3];
                CachePair cacheID12 = new CachePair(face.point1, face.point2);
                CachePair cacheID13 = new CachePair(face.point1, face.point3);
                CachePair cacheID23 = new CachePair(face.point2, face.point3);
                int newPoint1;
                int newPoint2;
                int newPoint3;
                //Add or select the vertex between point 12
                if (!meshCache.TryGetValue(cacheID12, out newPoint1))
                {
                    newPoint1 = verticies.Count;
                    verticies.Add((point1Vector + point2Vector).normalized);
                    meshCache.Add(cacheID12, newPoint1);
                }

                //Add or select the vertex between point 13
                if (!meshCache.TryGetValue(cacheID13, out newPoint2))
                {
                    newPoint2 = verticies.Count;
                    verticies.Add((point1Vector + point3Vector).normalized);
                    meshCache.Add(cacheID13, newPoint2);
                }

                //Add or select the vertex between point 23
                if (!meshCache.TryGetValue(cacheID23, out newPoint3))
                {
                    newPoint3 = verticies.Count;
                    verticies.Add((point2Vector + point3Vector).normalized);
                    meshCache.Add(cacheID23, newPoint3);
                }
                //Add the faces
                newFaces.Add(new Face(face.point1, newPoint1, newPoint2));
                newFaces.Add(new Face(face.point2, newPoint3, newPoint1));
                newFaces.Add(new Face(face.point3, newPoint2, newPoint3));
                newFaces.Add(new Face(newPoint3, newPoint2, newPoint1));
            }
            faces.Add(newFaces);
        }

        private static List<Vector3d> GetInitialVerticies()
        {
            //Definition:
            //2 points at the poles
            //10 points located around the equator 36 degrees apart, that alternate +/- atan(0.5) inclination
            List<Vector3d> returnPoints = new List<Vector3d>(12);

            //Get equator inclination and z value offset
            double equatorInc = Math.PI / 2d - Math.Atan(0.5);
            double equatorOffset = Math.Cos(equatorInc);

            double SinInc = Math.Sin(equatorInc);
            double Cos36 = Math.Sin(equatorInc) * Math.Cos(Math.PI / 10d);
            double Cos72 = Math.Sin(equatorInc) * Math.Cos(Math.PI / 5d);
            double Sin36 = Math.Sin(equatorInc) * Math.Sin(Math.PI / 10d);
            double Sin72 = Math.Sin(equatorInc) * Math.Sin(Math.PI / 5d);
            //Build points
            returnPoints.Add(new Vector3d(0, 1, 0));
            returnPoints.Add(new Vector3d(SinInc, equatorOffset, 0));
            returnPoints.Add(new Vector3d(Cos72, -equatorOffset, Sin72));
            returnPoints.Add(new Vector3d(Sin36, equatorOffset, Cos36));
            returnPoints.Add(new Vector3d(-Sin36, -equatorOffset, Cos36));
            returnPoints.Add(new Vector3d(-Cos72, equatorOffset, Sin72));
            returnPoints.Add(new Vector3d(-SinInc, -equatorOffset, 0));
            returnPoints.Add(new Vector3d(-Cos72, equatorOffset, -Sin72));
            returnPoints.Add(new Vector3d(-Sin36, -equatorOffset, -Cos36));
            returnPoints.Add(new Vector3d(Sin36, equatorOffset, -Cos36));
            returnPoints.Add(new Vector3d(Cos72, -equatorOffset, -Sin72));
            returnPoints.Add(new Vector3d(0, -1, 0));
            return returnPoints;
        }

        private static List<Face> GetInitialFaces()
        {
            List<Face> returnFaces = new List<Face>(20);
            //Point 0
            returnFaces.Add(new Face(0, 3, 1));
            returnFaces.Add(new Face(0, 5, 3));
            returnFaces.Add(new Face(0, 7, 5));
            returnFaces.Add(new Face(0, 9, 7));
            returnFaces.Add(new Face(0, 1, 9));

            //Middle
            returnFaces.Add(new Face(1, 3, 2));
            returnFaces.Add(new Face(2, 3, 4));
            returnFaces.Add(new Face(3, 5, 4));
            returnFaces.Add(new Face(4, 5, 6));
            returnFaces.Add(new Face(5, 7, 6));
            returnFaces.Add(new Face(6, 7, 8));
            returnFaces.Add(new Face(7, 9, 8));
            returnFaces.Add(new Face(8, 9, 10));
            returnFaces.Add(new Face(9, 1, 10));
            returnFaces.Add(new Face(10, 1, 2));

            //Point 11
            returnFaces.Add(new Face(11, 2, 4));
            returnFaces.Add(new Face(11, 4, 6));
            returnFaces.Add(new Face(11, 6, 8));
            returnFaces.Add(new Face(11, 8, 10));
            returnFaces.Add(new Face(11, 10, 2));

            return returnFaces;
        }

        private static void CalculateNeighbours(int level)
        {
            long vertInLevel = VerticiesInLevel(level);
            List<Face> currentFaces = faces[level];
            int[] newNeighbours = new int[vertInLevel * 6];
            int[] storePos = new int[vertInLevel];
            for (int i = 0; i < vertInLevel * 6; i++)
            {
                newNeighbours[i] = -1;
            }
            for (int i = 0; i < currentFaces.Count; i++)
            {
                Face currentFace = currentFaces[i];
                LinkPoints(newNeighbours, storePos, currentFace.point1, currentFace.point2);
                LinkPoints(newNeighbours, storePos, currentFace.point1, currentFace.point3);
                LinkPoints(newNeighbours, storePos, currentFace.point2, currentFace.point1);
                LinkPoints(newNeighbours, storePos, currentFace.point2, currentFace.point3);
                LinkPoints(newNeighbours, storePos, currentFace.point3, currentFace.point1);
                LinkPoints(newNeighbours, storePos, currentFace.point3, currentFace.point2);
            }
            neighbours.Add(new List<int>(newNeighbours));
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

        private struct CachePair
        {
            public readonly uint smallIndex;
            public readonly uint bigIndex;

            public CachePair(int index1, int index2)
            {
                if (index1 < index2)
                {
                    smallIndex = (uint)index1;
                    bigIndex = (uint)index2;
                }
                else
                {
                    smallIndex = (uint)index2;
                    bigIndex = (uint)index1;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj is CachePair)
                {
                    CachePair theirObject = (CachePair)obj;
                    return smallIndex == theirObject.smallIndex && bigIndex == theirObject.bigIndex;
                }
                return false;
            }

            public override int GetHashCode()
            {
                uint bitWrap = (bigIndex & 0xFFFF0000) >> 16 | (bigIndex & 0xFFFF) << 16;
                return (int)(smallIndex ^ bitWrap);
            }
        }
    }
}

