namespace vJoyIOFeederGUI.GUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.slJoyAxis = new System.Windows.Forms.ProgressBar();
            this.slRawAxis = new System.Windows.Forms.ProgressBar();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.lblCurrentGame = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbConfigSet = new System.Windows.Forms.ComboBox();
            this.btnControlSets = new System.Windows.Forms.Button();
            this.btnTuneEffects = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.btnOutputs = new System.Windows.Forms.Button();
            this.btnShowLogWindow = new System.Windows.Forms.Button();
            this.btnConfigureHardware = new System.Windows.Forms.Button();
            this.btnButtons = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRawAxisValue = new System.Windows.Forms.TextBox();
            this.btnAxisMappingEditor = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.axesJoyGauge = new LiveCharts.WinForms.AngularGauge();
            this.txtJoyAxisValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSelectedAxis = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // slJoyAxis
            // 
            this.slJoyAxis.Location = new System.Drawing.Point(197, 36);
            this.slJoyAxis.Maximum = 255;
            this.slJoyAxis.Name = "slJoyAxis";
            this.slJoyAxis.Size = new System.Drawing.Size(109, 20);
            this.slJoyAxis.Step = 255;
            this.slJoyAxis.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.slJoyAxis.TabIndex = 1;
            // 
            // slRawAxis
            // 
            this.slRawAxis.Location = new System.Drawing.Point(197, 65);
            this.slRawAxis.Maximum = 255;
            this.slRawAxis.Name = "slRawAxis";
            this.slRawAxis.Size = new System.Drawing.Size(109, 20);
            this.slRawAxis.Step = 255;
            this.slRawAxis.TabIndex = 2;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 200;
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
            this.splitContainerMain.Panel1.Controls.Add(this.lblCurrentGame);
            this.splitContainerMain.Panel1.Controls.Add(this.label7);
            this.splitContainerMain.Panel1.Controls.Add(this.cmbConfigSet);
            this.splitContainerMain.Panel1.Controls.Add(this.btnControlSets);
            this.splitContainerMain.Panel1.Controls.Add(this.btnTuneEffects);
            this.splitContainerMain.Panel1.Controls.Add(this.labelStatus);
            this.splitContainerMain.Panel1.Controls.Add(this.btnOutputs);
            this.splitContainerMain.Panel1.Controls.Add(this.btnShowLogWindow);
            this.splitContainerMain.Panel1.Controls.Add(this.btnConfigureHardware);
            this.splitContainerMain.Panel1.Controls.Add(this.btnButtons);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.label5);
            this.splitContainerMain.Panel2.Controls.Add(this.label1);
            this.splitContainerMain.Panel2.Controls.Add(this.txtRawAxisValue);
            this.splitContainerMain.Panel2.Controls.Add(this.btnAxisMappingEditor);
            this.splitContainerMain.Panel2.Controls.Add(this.slRawAxis);
            this.splitContainerMain.Panel2.Controls.Add(this.label4);
            this.splitContainerMain.Panel2.Controls.Add(this.axesJoyGauge);
            this.splitContainerMain.Panel2.Controls.Add(this.txtJoyAxisValue);
            this.splitContainerMain.Panel2.Controls.Add(this.label2);
            this.splitContainerMain.Panel2.Controls.Add(this.label3);
            this.splitContainerMain.Panel2.Controls.Add(this.cmbSelectedAxis);
            this.splitContainerMain.Panel2.Controls.Add(this.label6);
            this.splitContainerMain.Panel2.Controls.Add(this.slJoyAxis);
            this.splitContainerMain.Size = new System.Drawing.Size(680, 298);
            this.splitContainerMain.SplitterDistance = 69;
            this.splitContainerMain.TabIndex = 6;
            // 
            // lblCurrentGame
            // 
            this.lblCurrentGame.AutoSize = true;
            this.lblCurrentGame.Location = new System.Drawing.Point(572, 46);
            this.lblCurrentGame.Name = "lblCurrentGame";
            this.lblCurrentGame.Size = new System.Drawing.Size(38, 13);
            this.lblCurrentGame.TabIndex = 26;
            this.lblCurrentGame.Text = "Game:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(402, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "Control set";
            // 
            // cmbConfigSet
            // 
            this.cmbConfigSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbConfigSet.FormattingEnabled = true;
            this.cmbConfigSet.Location = new System.Drawing.Point(463, 42);
            this.cmbConfigSet.Name = "cmbConfigSet";
            this.cmbConfigSet.Size = new System.Drawing.Size(103, 21);
            this.cmbConfigSet.TabIndex = 24;
            this.cmbConfigSet.SelectedIndexChanged += new System.EventHandler(this.cmbConfigSet_SelectedIndexChanged);
            // 
            // btnControlSets
            // 
            this.btnControlSets.Location = new System.Drawing.Point(463, 12);
            this.btnControlSets.Name = "btnControlSets";
            this.btnControlSets.Size = new System.Drawing.Size(98, 23);
            this.btnControlSets.TabIndex = 23;
            this.btnControlSets.Text = "Control Sets";
            this.btnControlSets.UseVisualStyleBackColor = true;
            this.btnControlSets.Click += new System.EventHandler(this.btnControlSets_Click);
            // 
            // btnTuneEffects
            // 
            this.btnTuneEffects.Location = new System.Drawing.Point(351, 12);
            this.btnTuneEffects.Name = "btnTuneEffects";
            this.btnTuneEffects.Size = new System.Drawing.Size(106, 23);
            this.btnTuneEffects.TabIndex = 22;
            this.btnTuneEffects.Text = "Tune FFB effects";
            this.btnTuneEffects.UseVisualStyleBackColor = true;
            this.btnTuneEffects.Click += new System.EventHandler(this.btnTuneEffects_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(13, 42);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(48, 17);
            this.labelStatus.TabIndex = 21;
            this.labelStatus.Text = "Status";
            // 
            // btnOutputs
            // 
            this.btnOutputs.Enabled = false;
            this.btnOutputs.Location = new System.Drawing.Point(239, 12);
            this.btnOutputs.Name = "btnOutputs";
            this.btnOutputs.Size = new System.Drawing.Size(106, 23);
            this.btnOutputs.TabIndex = 20;
            this.btnOutputs.Text = "Configure outputs";
            this.btnOutputs.UseVisualStyleBackColor = true;
            // 
            // btnShowLogWindow
            // 
            this.btnShowLogWindow.Location = new System.Drawing.Point(570, 12);
            this.btnShowLogWindow.Name = "btnShowLogWindow";
            this.btnShowLogWindow.Size = new System.Drawing.Size(98, 23);
            this.btnShowLogWindow.TabIndex = 16;
            this.btnShowLogWindow.Text = "Log window";
            this.btnShowLogWindow.UseVisualStyleBackColor = true;
            this.btnShowLogWindow.Click += new System.EventHandler(this.btnShowLogWindow_Click);
            // 
            // btnConfigureHardware
            // 
            this.btnConfigureHardware.Location = new System.Drawing.Point(12, 12);
            this.btnConfigureHardware.Name = "btnConfigureHardware";
            this.btnConfigureHardware.Size = new System.Drawing.Size(109, 23);
            this.btnConfigureHardware.TabIndex = 13;
            this.btnConfigureHardware.Text = "App. and hardware";
            this.btnConfigureHardware.UseVisualStyleBackColor = true;
            this.btnConfigureHardware.Click += new System.EventHandler(this.btnConfigureHardware_Click);
            // 
            // btnButtons
            // 
            this.btnButtons.Location = new System.Drawing.Point(127, 12);
            this.btnButtons.Name = "btnButtons";
            this.btnButtons.Size = new System.Drawing.Size(106, 23);
            this.btnButtons.TabIndex = 18;
            this.btnButtons.Text = "Configure buttons";
            this.btnButtons.UseVisualStyleBackColor = true;
            this.btnButtons.Click += new System.EventHandler(this.btnButtons_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(554, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Outputs (lamps/drvbd)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(448, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Raw inputs";
            // 
            // txtRawAxisValue
            // 
            this.txtRawAxisValue.Location = new System.Drawing.Point(83, 65);
            this.txtRawAxisValue.Name = "txtRawAxisValue";
            this.txtRawAxisValue.Size = new System.Drawing.Size(100, 20);
            this.txtRawAxisValue.TabIndex = 9;
            // 
            // btnAxisMappingEditor
            // 
            this.btnAxisMappingEditor.Location = new System.Drawing.Point(197, 7);
            this.btnAxisMappingEditor.Name = "btnAxisMappingEditor";
            this.btnAxisMappingEditor.Size = new System.Drawing.Size(109, 23);
            this.btnAxisMappingEditor.TabIndex = 18;
            this.btnAxisMappingEditor.Text = "Axis mapping Editor";
            this.btnAxisMappingEditor.UseVisualStyleBackColor = true;
            this.btnAxisMappingEditor.Click += new System.EventHandler(this.btnAxisMappingEditor_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Raw value";
            // 
            // axesJoyGauge
            // 
            this.axesJoyGauge.Location = new System.Drawing.Point(83, 92);
            this.axesJoyGauge.Name = "axesJoyGauge";
            this.axesJoyGauge.Size = new System.Drawing.Size(113, 94);
            this.axesJoyGauge.TabIndex = 17;
            this.axesJoyGauge.Text = "angularGaugeAxis";
            // 
            // txtJoyAxisValue
            // 
            this.txtJoyAxisValue.Location = new System.Drawing.Point(83, 36);
            this.txtJoyAxisValue.Name = "txtJoyAxisValue";
            this.txtJoyAxisValue.Size = new System.Drawing.Size(100, 20);
            this.txtJoyAxisValue.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Selected axis";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(338, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "vJoy buttons";
            // 
            // cmbSelectedAxis
            // 
            this.cmbSelectedAxis.AllowDrop = true;
            this.cmbSelectedAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectedAxis.FormattingEnabled = true;
            this.cmbSelectedAxis.Location = new System.Drawing.Point(83, 9);
            this.cmbSelectedAxis.Name = "cmbSelectedAxis";
            this.cmbSelectedAxis.Size = new System.Drawing.Size(100, 21);
            this.cmbSelectedAxis.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "vJoy value";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(680, 298);
            this.Controls.Add(this.splitContainerMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "vJoyIOFeeder";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
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
        private System.Windows.Forms.Button btnConfigureHardware;
        private LiveCharts.WinForms.AngularGauge axesJoyGauge;
        private System.Windows.Forms.Button btnShowLogWindow;
        private System.Windows.Forms.Button btnAxisMappingEditor;
        private System.Windows.Forms.Button btnButtons;
        private System.Windows.Forms.Button btnOutputs;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button btnTuneEffects;
        private System.Windows.Forms.Button btnControlSets;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbConfigSet;
        private System.Windows.Forms.Label lblCurrentGame;
    }
}

