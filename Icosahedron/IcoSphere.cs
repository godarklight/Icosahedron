using System;
using System.Collections.Generic;

namespace Icosahedron
{
    public class IcoSphere
    {
        public Vector3d[] verticies;
        public Face[] faces;
        private int level;

        public IcoSphere(int level)
        {
            this.level = level;
            verticies = IcoCommon.GetVerticesCopy(level);
            faces = IcoCommon.GetFacesCopy(level);
        }

        public int Level
        {
            get
            {
                return level;
            }
        }

        public int Raycast(Vector3d direction)
        {
            return IcoCommon.RaycastVertex(direction, level, false);
        }

        public int RaycastNormalized(Vector3d direction)
        {
            return IcoCommon.RaycastVertex(direction, level, true);
        }
    }
}