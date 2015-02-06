using System;

namespace Icosahedron
{
    public struct Vector3d
    {
        public readonly double x;
        public readonly double y;
        public readonly double z;

        public Vector3d(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3d zero
        {
            get
            {
                return new Vector3d(0, 0, 0);
            }
        }

        public double magnitude
        {
            get
            {
                return (Math.Sqrt(Math.Pow(this.x, 2) + Math.Pow(this.y, 2) + Math.Pow(this.z, 2)));
            }
        }

        public Vector3d normalized
        {
            get
            {
                double length = this.magnitude;
                return new Vector3d(this.x / length, this.y / length, this.z / length);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3d)
            {
                Vector3d vectorObj = (Vector3d)obj;
                return this.x == vectorObj.x && this.y == vectorObj.y && this.z == vectorObj.z;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}]", this.x, this.y, this.z);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs)
        {
            return !lhs.Equals(rhs);
        }

        public static Vector3d operator +(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static Vector3d operator -(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static bool operator >(Vector3d lhs, Vector3d rhs)
        {
            //Don't need to square root as they cancel
            double lhsMag = lhs.x * lhs.x + lhs.y * lhs.y + lhs.z * lhs.z;
            double rhsMag = rhs.x * rhs.x + rhs.y * rhs.y + rhs.z * rhs.z;
            return (lhsMag > rhsMag);
        }

        public static bool operator <(Vector3d lhs, Vector3d rhs)
        {
            //Don't need to square root as they cancel
            double lhsMag = lhs.x * lhs.x + lhs.y * lhs.y + lhs.z * lhs.z;
            double rhsMag = rhs.x * rhs.x + rhs.y * rhs.y + rhs.z * rhs.z;
            return (lhsMag < rhsMag);
        }
    }
}

