namespace IOFeederGUI.GUI
{
    partial class ButtonsForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cmbBtnMapFrom = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbBtnMapTo = new System.Windows.Forms.ComboBox();
            this.chkToggling = new System.Windows.Forms.CheckBox();
            this.chkAutofire = new System.Windows.Forms.CheckBox();
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
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.label3);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.label11);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbBtnMapFrom);
            this.splitContainerMain.Panel2.Controls.Add(this.label9);
            this.splitContainerMain.Panel2.Controls.Add(this.label10);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbBtnMapTo);
            this.splitContainerMain.Panel2.Controls.Add(this.chkToggling);
            this.splitContainerMain.Panel2.Controls.Add(this.chkAutofire);
            this.splitContainerMain.Size = new System.Drawing.Size(526, 326);
            this.splitContainerMain.SplitterDistance = 258;
            this.splitContainerMain.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Raw buttons";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(110, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "vJoy";
            // 
            // cmbBtnMapFrom
            // 
            this.cmbBtnMapFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapFrom.FormattingEnabled = true;
            this.cmbBtnMapFrom.Location = new System.Drawing.Point(8, 29);
            this.cmbBtnMapFrom.Name = "cmbBtnMapFrom";
            this.cmbBtnMapFrom.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapFrom.TabIndex = 19;
            this.cmbBtnMapFrom.SelectedIndexChanged += new System.EventHandler(this.cmbBtnMapFrom_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 14);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Mapping raw";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(77, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "to";
            // 
            // cmbBtnMapTo
            // 
            this.cmbBtnMapTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapTo.FormattingEnabled = true;
            this.cmbBtnMapTo.Location = new System.Drawing.Point(98, 29);
            this.cmbBtnMapTo.Name = "cmbBtnMapTo";
            this.cmbBtnMapTo.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapTo.TabIndex = 15;
            this.cmbBtnMapTo.SelectedIndexChanged += new System.EventHandler(this.cmbBtnMapTo_SelectedIndexChanged);
            // 
            // chkToggling
            // 
            this.chkToggling.AutoSize = true;
            this.chkToggling.Location = new System.Drawing.Point(168, 32);
            this.chkToggling.Name = "chkToggling";
            this.chkToggling.Size = new System.Drawing.Size(105, 17);
            this.chkToggling.TabIndex = 16;
            this.chkToggling.Text = "Toggling mode ?";
            this.chkToggling.UseVisualStyleBackColor = true;
            // 
            // chkAutofire
            // 
            this.chkAutofire.AutoSize = true;
            this.chkAutofire.Location = new System.Drawing.Point(283, 32);
            this.chkAutofire.Name = "chkAutofire";
            this.chkAutofire.Size = new System.Drawing.Size(100, 17);
            this.chkAutofire.TabIndex = 17;
            this.chkAutofire.Text = "Autofire mode ?";
            this.chkAutofire.UseVisualStyleBackColor = true;
            // 
            // ButtonsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(526, 326);
            this.Controls.Add(this.splitContainerMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ButtonsForm";
            this.Text = "vJoyIOFeeder";
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
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}

