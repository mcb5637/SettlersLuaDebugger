namespace LuaDebugger.Plugins.S5CutsceneEditor
{
    partial class S5CutsceneEditorMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(S5CutsceneEditorMain));
            this.tmrUpdateCamera = new System.Windows.Forms.Timer(this.components);
            this.btnFreeFlight = new System.Windows.Forms.Button();
            this.lvCut = new System.Windows.Forms.ListView();
            this.chJumpTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chProperties = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chLuaCall = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cbFlights = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddFlight = new System.Windows.Forms.Button();
            this.btnAddPointBelow = new System.Windows.Forms.Button();
            this.btnAddPointAbove = new System.Windows.Forms.Button();
            this.tbFlightName = new System.Windows.Forms.TextBox();
            this.btnFlightNrDown = new System.Windows.Forms.Button();
            this.btnFlightNrUp = new System.Windows.Forms.Button();
            this.cbInvertCameraControl = new System.Windows.Forms.CheckBox();
            this.btnPlaySelected = new System.Windows.Forms.Button();
            this.btnPlayFlight = new System.Windows.Forms.Button();
            this.btnSaveCutscene = new System.Windows.Forms.Button();
            this.btnRemoveFlight = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gbPreviewCutscene = new System.Windows.Forms.GroupBox();
            this.btnPlayCutscene = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.tbSpeed = new System.Windows.Forms.TrackBar();
            this.joyStickCutsceneEditor = new LuaDebugger.Plugins.S5CutsceneEditor.JoyStick();
            this.gbPreviewCutscene.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrUpdateCamera
            // 
            this.tmrUpdateCamera.Interval = 5;
            this.tmrUpdateCamera.Tick += new System.EventHandler(this.tmrUpdateCamera_Tick);
            // 
            // btnFreeFlight
            // 
            this.btnFreeFlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFreeFlight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFreeFlight.Location = new System.Drawing.Point(15, 344);
            this.btnFreeFlight.Name = "btnFreeFlight";
            this.btnFreeFlight.Size = new System.Drawing.Size(138, 28);
            this.btnFreeFlight.TabIndex = 2;
            this.btnFreeFlight.Text = "Toggle Free Flight";
            this.btnFreeFlight.UseVisualStyleBackColor = true;
            this.btnFreeFlight.Click += new System.EventHandler(this.btnFreeFlight_Click);
            // 
            // lvCut
            // 
            this.lvCut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvCut.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chJumpTo,
            this.chProperties,
            this.chLuaCall});
            this.lvCut.FullRowSelect = true;
            this.lvCut.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvCut.HideSelection = false;
            this.lvCut.Location = new System.Drawing.Point(235, 12);
            this.lvCut.MultiSelect = false;
            this.lvCut.Name = "lvCut";
            this.lvCut.Size = new System.Drawing.Size(319, 323);
            this.lvCut.TabIndex = 5;
            this.lvCut.UseCompatibleStateImageBehavior = false;
            this.lvCut.View = System.Windows.Forms.View.Details;
            this.lvCut.SelectedIndexChanged += new System.EventHandler(this.lvCut_SelectedIndexChanged);
            this.lvCut.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvCut_MouseClick);
            this.lvCut.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvCut_MouseDoubleClick);
            // 
            // chJumpTo
            // 
            this.chJumpTo.Text = "JumpTo";
            this.chJumpTo.Width = 53;
            // 
            // chProperties
            // 
            this.chProperties.Text = "Properties";
            this.chProperties.Width = 66;
            // 
            // chLuaCall
            // 
            this.chLuaCall.Text = "Lua Callback";
            this.chLuaCall.Width = 161;
            // 
            // cbFlights
            // 
            this.cbFlights.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFlights.FormattingEnabled = true;
            this.cbFlights.Location = new System.Drawing.Point(58, 12);
            this.cbFlights.Name = "cbFlights";
            this.cbFlights.Size = new System.Drawing.Size(110, 21);
            this.cbFlights.TabIndex = 6;
            this.cbFlights.SelectedIndexChanged += new System.EventHandler(this.cbFlights_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Flights:";
            // 
            // btnAddFlight
            // 
            this.btnAddFlight.Location = new System.Drawing.Point(174, 66);
            this.btnAddFlight.Name = "btnAddFlight";
            this.btnAddFlight.Size = new System.Drawing.Size(55, 23);
            this.btnAddFlight.TabIndex = 8;
            this.btnAddFlight.Text = "Add";
            this.btnAddFlight.UseVisualStyleBackColor = true;
            this.btnAddFlight.Click += new System.EventHandler(this.btnAddFlight_Click);
            // 
            // btnAddPointBelow
            // 
            this.btnAddPointBelow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddPointBelow.Location = new System.Drawing.Point(11, 290);
            this.btnAddPointBelow.Name = "btnAddPointBelow";
            this.btnAddPointBelow.Size = new System.Drawing.Size(142, 40);
            this.btnAddPointBelow.TabIndex = 10;
            this.btnAddPointBelow.Text = "Add Point Below";
            this.btnAddPointBelow.UseVisualStyleBackColor = true;
            this.btnAddPointBelow.Click += new System.EventHandler(this.btnAddPointBelow_Click);
            // 
            // btnAddPointAbove
            // 
            this.btnAddPointAbove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddPointAbove.Location = new System.Drawing.Point(11, 244);
            this.btnAddPointAbove.Name = "btnAddPointAbove";
            this.btnAddPointAbove.Size = new System.Drawing.Size(68, 40);
            this.btnAddPointAbove.TabIndex = 11;
            this.btnAddPointAbove.Text = "Add Point Above";
            this.btnAddPointAbove.UseVisualStyleBackColor = true;
            this.btnAddPointAbove.Click += new System.EventHandler(this.btnAddPointAbove_Click);
            // 
            // tbFlightName
            // 
            this.tbFlightName.Location = new System.Drawing.Point(58, 68);
            this.tbFlightName.Name = "tbFlightName";
            this.tbFlightName.Size = new System.Drawing.Size(110, 20);
            this.tbFlightName.TabIndex = 12;
            this.tbFlightName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbFlightName_KeyDown);
            // 
            // btnFlightNrDown
            // 
            this.btnFlightNrDown.Enabled = false;
            this.btnFlightNrDown.Location = new System.Drawing.Point(58, 39);
            this.btnFlightNrDown.Name = "btnFlightNrDown";
            this.btnFlightNrDown.Size = new System.Drawing.Size(53, 23);
            this.btnFlightNrDown.TabIndex = 13;
            this.btnFlightNrDown.Tag = "1";
            this.btnFlightNrDown.Text = "Down";
            this.btnFlightNrDown.UseVisualStyleBackColor = true;
            this.btnFlightNrDown.Click += new System.EventHandler(this.MoveFlightOrder);
            // 
            // btnFlightNrUp
            // 
            this.btnFlightNrUp.Enabled = false;
            this.btnFlightNrUp.Location = new System.Drawing.Point(117, 39);
            this.btnFlightNrUp.Name = "btnFlightNrUp";
            this.btnFlightNrUp.Size = new System.Drawing.Size(51, 23);
            this.btnFlightNrUp.TabIndex = 15;
            this.btnFlightNrUp.Tag = "-1";
            this.btnFlightNrUp.Text = "Up";
            this.btnFlightNrUp.UseVisualStyleBackColor = true;
            this.btnFlightNrUp.Click += new System.EventHandler(this.MoveFlightOrder);
            // 
            // cbInvertCameraControl
            // 
            this.cbInvertCameraControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbInvertCameraControl.AutoSize = true;
            this.cbInvertCameraControl.Location = new System.Drawing.Point(19, 584);
            this.cbInvertCameraControl.Name = "cbInvertCameraControl";
            this.cbInvertCameraControl.Size = new System.Drawing.Size(128, 17);
            this.cbInvertCameraControl.TabIndex = 16;
            this.cbInvertCameraControl.Text = "Invert Camera Control";
            this.cbInvertCameraControl.UseVisualStyleBackColor = true;
            this.cbInvertCameraControl.CheckedChanged += new System.EventHandler(this.cbInvertCameraControl_CheckedChanged);
            // 
            // btnPlaySelected
            // 
            this.btnPlaySelected.Enabled = false;
            this.btnPlaySelected.Location = new System.Drawing.Point(21, 26);
            this.btnPlaySelected.Name = "btnPlaySelected";
            this.btnPlaySelected.Size = new System.Drawing.Size(171, 23);
            this.btnPlaySelected.TabIndex = 17;
            this.btnPlaySelected.Text = "Play Selected";
            this.btnPlaySelected.UseVisualStyleBackColor = true;
            this.btnPlaySelected.Click += new System.EventHandler(this.btnPlaySelected_Click);
            // 
            // btnPlayFlight
            // 
            this.btnPlayFlight.Location = new System.Drawing.Point(21, 57);
            this.btnPlayFlight.Name = "btnPlayFlight";
            this.btnPlayFlight.Size = new System.Drawing.Size(171, 23);
            this.btnPlayFlight.TabIndex = 20;
            this.btnPlayFlight.Text = "Play Flight";
            this.btnPlayFlight.UseVisualStyleBackColor = true;
            this.btnPlayFlight.Click += new System.EventHandler(this.btnPlayFlight_Click);
            // 
            // btnSaveCutscene
            // 
            this.btnSaveCutscene.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveCutscene.Location = new System.Drawing.Point(160, 244);
            this.btnSaveCutscene.Name = "btnSaveCutscene";
            this.btnSaveCutscene.Size = new System.Drawing.Size(68, 40);
            this.btnSaveCutscene.TabIndex = 21;
            this.btnSaveCutscene.Text = "Save Cutscene";
            this.btnSaveCutscene.UseVisualStyleBackColor = true;
            this.btnSaveCutscene.Click += new System.EventHandler(this.btnSaveCutscene_Click);
            // 
            // btnRemoveFlight
            // 
            this.btnRemoveFlight.Enabled = false;
            this.btnRemoveFlight.Location = new System.Drawing.Point(174, 10);
            this.btnRemoveFlight.Name = "btnRemoveFlight";
            this.btnRemoveFlight.Size = new System.Drawing.Size(55, 23);
            this.btnRemoveFlight.TabIndex = 22;
            this.btnRemoveFlight.Tag = "-1";
            this.btnRemoveFlight.Text = "Remove";
            this.btnRemoveFlight.UseVisualStyleBackColor = true;
            this.btnRemoveFlight.Click += new System.EventHandler(this.btnRemoveFlight_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "New:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Order:";
            // 
            // gbPreviewCutscene
            // 
            this.gbPreviewCutscene.Controls.Add(this.btnPlayCutscene);
            this.gbPreviewCutscene.Controls.Add(this.btnPlaySelected);
            this.gbPreviewCutscene.Controls.Add(this.btnPlayFlight);
            this.gbPreviewCutscene.Location = new System.Drawing.Point(15, 105);
            this.gbPreviewCutscene.Name = "gbPreviewCutscene";
            this.gbPreviewCutscene.Size = new System.Drawing.Size(211, 123);
            this.gbPreviewCutscene.TabIndex = 25;
            this.gbPreviewCutscene.TabStop = false;
            this.gbPreviewCutscene.Text = "Preview Cutscene";
            // 
            // btnPlayCutscene
            // 
            this.btnPlayCutscene.Location = new System.Drawing.Point(21, 89);
            this.btnPlayCutscene.Name = "btnPlayCutscene";
            this.btnPlayCutscene.Size = new System.Drawing.Size(171, 23);
            this.btnPlayCutscene.TabIndex = 21;
            this.btnPlayCutscene.Text = "Play Cutscene";
            this.btnPlayCutscene.UseVisualStyleBackColor = true;
            this.btnPlayCutscene.Click += new System.EventHandler(this.btnPlayCutscene_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(160, 290);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(68, 40);
            this.btnLoad.TabIndex = 26;
            this.btnLoad.Text = "Load Cutscene";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplace.Location = new System.Drawing.Point(85, 244);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(68, 40);
            this.btnReplace.TabIndex = 27;
            this.btnReplace.Text = "Replace Point";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // tbSpeed
            // 
            this.tbSpeed.AutoSize = false;
            this.tbSpeed.Location = new System.Drawing.Point(348, 579);
            this.tbSpeed.Maximum = 50;
            this.tbSpeed.Minimum = 1;
            this.tbSpeed.Name = "tbSpeed";
            this.tbSpeed.Size = new System.Drawing.Size(201, 23);
            this.tbSpeed.TabIndex = 28;
            this.tbSpeed.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbSpeed.Value = 20;
            this.tbSpeed.Scroll += new System.EventHandler(this.tbSpeed_Scroll);
            // 
            // joyStickCutsceneEditor
            // 
            this.joyStickCutsceneEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.joyStickCutsceneEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.joyStickCutsceneEditor.Enabled = false;
            this.joyStickCutsceneEditor.Location = new System.Drawing.Point(12, 341);
            this.joyStickCutsceneEditor.Name = "joyStickCutsceneEditor";
            this.joyStickCutsceneEditor.Size = new System.Drawing.Size(542, 263);
            this.joyStickCutsceneEditor.TabIndex = 0;
            // 
            // S5CutsceneEditorMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 616);
            this.Controls.Add(this.tbSpeed);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.gbPreviewCutscene);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRemoveFlight);
            this.Controls.Add(this.btnSaveCutscene);
            this.Controls.Add(this.cbInvertCameraControl);
            this.Controls.Add(this.btnFlightNrUp);
            this.Controls.Add(this.btnFlightNrDown);
            this.Controls.Add(this.tbFlightName);
            this.Controls.Add(this.btnAddPointAbove);
            this.Controls.Add(this.btnAddPointBelow);
            this.Controls.Add(this.btnAddFlight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbFlights);
            this.Controls.Add(this.lvCut);
            this.Controls.Add(this.btnFreeFlight);
            this.Controls.Add(this.joyStickCutsceneEditor);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "S5CutsceneEditorMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "S5 Cutscene Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.S5CutsceneEditorMain_FormClosing);
            this.gbPreviewCutscene.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LuaDebugger.Plugins.S5CutsceneEditor.JoyStick joyStickCutsceneEditor;
        private System.Windows.Forms.Timer tmrUpdateCamera;
        private System.Windows.Forms.Button btnFreeFlight;
        private System.Windows.Forms.ListView lvCut;
        private System.Windows.Forms.ColumnHeader chJumpTo;
        private System.Windows.Forms.ColumnHeader chProperties;
        private System.Windows.Forms.ColumnHeader chLuaCall;
        private System.Windows.Forms.ComboBox cbFlights;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddFlight;
        private System.Windows.Forms.Button btnAddPointBelow;
        private System.Windows.Forms.Button btnAddPointAbove;
        private System.Windows.Forms.TextBox tbFlightName;
        private System.Windows.Forms.Button btnFlightNrDown;
        private System.Windows.Forms.Button btnFlightNrUp;
        private System.Windows.Forms.CheckBox cbInvertCameraControl;
        private System.Windows.Forms.Button btnPlaySelected;
        private System.Windows.Forms.Button btnPlayFlight;
        private System.Windows.Forms.Button btnSaveCutscene;
        private System.Windows.Forms.Button btnRemoveFlight;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbPreviewCutscene;
        private System.Windows.Forms.Button btnPlayCutscene;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.TrackBar tbSpeed;

    }
}