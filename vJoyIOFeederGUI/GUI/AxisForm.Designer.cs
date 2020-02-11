namespace IOFeederGUI.GUI
{
    partial class AxisForm
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
            this.slJoyAxis = new System.Windows.Forms.ProgressBar();
            this.slRawAxis = new System.Windows.Forms.ProgressBar();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.axesJoyGauge = new LiveCharts.WinForms.AngularGauge();
            this.label2 = new System.Windows.Forms.Label();
            this.txtJoyAxisValue = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbSelectedAxis = new System.Windows.Forms.ComboBox();
            this.btnAxisMappingEditor = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRawAxisValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // slJoyAxis
            // 
            this.slJoyAxis.Location = new System.Drawing.Point(186, 72);
            this.slJoyAxis.Maximum = 255;
            this.slJoyAxis.Name = "slJoyAxis";
            this.slJoyAxis.Size = new System.Drawing.Size(79, 20);
            this.slJoyAxis.Step = 255;
            this.slJoyAxis.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.slJoyAxis.TabIndex = 1;
            // 
            // slRawAxis
            // 
            this.slRawAxis.Location = new System.Drawing.Point(228, 15);
            this.slRawAxis.Maximum = 255;
            this.slRawAxis.Name = "slRawAxis";
            this.slRawAxis.Size = new System.Drawing.Size(100, 23);
            this.slRawAxis.Step = 255;
            this.slRawAxis.TabIndex = 2;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(12, 12);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.axesJoyGauge);
            this.splitContainerMain.Panel1.Controls.Add(this.label2);
            this.splitContainerMain.Panel1.Controls.Add(this.txtJoyAxisValue);
            this.splitContainerMain.Panel1.Controls.Add(this.label6);
            this.splitContainerMain.Panel1.Controls.Add(this.cmbSelectedAxis);
            this.splitContainerMain.Panel1.Controls.Add(this.slJoyAxis);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.btnAxisMappingEditor);
            this.splitContainerMain.Panel2.Controls.Add(this.slRawAxis);
            this.splitContainerMain.Panel2.Controls.Add(this.label4);
            this.splitContainerMain.Panel2.Controls.Add(this.txtRawAxisValue);
            this.splitContainerMain.Size = new System.Drawing.Size(776, 426);
            this.splitContainerMain.SplitterDistance = 200;
            this.splitContainerMain.TabIndex = 6;
            // 
            // axesJoyGauge
            // 
            this.axesJoyGauge.Location = new System.Drawing.Point(325, 20);
            this.axesJoyGauge.Name = "axesJoyGauge";
            this.axesJoyGauge.Size = new System.Drawing.Size(113, 94);
            this.axesJoyGauge.TabIndex = 17;
            this.axesJoyGauge.Text = "angularGaugeAxis";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Selected axis";
            // 
            // txtJoyAxisValue
            // 
            this.txtJoyAxisValue.Location = new System.Drawing.Point(91, 72);
            this.txtJoyAxisValue.Name = "txtJoyAxisValue";
            this.txtJoyAxisValue.Size = new System.Drawing.Size(89, 20);
            this.txtJoyAxisValue.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "vJoy value";
            // 
            // cmbSelectedAxis
            // 
            this.cmbSelectedAxis.AllowDrop = true;
            this.cmbSelectedAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectedAxis.FormattingEnabled = true;
            this.cmbSelectedAxis.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Rx",
            "Ry",
            "Rz",
            "Sl0",
            "Sl1/Dial"});
            this.cmbSelectedAxis.Location = new System.Drawing.Point(91, 45);
            this.cmbSelectedAxis.Name = "cmbSelectedAxis";
            this.cmbSelectedAxis.Size = new System.Drawing.Size(89, 21);
            this.cmbSelectedAxis.TabIndex = 6;
            // 
            // btnAxisMappingEditor
            // 
            this.btnAxisMappingEditor.Location = new System.Drawing.Point(353, 15);
            this.btnAxisMappingEditor.Name = "btnAxisMappingEditor";
            this.btnAxisMappingEditor.Size = new System.Drawing.Size(121, 23);
            this.btnAxisMappingEditor.TabIndex = 18;
            this.btnAxisMappingEditor.Text = "Axis mapping Editor";
            this.btnAxisMappingEditor.UseVisualStyleBackColor = true;
            this.btnAxisMappingEditor.Click += new System.EventHandler(this.btnAxisMappingEditor_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Raw value";
            // 
            // txtRawAxisValue
            // 
            this.txtRawAxisValue.Location = new System.Drawing.Point(122, 15);
            this.txtRawAxisValue.Name = "txtRawAxisValue";
            this.txtRawAxisValue.Size = new System.Drawing.Size(100, 20);
            this.txtRawAxisValue.TabIndex = 9;
            // 
            // AxisForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainerMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AxisForm";
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
        private System.Windows.Forms.ProgressBar slJoyAxis;
        private System.Windows.Forms.ProgressBar slRawAxis;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TextBox txtRawAxisValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSelectedAxis;
        private System.Windows.Forms.TextBox txtJoyAxisValue;
        private System.Windows.Forms.Label label6;
        private LiveCharts.WinForms.AngularGauge axesJoyGauge;
        private System.Windows.Forms.Button btnAxisMappingEditor;
    }
}

