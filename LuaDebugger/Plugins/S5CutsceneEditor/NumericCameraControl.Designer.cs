namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    partial class NumericCameraControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.numZ = new System.Windows.Forms.NumericUpDown();
            this.numPitch = new System.Windows.Forms.NumericUpDown();
            this.numYaw = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTerrain = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCamHeight = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYaw)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Camera X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Camera Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Camera Z:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Camera Pitch:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Camera Yaw:";
            // 
            // numX
            // 
            this.numX.DecimalPlaces = 3;
            this.numX.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numX.Location = new System.Drawing.Point(138, 9);
            this.numX.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(120, 20);
            this.numX.TabIndex = 5;
            this.numX.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numY
            // 
            this.numY.DecimalPlaces = 3;
            this.numY.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numY.Location = new System.Drawing.Point(138, 35);
            this.numY.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(120, 20);
            this.numY.TabIndex = 6;
            this.numY.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numZ
            // 
            this.numZ.DecimalPlaces = 3;
            this.numZ.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numZ.Location = new System.Drawing.Point(138, 61);
            this.numZ.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this.numZ.Name = "numZ";
            this.numZ.Size = new System.Drawing.Size(120, 20);
            this.numZ.TabIndex = 7;
            this.numZ.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numPitch
            // 
            this.numPitch.DecimalPlaces = 3;
            this.numPitch.Location = new System.Drawing.Point(138, 87);
            this.numPitch.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numPitch.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numPitch.Name = "numPitch";
            this.numPitch.Size = new System.Drawing.Size(120, 20);
            this.numPitch.TabIndex = 8;
            this.numPitch.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // numYaw
            // 
            this.numYaw.DecimalPlaces = 3;
            this.numYaw.Location = new System.Drawing.Point(138, 113);
            this.numYaw.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numYaw.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numYaw.Name = "numYaw";
            this.numYaw.Size = new System.Drawing.Size(120, 20);
            this.numYaw.TabIndex = 9;
            this.numYaw.ValueChanged += new System.EventHandler(this.num_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Terrain Z:";
            // 
            // lblTerrain
            // 
            this.lblTerrain.AutoSize = true;
            this.lblTerrain.Location = new System.Drawing.Point(135, 144);
            this.lblTerrain.Name = "lblTerrain";
            this.lblTerrain.Size = new System.Drawing.Size(13, 13);
            this.lblTerrain.TabIndex = 11;
            this.lblTerrain.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(98, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Camera - Terrain Z:";
            // 
            // lblCamHeight
            // 
            this.lblCamHeight.AutoSize = true;
            this.lblCamHeight.Location = new System.Drawing.Point(135, 173);
            this.lblCamHeight.Name = "lblCamHeight";
            this.lblCamHeight.Size = new System.Drawing.Size(13, 13);
            this.lblCamHeight.TabIndex = 13;
            this.lblCamHeight.Text = "0";
            // 
            // NumericCameraControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 201);
            this.Controls.Add(this.lblCamHeight);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblTerrain);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numYaw);
            this.Controls.Add(this.numPitch);
            this.Controls.Add(this.numZ);
            this.Controls.Add(this.numY);
            this.Controls.Add(this.numX);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NumericCameraControl";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Camera Numbers";
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYaw)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.NumericUpDown numZ;
        private System.Windows.Forms.NumericUpDown numPitch;
        private System.Windows.Forms.NumericUpDown numYaw;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTerrain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCamHeight;
    }
}