namespace BackForceFeederGUI.GUI
{
    partial class ButtonsEditor
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonsEditor));
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkKeyStroke = new System.Windows.Forms.CheckBox();
            this.cmbKeyStroke = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUpDnDelay = new System.Windows.Forms.TextBox();
            this.chkNeutralIsFirstBtn = new System.Windows.Forms.CheckBox();
            this.cmbShifterDecoder = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkSequenced = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkInvertRawLogic = new System.Windows.Forms.CheckBox();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstJoyBtn = new System.Windows.Forms.ListBox();
            this.chkAutofire = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chkToggling = new System.Windows.Forms.CheckBox();
            this.cmbBtnMapFrom = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbBtnMapTo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.IsSplitterFixed = true;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.label4);
            this.splitContainerMain.Panel1.Controls.Add(this.label2);
            this.splitContainerMain.Panel1.Controls.Add(this.label3);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.chkKeyStroke);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbKeyStroke);
            this.splitContainerMain.Panel2.Controls.Add(this.label5);
            this.splitContainerMain.Panel2.Controls.Add(this.txtUpDnDelay);
            this.splitContainerMain.Panel2.Controls.Add(this.chkNeutralIsFirstBtn);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbShifterDecoder);
            this.splitContainerMain.Panel2.Controls.Add(this.label1);
            this.splitContainerMain.Panel2.Controls.Add(this.chkSequenced);
            this.splitContainerMain.Panel2.Controls.Add(this.btnClose);
            this.splitContainerMain.Panel2.Controls.Add(this.chkInvertRawLogic);
            this.splitContainerMain.Panel2.Controls.Add(this.btnResetAll);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRemove);
            this.splitContainerMain.Panel2.Controls.Add(this.btnAdd);
            this.splitContainerMain.Panel2.Controls.Add(this.lstJoyBtn);
            this.splitContainerMain.Panel2.Controls.Add(this.chkAutofire);
            this.splitContainerMain.Panel2.Controls.Add(this.label11);
            this.splitContainerMain.Panel2.Controls.Add(this.chkToggling);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbBtnMapFrom);
            this.splitContainerMain.Panel2.Controls.Add(this.label9);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbBtnMapTo);
            this.splitContainerMain.Size = new System.Drawing.Size(684, 356);
            this.splitContainerMain.SplitterDistance = 229;
            this.splitContainerMain.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(506, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(180, 226);
            this.label4.TabIndex = 32;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(250, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "vJoy buttons";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Raw inputs";
            // 
            // chkKeyStroke
            // 
            this.chkKeyStroke.AutoSize = true;
            this.chkKeyStroke.Location = new System.Drawing.Point(430, 40);
            this.chkKeyStroke.Name = "chkKeyStroke";
            this.chkKeyStroke.Size = new System.Drawing.Size(109, 17);
            this.chkKeyStroke.TabIndex = 35;
            this.chkKeyStroke.Text = "Send KeyStroke?";
            this.chkKeyStroke.UseVisualStyleBackColor = true;
            this.chkKeyStroke.Click += new System.EventHandler(this.chkKeyStroke_Click);
            // 
            // cmbKeyStroke
            // 
            this.cmbKeyStroke.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKeyStroke.FormattingEnabled = true;
            this.cmbKeyStroke.Location = new System.Drawing.Point(544, 38);
            this.cmbKeyStroke.Name = "cmbKeyStroke";
            this.cmbKeyStroke.Size = new System.Drawing.Size(128, 21);
            this.cmbKeyStroke.TabIndex = 34;
            this.cmbKeyStroke.SelectedIndexChanged += new System.EventHandler(this.cmbKeyStroke_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(441, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 13);
            this.label5.TabIndex = 33;
            this.label5.Text = "Up/Dn delay (ms)";
            // 
            // txtUpDnDelay
            // 
            this.txtUpDnDelay.Location = new System.Drawing.Point(537, 90);
            this.txtUpDnDelay.Name = "txtUpDnDelay";
            this.txtUpDnDelay.Size = new System.Drawing.Size(61, 20);
            this.txtUpDnDelay.TabIndex = 33;
            this.txtUpDnDelay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUpDnDelay_KeyPress);
            this.txtUpDnDelay.Leave += new System.EventHandler(this.txtUpDnDelay_Leave);
            // 
            // chkNeutralIsFirstBtn
            // 
            this.chkNeutralIsFirstBtn.AutoSize = true;
            this.chkNeutralIsFirstBtn.Location = new System.Drawing.Point(307, 93);
            this.chkNeutralIsFirstBtn.Name = "chkNeutralIsFirstBtn";
            this.chkNeutralIsFirstBtn.Size = new System.Drawing.Size(128, 17);
            this.chkNeutralIsFirstBtn.TabIndex = 32;
            this.chkNeutralIsFirstBtn.Text = "Neutral is first button?";
            this.chkNeutralIsFirstBtn.UseVisualStyleBackColor = true;
            this.chkNeutralIsFirstBtn.Click += new System.EventHandler(this.chkNeutralIsFirstBtn_Click);
            // 
            // cmbShifterDecoder
            // 
            this.cmbShifterDecoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbShifterDecoder.FormattingEnabled = true;
            this.cmbShifterDecoder.Location = new System.Drawing.Point(457, 65);
            this.cmbShifterDecoder.Name = "cmbShifterDecoder";
            this.cmbShifterDecoder.Size = new System.Drawing.Size(144, 21);
            this.cmbShifterDecoder.TabIndex = 31;
            this.cmbShifterDecoder.SelectedIndexChanged += new System.EventHandler(this.cmbShifterDecoder_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(305, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "HShifter/UpDown decoder";
            // 
            // chkSequenced
            // 
            this.chkSequenced.AutoSize = true;
            this.chkSequenced.Location = new System.Drawing.Point(308, 40);
            this.chkSequenced.Name = "chkSequenced";
            this.chkSequenced.Size = new System.Drawing.Size(125, 17);
            this.chkSequenced.TabIndex = 28;
            this.chkSequenced.Text = "Sequenced buttons?";
            this.chkSequenced.UseVisualStyleBackColor = true;
            this.chkSequenced.Click += new System.EventHandler(this.chkSequenced_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(609, 88);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 23);
            this.btnClose.TabIndex = 27;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkInvertRawLogic
            // 
            this.chkInvertRawLogic.AutoSize = true;
            this.chkInvertRawLogic.Location = new System.Drawing.Point(308, 14);
            this.chkInvertRawLogic.Name = "chkInvertRawLogic";
            this.chkInvertRawLogic.Size = new System.Drawing.Size(116, 17);
            this.chkInvertRawLogic.TabIndex = 26;
            this.chkInvertRawLogic.Text = "Inverted raw logic?";
            this.chkInvertRawLogic.UseVisualStyleBackColor = true;
            this.chkInvertRawLogic.Click += new System.EventHandler(this.chkInvertRawLogic_Click);
            // 
            // btnResetAll
            // 
            this.btnResetAll.Location = new System.Drawing.Point(11, 88);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(63, 23);
            this.btnResetAll.TabIndex = 25;
            this.btnResetAll.Text = "Reset all";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(239, 88);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(53, 23);
            this.btnRemove.TabIndex = 24;
            this.btnRemove.Text = "Del";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(240, 55);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(53, 23);
            this.btnAdd.TabIndex = 23;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstJoyBtn
            // 
            this.lstJoyBtn.Location = new System.Drawing.Point(113, 26);
            this.lstJoyBtn.Name = "lstJoyBtn";
            this.lstJoyBtn.ScrollAlwaysVisible = true;
            this.lstJoyBtn.Size = new System.Drawing.Size(120, 82);
            this.lstJoyBtn.TabIndex = 22;
            this.lstJoyBtn.SelectedValueChanged += new System.EventHandler(this.lstJoyBtn_SelectedValueChanged);
            // 
            // chkAutofire
            // 
            this.chkAutofire.AutoSize = true;
            this.chkAutofire.Location = new System.Drawing.Point(544, 14);
            this.chkAutofire.Name = "chkAutofire";
            this.chkAutofire.Size = new System.Drawing.Size(100, 17);
            this.chkAutofire.TabIndex = 17;
            this.chkAutofire.Text = "Autofire mode ?";
            this.chkAutofire.UseVisualStyleBackColor = true;
            this.chkAutofire.Click += new System.EventHandler(this.chkAutofire_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(113, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(85, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "to vJoy button(s)";
            // 
            // chkToggling
            // 
            this.chkToggling.AutoSize = true;
            this.chkToggling.Location = new System.Drawing.Point(430, 14);
            this.chkToggling.Name = "chkToggling";
            this.chkToggling.Size = new System.Drawing.Size(105, 17);
            this.chkToggling.TabIndex = 16;
            this.chkToggling.Text = "Toggling mode ?";
            this.chkToggling.UseVisualStyleBackColor = true;
            this.chkToggling.Click += new System.EventHandler(this.chkToggling_Click);
            // 
            // cmbBtnMapFrom
            // 
            this.cmbBtnMapFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapFrom.FormattingEnabled = true;
            this.cmbBtnMapFrom.Location = new System.Drawing.Point(12, 23);
            this.cmbBtnMapFrom.Name = "cmbBtnMapFrom";
            this.cmbBtnMapFrom.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapFrom.TabIndex = 19;
            this.cmbBtnMapFrom.SelectedIndexChanged += new System.EventHandler(this.cmbBtnMapFrom_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Mapping raw input";
            // 
            // cmbBtnMapTo
            // 
            this.cmbBtnMapTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapTo.FormattingEnabled = true;
            this.cmbBtnMapTo.Location = new System.Drawing.Point(240, 23);
            this.cmbBtnMapTo.Name = "cmbBtnMapTo";
            this.cmbBtnMapTo.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapTo.TabIndex = 15;
            this.cmbBtnMapTo.SelectedIndexChanged += new System.EventHandler(this.cmbBtnMapTo_SelectedIndexChanged);
            // 
            // ButtonsEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(684, 356);
            this.Controls.Add(this.splitContainerMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ButtonsEditor";
            this.Text = "Button mapping editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAutofire;
        private System.Windows.Forms.CheckBox chkToggling;
        private System.Windows.Forms.ComboBox cmbBtnMapTo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbBtnMapFrom;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox lstJoyBtn;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.CheckBox chkInvertRawLogic;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.CheckBox chkSequenced;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbShifterDecoder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkNeutralIsFirstBtn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtUpDnDelay;
        private System.Windows.Forms.CheckBox chkKeyStroke;
        private System.Windows.Forms.ComboBox cmbKeyStroke;
    }
}

