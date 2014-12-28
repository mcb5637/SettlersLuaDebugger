using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    [StructLayout(LayoutKind.Sequential)]
    public struct S5CameraInfo
    {
        public float PosX;
        public float PosY;
        public float PosZ;

        public float Distance; //should be 0

        public float YawAngle;
        public float PitchAngle;


        private static IntPtr CameraInfoPointer = IntPtr.Zero;

        public Point3D Point3D 
        {
            get
            {
                return new Point3D(this.PosX, this.PosY, this.PosZ);
            }
            set
            {
                this.PosX = value.X;
                this.PosY = value.Y;
                this.PosZ = value.Z;
            }
        }

        public static S5CameraInfo GetCurrentCamera()
        {
            if (CameraInfoPointer == IntPtr.Zero)
                CameraInfoPointer = new IntPtr(Marshal.ReadInt32(new IntPtr(0x87EC68)) + 0x1C);

            return (S5CameraInfo)Marshal.PtrToStructure(CameraInfoPointer, typeof(S5CameraInfo));
        }

        public unsafe void WriteToMemory()
        {
            Marshal.StructureToPtr(this, CameraInfoPointer, false);
        }
    }
}
