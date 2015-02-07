using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public class IcoSphere
    {
        public Vector3d[] verticies;
        public Face[] faces;

        public IcoSphere(int subdivides)
        {
            verticies = IcoCommon.GetVerticesCopy(subdivides);
            faces = IcoCommon.GetFacesCopy(subdivides);
        }
    }
}