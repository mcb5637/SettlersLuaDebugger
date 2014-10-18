using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LuaDebugger
{
    public partial class DbgMain : Form
    {
        protected StateView activeState;
        protected Dictionary<LuaState, StateView> state2View = new Dictionary<LuaState, StateView>();

        public DbgMain()
        {
            InitializeComponent();
            Rectangle screen = Screen.GetWorkingArea(this);
            this.Location = new Point(1040, 0);
            this.Size = new Size(screen.Width - 1050, screen.Height);
            GlobalState.DebuggerWindow = this;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;
                const int WS_EX_COMPOSITED = 0x02000000;

                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F5:
                    if (PRtoolStripMenuItem.Enabled)
                        this.activeState.PauseResume();
                    break;
                case Keys.F10:
                    if (stepLineToolStripMenuItem.Enabled)
                        this.activeState.StepLine();
                    break;
                case Keys.F11:
                    if (stepInToolStripMenuItem.Enabled)
                        this.activeState.StepIn();
                    break;
                case (Keys.Shift | Keys.F11):
                    if (stepOutToolStripMenuItem.Enabled)
                        this.activeState.StepOut();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void DbgMain_Load(object sender, EventArgs e)
        {
            this.Text += VersionHelper.GetVersion();
            this.menuStrip1.Renderer = new MyRenderer();
            this.tcbState.ComboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
            tcbState.ComboBox.MouseWheel += ComboBox_MouseWheel;
        }

        protected object cancelMouseWheel = null;
        void ComboBox_MouseWheel(object sender, MouseEventArgs e)
        {
            this.cancelMouseWheel = tcbState.ComboBox.SelectedItem;
        }

        void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchToState(this.tcbState.SelectedItem as LuaState);
        }

        bool IsSettlersFullscreen()
        {
            WindowStyle settlersWndStyle = (WindowStyle)WinAPI.GetWindowLong(GlobalState.settlersWindowHandle, WinAPI.GWL_STYLE);
            return ((settlersWndStyle & WindowStyle.WS_POPUP) != 0);
        }

        void stateViews_OnDebugStateChange(object sender, DebugStateChangedEventArgs e)
        {
            if (IsSettlersFullscreen())
                if (e.State == DebugState.Running)
                {
                    WinAPI.SetForegroundWindow(GlobalState.settlersWindowHandle);
                    WinAPI.ShowWindow(GlobalState.settlersWindowHandle, WindowShowStyle.Restore);
                }
                else
                    WinAPI.SetForegroundWindow(this.Handle);

            if (this.activeState != e.LuaState.StateView)
                SwitchToState(e.LuaState);
            else
                UpdateDebuggerButtons();
        }

        private void tmUpdateView_Tick(object sender, EventArgs e)
        {
            UpdateGUI();
        }

        public void UpdateGUI()
        {
            lock (GlobalState.GuiUpdateLock)
            {
                tmUpdateView.Enabled = false;

                if (GlobalState.UpdateStatesView)
                {
                    tcbState.ComboBox.Items.Clear();

                    foreach (LuaState ls in GlobalState.L2State.Values)
                    {
                        tcbState.ComboBox.Items.Add(ls);
                        if (ls.StateView == null)
                        {
                            ls.StateView = new StateView();
                            ls.StateView.InitState(ls);
                            ls.StateView.OnDebugStateChange += stateViews_OnDebugStateChange;
                            ls.StateView.Dock = DockStyle.Fill;
                        }
                    }
                    if (GlobalState.LuaStates.Count > 0)
                        SwitchToState(GlobalState.LuaStates[GlobalState.LuaStates.Count - 1]);
                    else
                        SwitchToState(null);
                    GlobalState.UpdateStatesView = false;
                }

                foreach (LuaState ls in GlobalState.L2State.Values)
                    ls.StateView.UpdateView();

                tmUpdateView.Enabled = true;
            }
        }

        private void SwitchToState(LuaState ls)
        {
            if (ls == null)
            {
                this.pnlMain.Controls.Clear();
                this.activeState = null;
                this.pnlMain.Controls.Add(this.lblNoStates);
                PRtoolStripMenuItem.Enabled = false;
            }
            else
            {
                if (this.activeState == ls.StateView)
                    return;

                this.pnlMain.Controls.Clear();
                this.pnlMain.Controls.Add(ls.StateView);
                this.tcbState.SelectedItem = ls;
                PRtoolStripMenuItem.Enabled = true;
                this.activeState = ls.StateView;
                UpdateDebuggerButtons();
            }
        }

        private void PRtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeState.PauseResume();
        }

        private void stepLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeState.StepLine();
        }

        private void stepInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeState.StepIn();
        }

        private void stepOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.activeState.StepOut();
        }

        protected void ShowResume()
        {
            PRtoolStripMenuItem.Text = "Resume";
            PRtoolStripMenuItem.Image = Properties.Resources.Run;
        }

        protected void ShowPause()
        {
            PRtoolStripMenuItem.Text = "Pause";
            PRtoolStripMenuItem.Image = Properties.Resources.Pause;
        }

        protected void UpdateDebuggerButtons()
        {
            if (this.activeState.CurrentState == DebugState.Running)
            {
                ShowPause();
                stepLineToolStripMenuItem.Enabled = false;
                stepInToolStripMenuItem.Enabled = false;
                stepOutToolStripMenuItem.Enabled = false;
            }
            else if (this.activeState.CurrentState == DebugState.Paused)
            {
                ShowResume();
                stepLineToolStripMenuItem.Enabled = true;
                stepInToolStripMenuItem.Enabled = true;
                stepOutToolStripMenuItem.Enabled = true;
            }
            else if (this.activeState.CurrentState == DebugState.CaughtError)
            {
                ShowResume();
                stepLineToolStripMenuItem.Enabled = false;
                stepInToolStripMenuItem.Enabled = false;
                stepOutToolStripMenuItem.Enabled = false;
            }
        }

        private void DbgMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //   e.Cancel = true;
        }

        private void toolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show((sender as ToolStripMenuItem).ToolTipText, this, PointToClient(MousePosition));
        }

        private void toolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(this);
        }

        private void tmrAlive_Tick(object sender, EventArgs e)
        {
            //watchdog, check whether the game has been closed but failed to kill the debugger thread
            if (!WinAPI.IsWindow(GlobalState.settlersWindowHandle))
                Environment.Exit(0);
        }
    }

    class MyRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected || e.Item.Enabled && e.Item.Tag != null)
                base.OnRenderMenuItemBackground(e);
        }
    }
}
