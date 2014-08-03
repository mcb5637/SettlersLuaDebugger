namespace LuaDebugger
{
    partial class DbgMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DbgMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.PRtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.stepOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmLS = new System.Windows.Forms.ToolStripMenuItem();
            this.tcbState = new System.Windows.Forms.ToolStripComboBox();
            this.tmUpdateView = new System.Windows.Forms.Timer(this.components);
            this.lblNoStates = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PRtoolStripMenuItem,
            this.stepLineToolStripMenuItem,
            this.stepInToolStripMenuItem,
            this.toolStripMenuItem4,
            this.stepOutToolStripMenuItem,
            this.tsmLS,
            this.tcbState});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(624, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // PRtoolStripMenuItem
            // 
            this.PRtoolStripMenuItem.Enabled = false;
            this.PRtoolStripMenuItem.Image = global::LuaDebugger.Properties.Resources.Pause;
            this.PRtoolStripMenuItem.Name = "PRtoolStripMenuItem";
            this.PRtoolStripMenuItem.Size = new System.Drawing.Size(66, 23);
            this.PRtoolStripMenuItem.Tag = "ok";
            this.PRtoolStripMenuItem.Text = "Pause";
            this.PRtoolStripMenuItem.ToolTipText = "F5";
            this.PRtoolStripMenuItem.Click += new System.EventHandler(this.PRtoolStripMenuItem_Click);
            this.PRtoolStripMenuItem.MouseLeave += new System.EventHandler(this.toolStripMenuItem_MouseLeave);
            this.PRtoolStripMenuItem.MouseHover += new System.EventHandler(this.toolStripMenuItem_MouseHover);
            // 
            // stepLineToolStripMenuItem
            // 
            this.stepLineToolStripMenuItem.Enabled = false;
            this.stepLineToolStripMenuItem.Image = global::LuaDebugger.Properties.Resources.StepOver;
            this.stepLineToolStripMenuItem.Name = "stepLineToolStripMenuItem";
            this.stepLineToolStripMenuItem.Size = new System.Drawing.Size(83, 23);
            this.stepLineToolStripMenuItem.Tag = "ok";
            this.stepLineToolStripMenuItem.Text = "Step Line";
            this.stepLineToolStripMenuItem.ToolTipText = "F10";
            this.stepLineToolStripMenuItem.Click += new System.EventHandler(this.stepLineToolStripMenuItem_Click);
            this.stepLineToolStripMenuItem.MouseLeave += new System.EventHandler(this.toolStripMenuItem_MouseLeave);
            this.stepLineToolStripMenuItem.MouseHover += new System.EventHandler(this.toolStripMenuItem_MouseHover);
            // 
            // stepInToolStripMenuItem
            // 
            this.stepInToolStripMenuItem.Enabled = false;
            this.stepInToolStripMenuItem.Image = global::LuaDebugger.Properties.Resources.StepIn;
            this.stepInToolStripMenuItem.Name = "stepInToolStripMenuItem";
            this.stepInToolStripMenuItem.Size = new System.Drawing.Size(71, 23);
            this.stepInToolStripMenuItem.Tag = "ok";
            this.stepInToolStripMenuItem.Text = "Step In";
            this.stepInToolStripMenuItem.ToolTipText = "F11";
            this.stepInToolStripMenuItem.Click += new System.EventHandler(this.stepInToolStripMenuItem_Click);
            this.stepInToolStripMenuItem.MouseLeave += new System.EventHandler(this.toolStripMenuItem_MouseLeave);
            this.stepInToolStripMenuItem.MouseHover += new System.EventHandler(this.toolStripMenuItem_MouseHover);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(80, 23);
            this.toolStripMenuItem4.Text = "© yoq 2014";
            // 
            // stepOutToolStripMenuItem
            // 
            this.stepOutToolStripMenuItem.Enabled = false;
            this.stepOutToolStripMenuItem.Image = global::LuaDebugger.Properties.Resources.StepOut;
            this.stepOutToolStripMenuItem.Name = "stepOutToolStripMenuItem";
            this.stepOutToolStripMenuItem.Size = new System.Drawing.Size(81, 23);
            this.stepOutToolStripMenuItem.Tag = "ok";
            this.stepOutToolStripMenuItem.Text = "Step Out";
            this.stepOutToolStripMenuItem.ToolTipText = "Shift+F11";
            this.stepOutToolStripMenuItem.Click += new System.EventHandler(this.stepOutToolStripMenuItem_Click);
            this.stepOutToolStripMenuItem.MouseLeave += new System.EventHandler(this.toolStripMenuItem_MouseLeave);
            this.stepOutToolStripMenuItem.MouseHover += new System.EventHandler(this.toolStripMenuItem_MouseHover);
            // 
            // tsmLS
            // 
            this.tsmLS.Enabled = false;
            this.tsmLS.Name = "tsmLS";
            this.tsmLS.ShowShortcutKeys = false;
            this.tsmLS.Size = new System.Drawing.Size(76, 23);
            this.tsmLS.Text = "  Lua State:";
            // 
            // tcbState
            // 
            this.tcbState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tcbState.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tcbState.Items.AddRange(new object[] {
            "Lol",
            "Foo"});
            this.tcbState.Name = "tcbState";
            this.tcbState.Size = new System.Drawing.Size(121, 23);
            this.tcbState.Tag = "ok";
            // 
            // tmUpdateView
            // 
            this.tmUpdateView.Enabled = true;
            this.tmUpdateView.Tick += new System.EventHandler(this.tmUpdateView_Tick);
            // 
            // lblNoStates
            // 
            this.lblNoStates.AutoSize = true;
            this.lblNoStates.Location = new System.Drawing.Point(52, 47);
            this.lblNoStates.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNoStates.Name = "lblNoStates";
            this.lblNoStates.Size = new System.Drawing.Size(127, 13);
            this.lblNoStates.TabIndex = 5;
            this.lblNoStates.Text = "No Lua states available...";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.lblNoStates);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 27);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(624, 585);
            this.pnlMain.TabIndex = 6;
            // 
            // DbgMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 612);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DbgMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Lua Debugger v0.2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DbgMain_FormClosing);
            this.Load += new System.EventHandler(this.DbgMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem PRtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem stepOutToolStripMenuItem;
        private System.Windows.Forms.Timer tmUpdateView;
        private System.Windows.Forms.ToolStripMenuItem tsmLS;
        private System.Windows.Forms.ToolStripComboBox tcbState;
        private System.Windows.Forms.Label lblNoStates;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}