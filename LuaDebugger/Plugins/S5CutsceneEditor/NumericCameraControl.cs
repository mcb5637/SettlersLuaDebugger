using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public partial class NumericCameraControl : Form
    {
        public S5CameraInfo cam;
        private bool setup = false;
        public LuaStateWrapper ls;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate float GetHeightDel(UIntPtr th, float x, float y); // marshalling virtual methods does not really work
        private readonly GetHeightDel GetHeight;

        public NumericCameraControl()
        {
            InitializeComponent();
            GetHeight = Marshal.GetDelegateForFunctionPointer<GetHeightDel>(new IntPtr(0x47A953));
        }

        public void updateGUI()
        {
            setup = true;
            numX.Value = (decimal)cam.Point3D.X;
            numY.Value = (decimal)cam.Point3D.Y;
            numZ.Value = (decimal)cam.Point3D.Z;
            numPitch.Value = (decimal)cam.PitchAngle;
            numYaw.Value = (decimal)cam.YawAngle;
            setup = false;
            updateHeight();
        }

#pragma warning disable CS0649 // never assigned
        private unsafe struct ED_CGlobalsLogicEx
        {
            public uint vt;
            public uint* GameLogic;
            public uint* CGLEEntitiesDisplay, CGLEEffectsDisplay, CGLETerrainHiRes, CGLETerrainLowRes;
            public uint* Blocking, CGLELandscape, CTerrainVertexColors, RegionInfo, CPlayerExplorationHandler;
            public ED_CLandscape* Landscape;
        }
        private unsafe struct ED_CLandscape
        {
            public uint vt;
        }
#pragma warning restore CS0649
        private void updateHeight()
        {
            float terrainHeight = -1;
            unsafe
            {
                ED_CGlobalsLogicEx* ed_globalslogicex = *((ED_CGlobalsLogicEx**)0x8581EC);
                ED_CLandscape* ed_landscape = ed_globalslogicex->Landscape;
                if (ed_landscape->vt == 0x76A404) // type check before calling the method
                    terrainHeight = GetHeight((UIntPtr)ed_landscape, (float)numX.Value, (float)numY.Value);
            }

            if (terrainHeight > -1)
            {
                lblTerrain.Text = terrainHeight.ToString();
                lblCamHeight.Text = (((float)numZ.Value) - terrainHeight).ToString();
            }
            else
            {
                lblTerrain.Text = "invalid pos/memory";
                lblCamHeight.Text = "invalid pos/memory";
            }
        }

        public void updateCam()
        {
            cam.Point3D = new Point3D((float)numX.Value, (float)numY.Value, (float)numZ.Value);
            cam.PitchAngle = (float)numPitch.Value;
            cam.YawAngle = (float)numYaw.Value;
            updateHeight();
        }

        private void num_ValueChanged(object sender, EventArgs e)
        {
            if (!setup)
            {
                updateCam();
            }
        }
    }
}
