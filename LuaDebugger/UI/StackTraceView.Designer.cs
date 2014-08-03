namespace LuaDebugger
{
    partial class StackTraceView
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
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "global FailFunc()",
            "MyMapFile.lua",
            "24"}, -1);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "global DoStuff3()",
            "MyMapFile.lua",
            "56"}, -1);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "field timerCallback()",
            "myfunctions.lua",
            "221"}, -1);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "global ProcessTimers()",
            "myfunctions.lua",
            "471"}, -1);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Game Engine / direct call");
            this.lvStackTrace = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lvStackTrace
            // 
            this.lvStackTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvStackTrace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvStackTrace.FullRowSelect = true;
            this.lvStackTrace.GridLines = true;
            this.lvStackTrace.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvStackTrace.HideSelection = false;
            this.lvStackTrace.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5});
            this.lvStackTrace.Location = new System.Drawing.Point(3, 0);
            this.lvStackTrace.Margin = new System.Windows.Forms.Padding(2);
            this.lvStackTrace.Name = "lvStackTrace";
            this.lvStackTrace.Size = new System.Drawing.Size(402, 100);
            this.lvStackTrace.TabIndex = 2;
            this.lvStackTrace.UseCompatibleStateImageBehavior = false;
            this.lvStackTrace.View = System.Windows.Forms.View.Details;
            this.lvStackTrace.SelectedIndexChanged += new System.EventHandler(this.lvStackTrace_SelectedIndexChanged);
            this.lvStackTrace.MouseEnter += new System.EventHandler(this.lvStackTrace_MouseEnter);
            this.lvStackTrace.Resize += new System.EventHandler(this.lvStackTrace_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Function in Call Stack";
            this.columnHeader1.Width = 323;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Source";
            this.columnHeader2.Width = 139;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Line";
            this.columnHeader3.Width = 46;
            // 
            // StackTraceView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvStackTrace);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StackTraceView";
            this.Size = new System.Drawing.Size(408, 102);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvStackTrace;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader2;


    }
}
