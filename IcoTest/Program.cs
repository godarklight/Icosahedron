using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Icosahedron;

namespace IcoTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Test level
            int icoLevel = 8;
            int raycastLoops = 100000;
            bool neighbourTest = false;
            bool indexTest = false;

            //Info
            Console.WriteLine("===ICOSAHEDRON INFO===");
            for (int i = 0; i < 20; i++)
            {
                long vert = IcoCommon.VerticiesInLevel(i);
                long edge = IcoCommon.EdgesInLevel(i);
                long faces = IcoCommon.FacesInLevel(i);
                Console.WriteLine("Level: " + i + ", verticies: " + vert + ", edges: " + edge + ", faces: " + faces);
            }

            //Performance testing
            Console.WriteLine("===PREFORMANCE TESTING===");
            for (int i = 0; i <= icoLevel; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                IcoCommon.Precalculate(i);
                sw.Stop();
                Stopwatch sw2 = new Stopwatch();
                sw2.Start();
                IcoCommon.PrecalculateNeighboursVertex(i);
                sw2.Stop();
                Stopwatch sw3 = new Stopwatch();
                sw3.Start();
                IcoCommon.PrecalculateNeighboursFaces(i);
                sw3.Stop();
                Console.WriteLine("Precalculate " + i + ", verticies: " + IcoCommon.VerticiesInLevel(i) + ", faces: " + IcoCommon.FacesInLevel(i) + ", vertex/face time: " + sw.ElapsedMilliseconds + " ms, vertex neighbour time: " + sw2.ElapsedMilliseconds + " ms, face neighbour time: " + sw3.ElapsedMilliseconds + " ms.");
            }

            IcoSphere icos = new IcoSphere(icoLevel);
            Console.WriteLine("Icosphere level: " + icoLevel + ", verticies: " + icos.verticies.Length + ", faces: " + icos.faces.Length);

            //Index neighbour common
            IcoCollection<int> icoc = new IcoCollection<int>(icoLevel);
            for (int i = 0; i < icoc.Length; i++)
            {
                icoc[i] = i;
            }

            if (indexTest)
            {
                Console.WriteLine("===INDEX ENUMERATOR TESTING===");

                int i = 0;
                foreach (int j in icoc)
                {
                    if (i != j)
                    {
                        throw new Exception("Index 1 failed");
                    }
                    Console.WriteLine(i + " : " + j);
                    i++;
                }
            }

            if (neighbourTest)
            {
                Console.WriteLine("===NEIGHBOUR ENUMERATOR TESTING===");
                for (int i = 0; i < icoc.Length; i++)
                {
                    Console.WriteLine("Index " + i);
                    int[] neighbourIndexes = icoc.GetNeighboursIndex(i);
                    int count = 0;
                    foreach (int j in icoc.GetNeighbours(i))
                    {
                        int testj = icoc[neighbourIndexes[count]];
                        Console.WriteLine(j + " == " + testj);
                        if (j != testj)
                        {
                            throw new Exception("Neighbour 1 failed");
                        }
                        count++;
                    }
                    if (count != neighbourIndexes.Length)
                    {
                        throw new Exception("Neighbour 2 failed");
                    }
                    Console.WriteLine("---");
                }
            }

            //Raycast setup common
            Vector3d dirVector = new Vector3d(1, 1, 1).normalized;
            List<Vector3d> rawVertex = IcoCommon.GetVerticiesRaw(icoLevel);

            //Raycast vertex
            Console.WriteLine("===TREE RAYCAST VERTEX TESTING===");
            Console.WriteLine("Source Direction: " + dirVector);
            for (int i = 0; i <= icoLevel; i++)
            {
                int closestVertexID = 0;
                Stopwatch swRaycast = new Stopwatch();
                swRaycast.Start();
                for (int currentLoop = 0; currentLoop < raycastLoops; currentLoop++)
                {
                    closestVertexID = IcoCommon.RaycastVertex(dirVector, i, false);
                }
                swRaycast.Stop();
                Vector3d closestVertex = rawVertex[closestVertexID];
                double error = (closestVertex - dirVector).magnitude;
                double vertexDot = Vector3d.Dot(dirVector, closestVertex);
                Console.WriteLine("Level: " + i + ", Closest: " + closestVertexID + ", Direction: " + closestVertex + ", Dot: " + vertexDot + ", error: " + error + ", time: " + swRaycast.ElapsedMilliseconds + " ms.");
            }

            //Raycast vertex


            Console.WriteLine("===TREE RAYCAST FACE TESTING===");
            Console.WriteLine("Source Direction: " + dirVector);
            for (int i = 0; i <= icoLevel; i++)
            {

                int closestFaceID = 0;
                Stopwatch swRaycastFace = new Stopwatch();
                swRaycastFace.Start();
                for (int currentLoop = 0; currentLoop < raycastLoops; currentLoop++)
                {
                    closestFaceID = IcoCommon.RaycastFace(dirVector, i, false);
                }
                swRaycastFace.Stop();
                List<Face> rawFaces = IcoCommon.GetFacesRaw(i);
                Face closestFace = rawFaces[closestFaceID];
                Vector3d faceCentre = (rawVertex[closestFace.point1] + rawVertex[closestFace.point2] + rawVertex[closestFace.point3]).normalized;
                double error = (faceCentre - dirVector).magnitude;
                double centerDot = Vector3d.Dot(dirVector, faceCentre);
                Console.WriteLine("Level: " + i + ", Closest: " + closestFaceID + ", Vertex edges: [" + closestFace.point1 + "," + closestFace.point2 + "," + closestFace.point3 + "], Center: " + faceCentre + ", Dot: " + centerDot + ", error: " + error + ", time: " + swRaycastFace.ElapsedMilliseconds + " ms.");
            }
        }
    }
}
