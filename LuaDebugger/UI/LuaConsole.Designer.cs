namespace LuaDebugger
{
    partial class LuaConsole
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
            this.components = new System.ComponentModel.Container();
            this.tbInput = new LuaDebugger.CopyChangedTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.CB_Locals = new System.Windows.Forms.CheckBox();
            this.tbSpinner = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tbPrompt = new System.Windows.Forms.TextBox();
            this.rtbOutput = new LuaDebugger.RichTextBoxLink();
            this.tmrSpinner = new System.Windows.Forms.Timer(this.components);
            this.tmrWaitForSpinner = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbInput
            // 
            this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInput.BackColor = System.Drawing.SystemColors.Info;
            this.tbInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbInput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbInput.Location = new System.Drawing.Point(11, 108);
            this.tbInput.Margin = new System.Windows.Forms.Padding(0);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(460, 16);
            this.tbInput.TabIndex = 1;
            this.tbInput.Text = "Abc = function(foo) Message(foo+gvLol); end";
            this.tbInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbInput_KeyDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CB_Locals);
            this.panel1.Controls.Add(this.tbSpinner);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.tbInput);
            this.panel1.Controls.Add(this.tbPrompt);
            this.panel1.Controls.Add(this.rtbOutput);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(471, 124);
            this.panel1.TabIndex = 2;
            // 
            // CB_Locals
            // 
            this.CB_Locals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CB_Locals.AutoSize = true;
            this.CB_Locals.BackColor = System.Drawing.SystemColors.Control;
            this.CB_Locals.Checked = true;
            this.CB_Locals.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_Locals.Location = new System.Drawing.Point(401, 26);
            this.CB_Locals.Name = "CB_Locals";
            this.CB_Locals.Size = new System.Drawing.Size(53, 17);
            this.CB_Locals.TabIndex = 5;
            this.CB_Locals.Text = "locals";
            this.CB_Locals.UseVisualStyleBackColor = false;
            // 
            // tbSpinner
            // 
            this.tbSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbSpinner.BackColor = System.Drawing.SystemColors.Info;
            this.tbSpinner.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSpinner.Location = new System.Drawing.Point(0, 109);
            this.tbSpinner.Margin = new System.Windows.Forms.Padding(0);
            this.tbSpinner.Name = "tbSpinner";
            this.tbSpinner.ReadOnly = true;
            this.tbSpinner.Size = new System.Drawing.Size(11, 13);
            this.tbSpinner.TabIndex = 4;
            this.tbSpinner.Text = "◐";
            this.tbSpinner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbSpinner.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.SystemColors.Control;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Consolas", 7.8F);
            this.button1.Location = new System.Drawing.Point(402, 1);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(52, 20);
            this.button1.TabIndex = 3;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tbPrompt
            // 
            this.tbPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbPrompt.BackColor = System.Drawing.SystemColors.Info;
            this.tbPrompt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbPrompt.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbPrompt.Location = new System.Drawing.Point(0, 108);
            this.tbPrompt.Margin = new System.Windows.Forms.Padding(0);
            this.tbPrompt.Name = "tbPrompt";
            this.tbPrompt.ReadOnly = true;
            this.tbPrompt.Size = new System.Drawing.Size(11, 16);
            this.tbPrompt.TabIndex = 2;
            this.tbPrompt.Text = ">";
            this.tbPrompt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rtbOutput
            // 
            this.rtbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbOutput.BackColor = System.Drawing.Color.White;
            this.rtbOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(0, 1);
            this.rtbOutput.Margin = new System.Windows.Forms.Padding(2);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbOutput.Size = new System.Drawing.Size(471, 106);
            this.rtbOutput.TabIndex = 0;
            this.rtbOutput.Text = "> a = 6\n> b = 7\n> a+b\n13\n> foo = function(x, y) return x + y * 2; end\n> a + foo(b" +
    ", a)\n26";
            this.rtbOutput.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbOutput_LinkClicked);
            this.rtbOutput.VScroll += new System.EventHandler(this.rtbOutput_VScroll);
            this.rtbOutput.Enter += new System.EventHandler(this.rtbOutput_Enter);
            this.rtbOutput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtbOutput_KeyDown);
            this.rtbOutput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtbOutput_KeyPress);
            this.rtbOutput.Leave += new System.EventHandler(this.rtbOutput_Leave);
            this.rtbOutput.MouseLeave += new System.EventHandler(this.rtbOutput_MouseLeave);
            // 
            // tmrSpinner
            // 
            this.tmrSpinner.Interval = 50;
            this.tmrSpinner.Tick += new System.EventHandler(this.tmrSpinner_Tick);
            // 
            // tmrWaitForSpinner
            // 
            this.tmrWaitForSpinner.Tick += new System.EventHandler(this.tmrWaitForSpinner_Tick);
            // 
            // LuaConsole
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LuaConsole";
            this.Size = new System.Drawing.Size(471, 124);
            this.Load += new System.EventHandler(this.LuaConsole_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LuaDebugger.RichTextBoxLink rtbOutput;
        private LuaDebugger.CopyChangedTextBox tbInput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbPrompt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer tmrSpinner;
        private System.Windows.Forms.TextBox tbSpinner;
        private System.Windows.Forms.Timer tmrWaitForSpinner;
        internal System.Windows.Forms.CheckBox CB_Locals;
    }
}
