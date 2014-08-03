using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    public partial class StackTraceView : UserControl
    {
        public event EventHandler<StackTraceClickedEventArgs> OnStackLevelClick;

        public StackTraceView()
        {
            InitializeComponent();
        }

        public void ShowStackTrace(LuaStackTrace st)
        {
            this.lvStackTrace.Items.Clear();
            for (int i = 0; i < st.Count; i++)
            {
                LuaFunctionInfo lfi = st[i];
                ListViewItem item = new ListViewItem(new string[] { lfi.FunctionName, lfi.Source, lfi.Line.ToString() });
                item.Tag = i;
                item.Selected = i == 0;
                this.lvStackTrace.Items.Add(item);
            }
        }

        private void lvStackTrace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.lvStackTrace.SelectedItems.Count > 0)
            {
                int selectedLevel = (int)this.lvStackTrace.SelectedItems[0].Tag;
                if (this.OnStackLevelClick != null)
                    this.OnStackLevelClick(this, new StackTraceClickedEventArgs(selectedLevel));
            }
        }

        private void lvStackTrace_Resize(object sender, EventArgs e)
        {
            int fullWidth = this.lvStackTrace.Width;
            int proportionalWidth = fullWidth - 100;
            this.lvStackTrace.Columns[0].Width = (int)(proportionalWidth * 0.6);
            this.lvStackTrace.Columns[1].Width = (int)(proportionalWidth * 0.4);
            this.lvStackTrace.Columns[2].Width = 50;
        }

        private void lvStackTrace_MouseEnter(object sender, EventArgs e)
        {
            (sender as ListView).Select();
        }
    }

    public class StackTraceClickedEventArgs : EventArgs
    {
        public int StackLevel{ get; protected set; }

        public StackTraceClickedEventArgs(int level)
        {
            this.StackLevel = level;
        }
    }
}
