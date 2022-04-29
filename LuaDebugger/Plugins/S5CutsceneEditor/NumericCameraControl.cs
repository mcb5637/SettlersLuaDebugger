using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    public partial class NumericCameraControl : Form
    {
        public S5CameraInfo cam;
        private bool setup = false;
        public LuaStateWrapper ls;

        public NumericCameraControl()
        {
            InitializeComponent();
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

        private void updateHeight()
        {
            // TODO need better way to get this info, not this ugly hack
            String res = ls.EvaluateLua("S5Hook and (S5Hook.GetTerrainInfo(" + numX.Value + ", " + numY.Value + @"))");
            float terrainHeight = 0;
            if (float.TryParse(res, out terrainHeight)) {
                lblTerrain.Text = res;
                lblCamHeight.Text = (((float)numZ.Value) - terrainHeight).ToString();
            } else
            {
                lblTerrain.Text = "needs S5Hook";
                lblCamHeight.Text = "needs S5Hook";
            }
        }

        public void updateCam()
        {
            cam.Point3D = new Point3D((float)numX.Value, (float)numY.Value, (float)numZ.Value);
            cam.PitchAngle = (float)numPitch.Value;
            cam.YawAngle = (float)numYaw.Value;
            cam.WriteToMemory();
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
