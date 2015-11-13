namespace LuaDebugger
{
    partial class ErrorView
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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkErrorBreak = new System.Windows.Forms.CheckBox();
            this.tbErrorMessage = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(204)))), ((int)(((byte)(35)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "- Lua Error -";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.chkErrorBreak);
            this.panel1.Controls.Add(this.tbErrorMessage);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(332, 83);
            this.panel1.TabIndex = 1;
            // 
            // chkErrorBreak
            // 
            this.chkErrorBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkErrorBreak.AutoSize = true;
            this.chkErrorBreak.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(204)))), ((int)(((byte)(35)))));
            this.chkErrorBreak.Checked = true;
            this.chkErrorBreak.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkErrorBreak.Location = new System.Drawing.Point(235, 5);
            this.chkErrorBreak.Name = "chkErrorBreak";
            this.chkErrorBreak.Size = new System.Drawing.Size(94, 17);
            this.chkErrorBreak.TabIndex = 3;
            this.chkErrorBreak.Text = "Break on Error";
            this.chkErrorBreak.UseVisualStyleBackColor = false;
            this.chkErrorBreak.CheckedChanged += new System.EventHandler(this.chkErrorBreak_CheckedChanged);
            // 
            // tbErrorMessage
            // 
            this.tbErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbErrorMessage.BackColor = System.Drawing.Color.OldLace;
            this.tbErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbErrorMessage.CausesValidation = false;
            this.tbErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F);
            this.tbErrorMessage.Location = new System.Drawing.Point(0, 25);
            this.tbErrorMessage.Margin = new System.Windows.Forms.Padding(2);
            this.tbErrorMessage.Multiline = true;
            this.tbErrorMessage.Name = "tbErrorMessage";
            this.tbErrorMessage.ReadOnly = true;
            this.tbErrorMessage.Size = new System.Drawing.Size(332, 58);
            this.tbErrorMessage.TabIndex = 2;
            this.tbErrorMessage.Text = "in [string \"Data\\Script\\MapTools\\Comfort.lua\"]:255 attempt to index local \'_army\'" +
    " (a nil value)";
            // 
            // ErrorView
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ErrorView";
            this.Size = new System.Drawing.Size(338, 85);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbErrorMessage;
        private System.Windows.Forms.CheckBox chkErrorBreak;
    }
}
