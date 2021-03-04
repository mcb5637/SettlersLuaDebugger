using System;
using System.Collections.Generic;
using System.Text;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public struct Point3D
    {
        public float X, Y, Z;

        public Point3D(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Point3D MoveBy(float pitch, float yaw, float distance)
        {
            float dz = (float)-(distance * Math.Sin(pitch));
            float dxy = (float)(distance * Math.Cos(pitch));
            float dx = (float)-(dxy * Math.Sin(yaw));
            float dy = (float)(dxy * Math.Cos(yaw));

            return new Point3D(this.X + dx, this.Y + dy, this.Z + dz);
        }

        public Point3D SubtractAndScale(Point3D subWhat, float scale)
        {
            Point3D res;
            res.X = scale * (this.X - subWhat.X);
            res.Y = scale * (this.Y - subWhat.Y);
            res.Z = scale * (this.Z - subWhat.Z);
            return res;
        }

        public static Point3D operator +(Point3D a, Point3D b)
        {
            return new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Y);
        }

        public static Point3D operator -(Point3D a, Point3D b)
        {
            return new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Y);
        }


        public static Point3D operator -(Point3D a)
        {
            return new Point3D(-a.X, -a.Y, -a.Z);
        }

        public static Point3D operator /(Point3D a, float b)
        {
            return new Point3D(a.X / b, a.Y / b, a.Z / b);
        }

        public static Point3D operator *(Point3D a, float b)
        {
            return new Point3D(a.X * b, a.Y * b, a.Z * b);
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
    }
}
