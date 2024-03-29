﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace LuaDebugger
{
    public partial class LuaConsole : UserControl
    {
        protected LuaStateWrapper ls;
        protected List<string> History = new List<string>() { "" };
        protected int historyPos = 0;

        private string[] waitSpinner = new string[] { 
        /*"◢", "◣", "◤", "◥"*/
        "◐", "◓", "◑", "◒"
        /*"▲", "►", "▼", "◄"*/
        /*"◰", "◳", "◲", "◱"*/
        /*".", "o", "O", "o"*/
        /*"▏", "▎", "▍", "▋", "▊", "▉", "▉", "▊", "▋", "▍", "▎",*/
        /*"⠁", "⠂", "⠄", "⡀", "⢀", "⠠", "⠐", "⠈"*/
        /*"┤", "┘", "┴", "└", "├", "┌", "┬", "┐"*/
        /*"◴", "◷", "◶",  "◵"*/};

        public LuaConsole()
        {
            InitializeComponent();
            tbPrompt.Cursor = Cursors.Default;
            tbPrompt.GotFocus += TbPrompt_GotFocus;
        }

        private void TbPrompt_GotFocus(object sender, EventArgs e)
        {
            JumpToInput();
            tbInput.Select(0, 0);
        }

        public void InitState(LuaStateWrapper ls)
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

        public void RunCommand(string cmd, bool locals = false)
        {

            tbInput.ReadOnly = true;
            StartWait();

            ls.EvaluateLua(cmd, (answer, cmdex) =>
            {
                rtbOutput.Invoke((MethodInvoker)delegate
                {
                    int nextHistory = this.History.Count - 1;
                    this.History[nextHistory] = cmdex;
                    this.History.Add("");
                    this.historyPos = nextHistory + 1;

                    this.AppendText("> " + cmdex);
                    if (answer != "")
                        this.AppendText(answer);
                    rtbOutput.ScrollToCaret();
                    tbInput.ReadOnly = false;
                    EndWait();
                });
            }, locals);
        }

        public void AppendText(string text)
        {
            if (rtbOutput.InvokeRequired)
            {
                rtbOutput.Invoke((MethodInvoker)delegate { this.AppendText(text); });
                return;
            }

            if (text.Length > 50*1024) //50kB max
            {
                rtbOutput.AppendText(text.Replace("\b", ""));
            }
            else
            {
                string[] cutByLinks = text.Split(new char[] { '\b' });
                rtbOutput.SuspendUpdate();

                bool isLink = false;
                int cnt = 0;
                foreach (string str in cutByLinks)
                {
                    if (isLink)
                        rtbOutput.InsertLink(str);
                    else
                        rtbOutput.AppendText(str);
                    isLink = !isLink;
                    cnt++;
                    if (cnt > 10)
                    {
                        Application.DoEvents();
                        cnt = 0;
                    }
                }
                rtbOutput.ResumeUpdate();
            }

            rtbOutput.AppendText("\n");
            rtbOutput.ScrollToCaret();
        }

        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbInput.ReadOnly)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                string cmd = tbInput.Text;
                tbInput.Text = "";

                if (cmd != "")
                    RunCommand(cmd, CB_Locals.Visible && CB_Locals.Checked);
                else
                    this.AppendText(">");

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

        private void StartWait()
        {
            tbSpinner.Text = "⌛";
            tbSpinner.Visible = true;
            tmrWaitForSpinner.Enabled = true;
        }

        private void EndWait()
        {
            tmrWaitForSpinner.Enabled = false;
            tmrSpinner.Enabled = false;
            tbSpinner.Visible = false;
        }

        private int waitSpinnerState = 0;
        private void tmrSpinner_Tick(object sender, EventArgs e)
        {
            tbSpinner.Text = waitSpinner[waitSpinnerState];
            waitSpinnerState = (waitSpinnerState + 1) % waitSpinner.Length;
        }

        private void tmrWaitForSpinner_Tick(object sender, EventArgs e)
        {
            tmrWaitForSpinner.Enabled = false;
            waitSpinnerState = 1;
            tbSpinner.Text = waitSpinner[0];
            tmrSpinner.Enabled = true;
        }

        private void EnableCompositeDrawing()
        {
            uint exStyle = WinAPI.GetWindowLong(GlobalState.DebuggerWindow.Handle, WinAPI.GWL_EXSTYLE);
            exStyle |= (uint)ExtendedWindowStyles.WS_EX_COMPOSITED;
            WinAPI.SetWindowLong(GlobalState.DebuggerWindow.Handle, WinAPI.GWL_EXSTYLE, exStyle);
        }

        private void DisableCompositeDrawing()
        {
            uint exStyle = WinAPI.GetWindowLong(GlobalState.DebuggerWindow.Handle, WinAPI.GWL_EXSTYLE);
            exStyle &= ~(uint)ExtendedWindowStyles.WS_EX_COMPOSITED;
            WinAPI.SetWindowLong(GlobalState.DebuggerWindow.Handle, WinAPI.GWL_EXSTYLE, exStyle);
        }

        private void rtbOutput_Enter(object sender, EventArgs e)
        {
            DisableCompositeDrawing();
        }

        private void rtbOutput_Leave(object sender, EventArgs e)
        {
            EnableCompositeDrawing();
        }

        private void rtbOutput_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            string[] parts = e.LinkText.Split(new char[] { ':' });
            int line = int.Parse(parts[parts.Length - 1]);
            string path = string.Join(":", parts, 0, parts.Length - 1);

            EnableCompositeDrawing();

            if (this.ls.LoadedFiles.TryGetValue(path, out LuaFile fileObj))
                this.ls.StateView.SwitchToFile(fileObj, line);
            else
                this.ls.StateView.ShowSourceUnavailable();

            DisableCompositeDrawing();
        }

        private void rtbOutput_VScroll(object sender, EventArgs e)
        {
            DisableCompositeDrawing();
        }

        private void rtbOutput_MouseLeave(object sender, EventArgs e)
        {
            EnableCompositeDrawing();
        }
    }
}
