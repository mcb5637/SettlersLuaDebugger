namespace LuaDebugger
{
    partial class StateView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Map scripts");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Internal scripts");
            this.tvFiles = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.scUpper = new System.Windows.Forms.SplitContainer();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.pnlNoFileOpen = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.pnlNoSource = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.scSV = new System.Windows.Forms.SplitContainer();
            this.luaConsole = new LuaDebugger.LuaConsole();
            this.stackTraceView = new LuaDebugger.StackTraceView();
            this.errorView = new LuaDebugger.ErrorView();
            this.scUpper.Panel1.SuspendLayout();
            this.scUpper.Panel2.SuspendLayout();
            this.scUpper.SuspendLayout();
            this.pnlNoFileOpen.SuspendLayout();
            this.pnlNoSource.SuspendLayout();
            this.scSV.Panel1.SuspendLayout();
            this.scSV.Panel2.SuspendLayout();
            this.scSV.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvFiles
            // 
            this.tvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvFiles.FullRowSelect = true;
            this.tvFiles.HideSelection = false;
            this.tvFiles.Location = new System.Drawing.Point(3, 29);
            this.tvFiles.Margin = new System.Windows.Forms.Padding(2);
            this.tvFiles.Name = "tvFiles";
            treeNode1.Name = "MapScripts";
            treeNode1.Text = "Map scripts";
            treeNode2.Name = "Internal";
            treeNode2.Text = "Internal scripts";
            this.tvFiles.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.tvFiles.Size = new System.Drawing.Size(214, 196);
            this.tvFiles.TabIndex = 0;
            this.tvFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFiles_AfterSelect);
            this.tvFiles.MouseEnter += new System.EventHandler(this.tvFiles_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loaded Files:";
            // 
            // scUpper
            // 
            this.scUpper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scUpper.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scUpper.Location = new System.Drawing.Point(0, 0);
            this.scUpper.Margin = new System.Windows.Forms.Padding(2);
            this.scUpper.Name = "scUpper";
            // 
            // scUpper.Panel1
            // 
            this.scUpper.Panel1.Controls.Add(this.label1);
            this.scUpper.Panel1.Controls.Add(this.btnLoadFile);
            this.scUpper.Panel1.Controls.Add(this.tvFiles);
            this.scUpper.Panel1MinSize = 200;
            // 
            // scUpper.Panel2
            // 
            this.scUpper.Panel2.Controls.Add(this.pnlNoFileOpen);
            this.scUpper.Panel2.Controls.Add(this.pnlNoSource);
            this.scUpper.Size = new System.Drawing.Size(633, 225);
            this.scUpper.SplitterDistance = 220;
            this.scUpper.TabIndex = 3;
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadFile.Image = global::LuaDebugger.Properties.Resources.AddFile;
            this.btnLoadFile.Location = new System.Drawing.Point(193, 3);
            this.btnLoadFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(25, 25);
            this.btnLoadFile.TabIndex = 2;
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // pnlNoFileOpen
            // 
            this.pnlNoFileOpen.Controls.Add(this.label7);
            this.pnlNoFileOpen.Controls.Add(this.label8);
            this.pnlNoFileOpen.Controls.Add(this.label9);
            this.pnlNoFileOpen.Controls.Add(this.label10);
            this.pnlNoFileOpen.Location = new System.Drawing.Point(14, 15);
            this.pnlNoFileOpen.Margin = new System.Windows.Forms.Padding(2);
            this.pnlNoFileOpen.Name = "pnlNoFileOpen";
            this.pnlNoFileOpen.Size = new System.Drawing.Size(349, 175);
            this.pnlNoFileOpen.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(14, 8);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 20);
            this.label7.TabIndex = 2;
            this.label7.Text = "No file selected!";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 60);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(138, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "to make changes at runtime";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(30, 46);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(214, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "• No changes are possible, use the console";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(16, 28);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(237, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Click on a file on the left to view the source code";
            // 
            // pnlNoSource
            // 
            this.pnlNoSource.Controls.Add(this.label5);
            this.pnlNoSource.Controls.Add(this.label4);
            this.pnlNoSource.Controls.Add(this.label3);
            this.pnlNoSource.Controls.Add(this.label2);
            this.pnlNoSource.Location = new System.Drawing.Point(14, 15);
            this.pnlNoSource.Margin = new System.Windows.Forms.Padding(2);
            this.pnlNoSource.Name = "pnlNoSource";
            this.pnlNoSource.Size = new System.Drawing.Size(349, 175);
            this.pnlNoSource.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 60);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(171, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "• Code added via the Lua Console";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 46);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "• Internal Game Engine code";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "This happens when running:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Source code unavailable!";
            // 
            // scSV
            // 
            this.scSV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scSV.Location = new System.Drawing.Point(0, 0);
            this.scSV.Margin = new System.Windows.Forms.Padding(2);
            this.scSV.Name = "scSV";
            this.scSV.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scSV.Panel1
            // 
            this.scSV.Panel1.Controls.Add(this.scUpper);
            this.scSV.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // scSV.Panel2
            // 
            this.scSV.Panel2.Controls.Add(this.luaConsole);
            this.scSV.Panel2.Controls.Add(this.stackTraceView);
            this.scSV.Panel2.Controls.Add(this.errorView);
            this.scSV.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.scSV.Panel2.Resize += new System.EventHandler(this.scSV_Panel2_Resize);
            this.scSV.Size = new System.Drawing.Size(633, 577);
            this.scSV.SplitterDistance = 225;
            this.scSV.SplitterWidth = 3;
            this.scSV.TabIndex = 4;
            // 
            // luaConsole
            // 
            this.luaConsole.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.luaConsole.Dock = System.Windows.Forms.DockStyle.Top;
            this.luaConsole.Location = new System.Drawing.Point(0, 180);
            this.luaConsole.Margin = new System.Windows.Forms.Padding(2);
            this.luaConsole.Name = "luaConsole";
            this.luaConsole.Size = new System.Drawing.Size(633, 117);
            this.luaConsole.TabIndex = 2;
            this.luaConsole.LocationChanged += new System.EventHandler(this.luaConsole_LocationChanged);
            // 
            // stackTraceView
            // 
            this.stackTraceView.Dock = System.Windows.Forms.DockStyle.Top;
            this.stackTraceView.Location = new System.Drawing.Point(0, 77);
            this.stackTraceView.Margin = new System.Windows.Forms.Padding(2);
            this.stackTraceView.Name = "stackTraceView";
            this.stackTraceView.Size = new System.Drawing.Size(633, 103);
            this.stackTraceView.TabIndex = 1;
            this.stackTraceView.Visible = false;
            // 
            // errorView
            // 
            this.errorView.Dock = System.Windows.Forms.DockStyle.Top;
            this.errorView.Location = new System.Drawing.Point(0, 0);
            this.errorView.Margin = new System.Windows.Forms.Padding(2);
            this.errorView.Name = "errorView";
            this.errorView.Size = new System.Drawing.Size(633, 77);
            this.errorView.TabIndex = 0;
            this.errorView.Visible = false;
            // 
            // StateView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scSV);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StateView";
            this.Size = new System.Drawing.Size(633, 577);
            this.Load += new System.EventHandler(this.StateView_Load);
            this.scUpper.Panel1.ResumeLayout(false);
            this.scUpper.Panel1.PerformLayout();
            this.scUpper.Panel2.ResumeLayout(false);
            this.scUpper.ResumeLayout(false);
            this.pnlNoFileOpen.ResumeLayout(false);
            this.pnlNoFileOpen.PerformLayout();
            this.pnlNoSource.ResumeLayout(false);
            this.pnlNoSource.PerformLayout();
            this.scSV.Panel1.ResumeLayout(false);
            this.scSV.Panel2.ResumeLayout(false);
            this.scSV.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvFiles;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.SplitContainer scUpper;
        private System.Windows.Forms.SplitContainer scSV;
        private System.Windows.Forms.Panel pnlNoSource;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlNoFileOpen;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private ErrorView errorView;
        private LuaConsole luaConsole;
        private StackTraceView stackTraceView;
    }
}
