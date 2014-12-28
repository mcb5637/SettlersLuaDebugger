namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    partial class JoyStick
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
            this.pbJoyStick = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbJoyStick)).BeginInit();
            this.SuspendLayout();
            // 
            // pbJoyStick
            // 
            this.pbJoyStick.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbJoyStick.Location = new System.Drawing.Point(0, 0);
            this.pbJoyStick.Name = "pbJoyStick";
            this.pbJoyStick.Size = new System.Drawing.Size(694, 367);
            this.pbJoyStick.TabIndex = 0;
            this.pbJoyStick.TabStop = false;
            this.pbJoyStick.Paint += new System.Windows.Forms.PaintEventHandler(this.pbJoyStick_Paint);
            this.pbJoyStick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbJoyStick_MouseDown);
            this.pbJoyStick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbJoyStick_MouseMove);
            this.pbJoyStick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbJoyStick_MouseUp);
            // 
            // JoyStick
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbJoyStick);
            this.Name = "JoyStick";
            this.Size = new System.Drawing.Size(694, 367);
            ((System.ComponentModel.ISupportInitialize)(this.pbJoyStick)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbJoyStick;
    }
}
