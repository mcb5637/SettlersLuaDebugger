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
        public ErrorView()
        {
            InitializeComponent();
        }

        private void ErrorView_Load(object sender, EventArgs e)
        {

        }

        public string ErrorMessage
        {
            set
            {
                this.tbErrorMessage.Text = value;
            }
        }
    }
}
