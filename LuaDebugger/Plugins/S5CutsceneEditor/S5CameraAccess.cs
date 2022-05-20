using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public class S5CameraInfo
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct CameraInfo
        {
            public float LookAtX, LookAtY, LookAtZ;
            public float Distance;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct CRwCameraHandler {
            // public int vtIObject // pointer points to icamerahandle, so we just ignore this one
	        public int vtICameraHandle, vtICameraMovement, vtICameraSettings;

            public bool Dirty; // 4
            public int UpdateZMode; // >= 0 && < 4
            public bool bScrolling;
            public float FOV;
            public CameraInfo CameraInfo; // 8
            public float HorizontalAngle; // 12
            public float VerticalAngle;
            // there is more, but we dont need it
        }

        private unsafe CRwCameraHandler* GetPointer()
        {
            return *(CRwCameraHandler**)(0x87EC68);
        }

        public float PosX
        {
            get
            {
                unsafe {
                    return GetPointer()->CameraInfo.LookAtX;
                }
            }
            set
            {
                unsafe
                {
                    GetPointer()->CameraInfo.LookAtX = value;
                }
            }
        }
        public float PosY
        {
            get
            {
                unsafe
                {
                    return GetPointer()->CameraInfo.LookAtY;
                }
            }
            set
            {
                unsafe
                {
                    GetPointer()->CameraInfo.LookAtY = value;
                }
            }
        }
        public float PosZ
        {
            get
            {
                unsafe
                {
                    return GetPointer()->CameraInfo.LookAtZ;
                }
            }
            set
            {
                unsafe
                {
                    GetPointer()->CameraInfo.LookAtZ = value;
                }
            }
        }

        public float Distance
        {
            get
            {
                unsafe
                {
                    return GetPointer()->CameraInfo.Distance;
                }
            }
            set
            {
                unsafe
                {
                    GetPointer()->CameraInfo.Distance = value;
                }
            }
        } //should be 0

        public float YawAngle
        {
            get
            {
                unsafe
                {
                    return GetPointer()->HorizontalAngle;
                }
            }
            set
            {
                unsafe
                {
                    GetPointer()->HorizontalAngle = value;
                }
            }
        }
        public float PitchAngle
        {
            get
            {
                unsafe
                {
                    return GetPointer()->VerticalAngle;
                }
            }
            set
            {
                unsafe
                {
                    GetPointer()->VerticalAngle = value;
                }
            }
        }


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
    }
}
