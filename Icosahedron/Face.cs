using System;

namespace Icosahedron
{
    public struct Face
    {
        public int point1;
        public int point2;
        public int point3;

        public Face(int point1, int point2, int point3)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
        }
    }
}

