using System;
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
            int icoLevel = 1;
            int raycastLoops = 10000;
            bool neighbourTest = true;
            bool indexTest = true;

            //Performance testing
            Console.WriteLine("===PREFORMANCE TESTING===");
            for (int i = 0; i < 8; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                IcoCommon.Precalculate(i);
                sw.Stop();
                Stopwatch sw2 = new Stopwatch();
                sw2.Start();
                IcoCommon.PrecalculateNeighbours(i);
                sw2.Stop();
                Console.WriteLine("Precalculate " + i + ", verticies: " + IcoCommon.VerticiesInLevel(i) + ", faces: " + IcoCommon.FacesInLevel(i) + ", vertex/face time: " + sw.ElapsedMilliseconds + " ms, neighbour time: " + sw2.ElapsedMilliseconds + " ms.");
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
            Vector3d[] copyVertex = IcoCommon.GetVerticesCopy(7);

            //Raycast
            Console.WriteLine("===LINEAR RAYCAST TESTING (Not library function)===");
            Console.WriteLine("Source Direction: " + dirVector);
            int closestVertexID = 0;
            Vector3d closestVertex = copyVertex[0];
            double closestDot = double.NegativeInfinity;
            Stopwatch swRaycast = new Stopwatch();
            swRaycast.Start();

            for (int i = 0; i < 8; i++)
            {
                swRaycast.Reset();
                swRaycast.Start();
                for (int currentLoop = 0; currentLoop < raycastLoops; currentLoop++)
                {

                    int thisLevel = IcoCommon.VerticiesInLevel(i);
                    for (int j = closestVertexID; j < thisLevel; j++)
                    {
                        Vector3d searchVertex = copyVertex[j];
                        double searchDot = Vector3d.Dot(dirVector, searchVertex);
                        if (searchDot > closestDot)
                        {
                            closestVertexID = j;
                            closestVertex = searchVertex;
                            closestDot = searchDot;
                        }
                    }
                }
                swRaycast.Stop();
                Console.WriteLine("Level: " + i + ", Closest: " + closestVertexID + ", Direction: " + closestVertex + ", Dot: " + closestDot + ", time: " + swRaycast.ElapsedMilliseconds + " ms.");
            }

            Console.WriteLine("===TREE RAYCAST TESTING===");
            Console.WriteLine("Source Direction: " + dirVector);
            for (int i = 0; i < 8; i++)
            {
                swRaycast.Reset();
                swRaycast.Start();
                for (int currentLoop = 0; currentLoop < raycastLoops; currentLoop++)
                {
                    closestVertexID = IcoCommon.Raycast(dirVector, i, false);
                    closestVertex = copyVertex[closestVertexID];
                    closestDot = Vector3d.Dot(dirVector, closestVertex);
                }
                swRaycast.Stop();
                Console.WriteLine("Level: " + i + ", Closest: " + closestVertexID + ", Direction: " + closestVertex + ", Dot: " + closestDot + ", time: " + swRaycast.ElapsedMilliseconds + " ms.");
            }
        }
    }
}
