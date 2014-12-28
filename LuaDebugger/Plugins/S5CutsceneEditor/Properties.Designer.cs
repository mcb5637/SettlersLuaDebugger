namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    partial class Properties
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
            this.cbCamPosActive = new System.Windows.Forms.CheckBox();
            this.cbLookAtActive = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudSpeed = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // cbCamPosActive
            // 
            this.cbCamPosActive.AutoSize = true;
            this.cbCamPosActive.Checked = true;
            this.cbCamPosActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCamPosActive.Location = new System.Drawing.Point(24, 23);
            this.cbCamPosActive.Name = "cbCamPosActive";
            this.cbCamPosActive.Size = new System.Drawing.Size(135, 17);
            this.cbCamPosActive.TabIndex = 0;
            this.cbCamPosActive.Text = "Camera Position Active";
            this.cbCamPosActive.UseVisualStyleBackColor = true;
            this.cbCamPosActive.CheckedChanged += new System.EventHandler(this.cbCamPosActive_CheckedChanged);
            // 
            // cbLookAtActive
            // 
            this.cbLookAtActive.AutoSize = true;
            this.cbLookAtActive.Checked = true;
            this.cbLookAtActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbLookAtActive.Location = new System.Drawing.Point(24, 50);
            this.cbLookAtActive.Name = "cbLookAtActive";
            this.cbLookAtActive.Size = new System.Drawing.Size(133, 17);
            this.cbLookAtActive.TabIndex = 1;
            this.cbLookAtActive.Text = "LookAt Position Active";
            this.cbLookAtActive.UseVisualStyleBackColor = true;
            this.cbLookAtActive.CheckedChanged += new System.EventHandler(this.cbLookAtActive_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(171, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Speed:";
            // 
            // nudSpeed
            // 
            this.nudSpeed.Location = new System.Drawing.Point(174, 40);
            this.nudSpeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpeed.Name = "nudSpeed";
            this.nudSpeed.Size = new System.Drawing.Size(66, 20);
            this.nudSpeed.TabIndex = 4;
            this.nudSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpeed.ValueChanged += new System.EventHandler(this.nudSpeed_ValueChanged);
            // 
            // Properties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 90);
            this.Controls.Add(this.nudSpeed);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbLookAtActive);
            this.Controls.Add(this.cbCamPosActive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Properties";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Properties";
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbCamPosActive;
        private System.Windows.Forms.CheckBox cbLookAtActive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudSpeed;
    }
}