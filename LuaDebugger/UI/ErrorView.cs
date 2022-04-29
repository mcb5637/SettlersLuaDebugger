using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    public partial class ErrorView : UserControl
    {
        protected LuaStateWrapper ls = null;

        public ErrorView()
        {
            InitializeComponent();
        }

        public void InitState(LuaStateWrapper ls)
        {
            this.ls = ls;
        }

        protected string errMsg;

        public string ErrorMessage
        {
            set
            {
                this.errMsg = value;
                this.tbErrorMessage.Text = value;
            }
        }

        public bool Visible2
        {
            set
            {
                if (chkErrorBreak.Checked)
                    base.Visible = value;
            }
        }

        private void chkErrorBreak_CheckedChanged(object sender, EventArgs e)
        {
            this.ls.DebugEngine.BreakOnError = chkErrorBreak.Checked;
            this.Visible = !chkErrorBreak.Checked || this.ls.CurrentState == DebugState.CaughtError;

            if (this.Visible)
            {
                if (chkErrorBreak.Checked)
                    this.tbErrorMessage.Text = errMsg;
                else
                    this.tbErrorMessage.Text = "Break on Error disabled!\r\nErrors will be printed in the console.";
            }
        }
    }
}
