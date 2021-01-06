namespace BackForceFeederGUI.GUI
{
    partial class OutputsEditor
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
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.chkInvertLampLogic = new System.Windows.Forms.CheckBox();
            this.btnResetAll = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstRawBits = new System.Windows.Forms.ListBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbLampBit = new System.Windows.Forms.ComboBox();
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
            this.splitContainerMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.IsSplitterFixed = true;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.label2);
            this.splitContainerMain.Panel1.Controls.Add(this.label3);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.btnClear);
            this.splitContainerMain.Panel2.Controls.Add(this.btnClose);
            this.splitContainerMain.Panel2.Controls.Add(this.chkInvertLampLogic);
            this.splitContainerMain.Panel2.Controls.Add(this.btnResetAll);
            this.splitContainerMain.Panel2.Controls.Add(this.btnRemove);
            this.splitContainerMain.Panel2.Controls.Add(this.btnAdd);
            this.splitContainerMain.Panel2.Controls.Add(this.lstRawBits);
            this.splitContainerMain.Panel2.Controls.Add(this.label11);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbLampBit);
            this.splitContainerMain.Panel2.Controls.Add(this.label9);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbBtnMapTo);
            this.splitContainerMain.Size = new System.Drawing.Size(429, 326);
            this.splitContainerMain.SplitterDistance = 214;
            this.splitContainerMain.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Raw output bits (lamps/drvbd)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Game lamps/drv output bits";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(298, 46);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(53, 23);
            this.btnClear.TabIndex = 28;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(354, 78);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 23);
            this.btnClose.TabIndex = 27;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // chkInvertLampLogic
            // 
            this.chkInvertLampLogic.AutoSize = true;
            this.chkInvertLampLogic.Location = new System.Drawing.Point(308, 19);
            this.chkInvertLampLogic.Name = "chkInvertLampLogic";
            this.chkInvertLampLogic.Size = new System.Drawing.Size(121, 17);
            this.chkInvertLampLogic.TabIndex = 26;
            this.chkInvertLampLogic.Text = "Inverted lamp logic?";
            this.chkInvertLampLogic.UseVisualStyleBackColor = true;
            this.chkInvertLampLogic.Click += new System.EventHandler(this.chkInvertLampLogic_Click);
            // 
            // btnResetAll
            // 
            this.btnResetAll.Location = new System.Drawing.Point(4, 78);
            this.btnResetAll.Name = "btnResetAll";
            this.btnResetAll.Size = new System.Drawing.Size(63, 23);
            this.btnResetAll.TabIndex = 25;
            this.btnResetAll.Text = "Reset all";
            this.btnResetAll.UseVisualStyleBackColor = true;
            this.btnResetAll.Click += new System.EventHandler(this.btnResetAll_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(239, 75);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(53, 23);
            this.btnRemove.TabIndex = 24;
            this.btnRemove.Text = "Del";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(239, 46);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(53, 23);
            this.btnAdd.TabIndex = 23;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstRawBits
            // 
            this.lstRawBits.Location = new System.Drawing.Point(113, 19);
            this.lstRawBits.Name = "lstRawBits";
            this.lstRawBits.ScrollAlwaysVisible = true;
            this.lstRawBits.Size = new System.Drawing.Size(120, 82);
            this.lstRawBits.TabIndex = 22;
            this.lstRawBits.SelectedValueChanged += new System.EventHandler(this.lstRawBit_SelectedValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(123, 3);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "to raw bit(s)";
            // 
            // cmbLampBit
            // 
            this.cmbLampBit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLampBit.FormattingEnabled = true;
            this.cmbLampBit.Location = new System.Drawing.Point(3, 20);
            this.cmbLampBit.Name = "cmbLampBit";
            this.cmbLampBit.Size = new System.Drawing.Size(62, 21);
            this.cmbLampBit.TabIndex = 19;
            this.cmbLampBit.SelectedIndexChanged += new System.EventHandler(this.cmbBtnMapFrom_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1, 3);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Mapping game lamp bit";
            // 
            // cmbBtnMapTo
            // 
            this.cmbBtnMapTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapTo.FormattingEnabled = true;
            this.cmbBtnMapTo.Location = new System.Drawing.Point(239, 19);
            this.cmbBtnMapTo.Name = "cmbBtnMapTo";
            this.cmbBtnMapTo.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapTo.TabIndex = 15;
            this.cmbBtnMapTo.SelectedIndexChanged += new System.EventHandler(this.cmbBtnMapTo_SelectedIndexChanged);
            // 
            // OutputsEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(429, 326);
            this.Controls.Add(this.splitContainerMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "OutputsEditor";
            this.Text = "Game lamps output mapping editor";
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
        private System.Windows.Forms.ComboBox cmbBtnMapTo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbLampBit;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox lstRawBits;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnResetAll;
        private System.Windows.Forms.CheckBox chkInvertLampLogic;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClear;
    }
}

