using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LuaDebugger
{
    public partial class LuaConsole : UserControl
    {
        protected LuaState ls;
        protected List<string> History = new List<string>() { "" };
        protected int historyPos = 0;

        public LuaConsole()
        {
            InitializeComponent();
        }

        public void InitState(LuaState ls)
        {
            this.ls = ls;
        }

        private void ClearConsole()
        {
            rtbOutput.Text = "--< Lua Console >--";
            rtbOutput.SelectAll();
            rtbOutput.SelectionAlignment = HorizontalAlignment.Center;
            rtbOutput.AppendText("\n");
            rtbOutput.Select(rtbOutput.Text.Length, 0);
            rtbOutput.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void LuaConsole_Load(object sender, EventArgs e)
        {
            ClearConsole();
            tbInput.Text = "";

            Color bgCol = rtbOutput.BackColor;
            rtbOutput.ReadOnly = true;
            rtbOutput.BackColor = bgCol;
        }

        public void RunCommand(string cmd)
        {
            int nextHistory = this.History.Count - 1;
            this.History[nextHistory] = cmd;
            this.History.Add("");
            if (this.historyPos == nextHistory)
                this.historyPos++;

            rtbOutput.AppendText("\n> " + cmd);
            string answer = ls.EvaluateLua(cmd);
            if (answer != "")
                rtbOutput.AppendText("\n" + answer);
            rtbOutput.ScrollToCaret();
        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string cmd = tbInput.Text;
                tbInput.Text = "";

                RunCommand(cmd);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                if (this.historyPos == this.History.Count - 1)
                {
                    this.History[this.historyPos] = tbInput.Text;
                }

                if (this.historyPos != 0)
                {
                    this.historyPos--;
                    tbInput.Text = this.History[this.historyPos];
                    tbInput.Select(tbInput.Text.Length, 0);
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                if (this.historyPos != this.History.Count - 1)
                {
                    this.historyPos++;
                    tbInput.Text = this.History[this.historyPos];
                    tbInput.Select(tbInput.Text.Length, 0);
                }
            }
        }

        private void rtbOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= ' ' && e.KeyChar <= '}')
            {
                tbInput.Text += e.KeyChar;
                JumpToInput();
                e.Handled = true;
            }
        }

        private void rtbOutput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                JumpToInput();
        }

        protected void JumpToInput()
        {
            tbInput.Focus();
            tbInput.Select();
            tbInput.Select(tbInput.Text.Length, 0);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearConsole();
        }
    }
}
