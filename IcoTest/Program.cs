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

            //Index testing
            Console.WriteLine("===INDEX TESTING===");
            IcoCollection<double> icoc = new IcoCollection<double>(icoLevel);
            for (int j = 0; j < icoc.Length; j++)
            {
                icoc[j] = j;
            }
            int k = 0;
            foreach (double d in icoc)
            {
                Console.WriteLine(k++ + " : " + d);
            }

            Console.WriteLine("===NEIGHBOUR TESTING===");
            int[] neighbours = IcoCommon.GetNeighbours(icoLevel);
            for (int i = 0; i < icos.verticies.Length; i++)
            {
                Vector3d thing = icos.verticies[i];
                Console.WriteLine("Index " + i + " : " + thing);

                for (int neighbour = i * 6; neighbour < (i * 6 + 6); neighbour++)
                {
                    Console.WriteLine(neighbours[neighbour]);
                }
                Console.WriteLine("===");
            }

            //Neighbour testing
            for (int i = 0; i < icoc.Length; i++)
            {
                Console.WriteLine("Index " + i);
                foreach (double d in icoc.GetNeighbours(i))
                {
                    Console.WriteLine(d);
                }
                Console.WriteLine("===");
            }
        }
    }
}
