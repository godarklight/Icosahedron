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
            int icoLevel = 1;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IcoSphere icos = new IcoSphere(icoLevel);
            sw.Stop();
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            IcoCollection<int> icoc = new IcoCollection<int>(icoLevel);
            sw2.Stop();

            for (int i = 0; i < icos.mesh.verticies.Length; i++)
            {
                Vector3d thing = icos.mesh.verticies[i];
                Console.WriteLine(thing);
                for (int neighbour = i * 6; neighbour < (i * 6 + 6); neighbour++)
                {
                    Console.WriteLine(icos.neighbours[neighbour]);
                }
                Console.WriteLine("===");
            }
            

            Console.WriteLine("Icosphere level: " + icoLevel + ", verticies: " + icos.mesh.verticies.Length + ", faces: " + icos.mesh.triangles.Length + ", sphere time: " + sw.ElapsedMilliseconds + " ms.");
            Console.WriteLine("Icocollection level: " + icoLevel + ", sphere time: " + sw2.ElapsedMilliseconds + " ms.");

            for (int i = 0; i < 8; i++)
            {
                Stopwatch sw3 = new Stopwatch();
                sw3.Start();
                IcoSphere icoS = new IcoSphere(i);
                sw3.Stop();
                Console.WriteLine(i + " : " + icoS.mesh.verticies.Length + " : " + icoS.mesh.triangles.Length + ", time: " + sw3.ElapsedMilliseconds);
            }

            IcoCollection<double> icoc2 = new IcoCollection<double>(icoLevel);
            for (int j = 0; j < icoc2.Length; j++)
            {
                icoc2[j] = j;
            }
            int k = 0;
            foreach (double d in icoc2)
            {
                Console.WriteLine(k++ + " : " + d);
            }
        }
    }
}
