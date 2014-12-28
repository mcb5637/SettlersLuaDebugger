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
    public partial class Properties : Form
    {
        FlightPoint curFlightPoint;
        public Properties()
        {
            InitializeComponent();
        }

        public void ShowProperties(FlightPoint fp)
        {
            curFlightPoint = fp;
            nudSpeed.Value = (decimal)curFlightPoint.Speed;
            cbLookAtActive.Checked = curFlightPoint.LookAtPos.Active;
            cbCamPosActive.Checked = curFlightPoint.CamPos.Active;
            this.ShowDialog();
        }

        private void cbCamPosActive_CheckedChanged(object sender, EventArgs e)
        {
            curFlightPoint.CamPos.Active = cbCamPosActive.Checked;
        }

        private void cbLookAtActive_CheckedChanged(object sender, EventArgs e)
        {
            curFlightPoint.LookAtPos.Active = cbLookAtActive.Checked;
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            curFlightPoint.Speed = (float)nudSpeed.Value;
        }
    }
}
