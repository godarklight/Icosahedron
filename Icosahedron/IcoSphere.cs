using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public class IcoSphere
    {
        public Mesh mesh;
        public int[] neighbours;

        public IcoSphere(int subdivides)
        {
            mesh = new Mesh();
            mesh.verticies = GenerateInitialIcosahedron();
            mesh.triangles = GetInitialFaces();
            Subdivide(mesh, subdivides);
            neighbours = IcoCommon.CalculateNeighbours(mesh.triangles);
        }

        private Vector3d[] GenerateInitialIcosahedron()
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

        private Face[] GetInitialFaces()
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

        private void Subdivide(Mesh subdivideMesh, int numberOfTimes)
        {
            for (int i = 0; i < numberOfTimes; i++)
            {
                Subdivide(subdivideMesh);
            }
        }

        private void Subdivide(Mesh subdivideMesh)
        {
            List<Vector3d> newVertices = new List<Vector3d>(subdivideMesh.verticies);
            List<Face> newFaces = new List<Face>();
            Dictionary<ulong, int> meshCache = new Dictionary<ulong, int>();
            foreach (Face face in subdivideMesh.triangles)
            {
                ulong cacheID1 = GetCacheID(face.point1, face.point2);
                ulong cacheID2 = GetCacheID(face.point1, face.point3);
                ulong cacheID3 = GetCacheID(face.point2, face.point3);
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
                    newVertices.Add((newVertices[face.point1] + newVertices[face.point2]).normalized);
                    newPoint1 = newVertices.Count - 1;
                    meshCache[cacheID1] = newPoint1;
                }
                //Add or select the vertex between point 13
                if (meshCache.ContainsKey(cacheID2))
                {
                    newPoint2 = meshCache[cacheID2];
                }
                else
                {
                    newVertices.Add((newVertices[face.point1] + newVertices[face.point3]).normalized);
                    newPoint2 = newVertices.Count - 1;
                    meshCache[cacheID2] = newPoint2;
                }
                //Add or select the vertex between point 23
                if (meshCache.ContainsKey(cacheID3))
                {
                    newPoint3 = meshCache[cacheID3];
                }
                else
                {
                    newVertices.Add((newVertices[face.point2] + newVertices[face.point3]).normalized);
                    newPoint3 = newVertices.Count - 1;
                    meshCache[cacheID3] = newPoint3;
                }
                //Add the faces
                newFaces.Add(new Face(face.point1, newPoint1, newPoint2));
                newFaces.Add(new Face(face.point2, newPoint3, newPoint1));
                newFaces.Add(new Face(face.point3, newPoint2, newPoint3));
                newFaces.Add(new Face(newPoint3, newPoint2, newPoint1));
            }
            subdivideMesh.verticies = newVertices.ToArray();
            subdivideMesh.triangles = newFaces.ToArray();
        }

        private ulong GetCacheID(int left, int right)
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


    }
}

