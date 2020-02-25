namespace IOFeederGUI.GUI
{
    partial class TargetHdwForm
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
            this.cmbSelectMode = new System.Windows.Forms.ComboBox();
            this.btnStartStopManager = new System.Windows.Forms.Button();
            this.btnOpenvJoyConfig = new System.Windows.Forms.Button();
            this.btnOpenvJoyMonitor = new System.Windows.Forms.Button();
            this.btnOpenJoyCPL = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDeviceReady = new System.Windows.Forms.Button();
            this.txtPowerLaw = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbPowerLaw = new System.Windows.Forms.TrackBar();
            this.chkSkipStopEffect = new System.Windows.Forms.CheckBox();
            this.txtGlobalGain = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbGlobalGain = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.chkEmulateMissing = new System.Windows.Forms.CheckBox();
            this.chkPulsedTrq = new System.Windows.Forms.CheckBox();
            this.chkInvertTorque = new System.Windows.Forms.CheckBox();
            this.chkInvertWheel = new System.Windows.Forms.CheckBox();
            this.txtTrqDeadBand = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTrqDeadBand = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).BeginInit();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 500;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // cmbSelectMode
            // 
            this.cmbSelectMode.AllowDrop = true;
            this.cmbSelectMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectMode.FormattingEnabled = true;
            this.cmbSelectMode.Location = new System.Drawing.Point(12, 54);
            this.cmbSelectMode.Name = "cmbSelectMode";
            this.cmbSelectMode.Size = new System.Drawing.Size(121, 21);
            this.cmbSelectMode.TabIndex = 18;
            this.cmbSelectMode.SelectedIndexChanged += new System.EventHandler(this.cmbSelectMode_SelectedIndexChanged);
            // 
            // btnStartStopManager
            // 
            this.btnStartStopManager.Location = new System.Drawing.Point(143, 54);
            this.btnStartStopManager.Name = "btnStartStopManager";
            this.btnStartStopManager.Size = new System.Drawing.Size(121, 21);
            this.btnStartStopManager.TabIndex = 17;
            this.btnStartStopManager.UseVisualStyleBackColor = true;
            this.btnStartStopManager.Click += new System.EventHandler(this.btnStartStopManager_Click);
            // 
            // btnOpenvJoyConfig
            // 
            this.btnOpenvJoyConfig.Location = new System.Drawing.Point(276, 12);
            this.btnOpenvJoyConfig.Name = "btnOpenvJoyConfig";
            this.btnOpenvJoyConfig.Size = new System.Drawing.Size(121, 21);
            this.btnOpenvJoyConfig.TabIndex = 15;
            this.btnOpenvJoyConfig.Text = "Open vJoy Conf";
            this.btnOpenvJoyConfig.UseVisualStyleBackColor = true;
            this.btnOpenvJoyConfig.Click += new System.EventHandler(this.btnOpenvJoyConfig_Click);
            // 
            // btnOpenvJoyMonitor
            // 
            this.btnOpenvJoyMonitor.Location = new System.Drawing.Point(143, 12);
            this.btnOpenvJoyMonitor.Name = "btnOpenvJoyMonitor";
            this.btnOpenvJoyMonitor.Size = new System.Drawing.Size(121, 21);
            this.btnOpenvJoyMonitor.TabIndex = 14;
            this.btnOpenvJoyMonitor.Text = "Open vJoy Monitor";
            this.btnOpenvJoyMonitor.UseVisualStyleBackColor = true;
            this.btnOpenvJoyMonitor.Click += new System.EventHandler(this.btnOpenvJoyMonitor_Click);
            // 
            // btnOpenJoyCPL
            // 
            this.btnOpenJoyCPL.Location = new System.Drawing.Point(12, 12);
            this.btnOpenJoyCPL.Name = "btnOpenJoyCPL";
            this.btnOpenJoyCPL.Size = new System.Drawing.Size(121, 21);
            this.btnOpenJoyCPL.TabIndex = 13;
            this.btnOpenJoyCPL.Text = "Open Joy.cpl";
            this.btnOpenJoyCPL.UseVisualStyleBackColor = true;
            this.btnOpenJoyCPL.Click += new System.EventHandler(this.btnOpenJoyCPL_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Target hardware :";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.btnDeviceReady);
            this.splitContainer1.Panel1.Controls.Add(this.cmbSelectMode);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenvJoyMonitor);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenvJoyConfig);
            this.splitContainer1.Panel1.Controls.Add(this.btnStartStopManager);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenJoyCPL);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtTrqDeadBand);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.tbTrqDeadBand);
            this.splitContainer1.Panel2.Controls.Add(this.txtPowerLaw);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.tbPowerLaw);
            this.splitContainer1.Panel2.Controls.Add(this.chkSkipStopEffect);
            this.splitContainer1.Panel2.Controls.Add(this.txtGlobalGain);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.tbGlobalGain);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.chkEmulateMissing);
            this.splitContainer1.Panel2.Controls.Add(this.chkPulsedTrq);
            this.splitContainer1.Panel2.Controls.Add(this.chkInvertTorque);
            this.splitContainer1.Panel2.Controls.Add(this.chkInvertWheel);
            this.splitContainer1.Size = new System.Drawing.Size(634, 436);
            this.splitContainer1.SplitterDistance = 125;
            this.splitContainer1.TabIndex = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(143, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Manager status";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(276, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Device status";
            // 
            // btnDeviceReady
            // 
            this.btnDeviceReady.Enabled = false;
            this.btnDeviceReady.Location = new System.Drawing.Point(276, 54);
            this.btnDeviceReady.Name = "btnDeviceReady";
            this.btnDeviceReady.Size = new System.Drawing.Size(121, 21);
            this.btnDeviceReady.TabIndex = 20;
            this.btnDeviceReady.Text = "--";
            this.btnDeviceReady.UseVisualStyleBackColor = true;
            // 
            // txtPowerLaw
            // 
            this.txtPowerLaw.Location = new System.Drawing.Point(540, 54);
            this.txtPowerLaw.Name = "txtPowerLaw";
            this.txtPowerLaw.Size = new System.Drawing.Size(57, 20);
            this.txtPowerLaw.TabIndex = 29;
            this.txtPowerLaw.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPowerLaw_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(364, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Power law";
            // 
            // tbPowerLaw
            // 
            this.tbPowerLaw.Location = new System.Drawing.Point(430, 52);
            this.tbPowerLaw.Maximum = 30;
            this.tbPowerLaw.Minimum = 1;
            this.tbPowerLaw.Name = "tbPowerLaw";
            this.tbPowerLaw.Size = new System.Drawing.Size(104, 45);
            this.tbPowerLaw.TabIndex = 27;
            this.tbPowerLaw.TickFrequency = 3;
            this.tbPowerLaw.Value = 10;
            this.tbPowerLaw.Scroll += new System.EventHandler(this.tbPowerLaw_Scroll);
            // 
            // chkSkipStopEffect
            // 
            this.chkSkipStopEffect.AutoSize = true;
            this.chkSkipStopEffect.Location = new System.Drawing.Point(12, 18);
            this.chkSkipStopEffect.Name = "chkSkipStopEffect";
            this.chkSkipStopEffect.Size = new System.Drawing.Size(108, 17);
            this.chkSkipStopEffect.TabIndex = 26;
            this.chkSkipStopEffect.Text = "Skip Stop Effects";
            this.chkSkipStopEffect.UseVisualStyleBackColor = true;
            this.chkSkipStopEffect.Click += new System.EventHandler(this.chkSkipStopEffect_Click);
            // 
            // txtGlobalGain
            // 
            this.txtGlobalGain.Location = new System.Drawing.Point(540, 17);
            this.txtGlobalGain.Name = "txtGlobalGain";
            this.txtGlobalGain.Size = new System.Drawing.Size(57, 20);
            this.txtGlobalGain.TabIndex = 25;
            this.txtGlobalGain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGlobalGain_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(364, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Global gain";
            // 
            // tbGlobalGain
            // 
            this.tbGlobalGain.Location = new System.Drawing.Point(430, 15);
            this.tbGlobalGain.Maximum = 30;
            this.tbGlobalGain.Minimum = 1;
            this.tbGlobalGain.Name = "tbGlobalGain";
            this.tbGlobalGain.Size = new System.Drawing.Size(104, 45);
            this.tbGlobalGain.TabIndex = 23;
            this.tbGlobalGain.TickFrequency = 3;
            this.tbGlobalGain.Value = 10;
            this.tbGlobalGain.Scroll += new System.EventHandler(this.tbGlobalGain_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(334, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Internal parameters (some may be changed when manager is running)";
            // 
            // chkEmulateMissing
            // 
            this.chkEmulateMissing.AutoSize = true;
            this.chkEmulateMissing.Enabled = false;
            this.chkEmulateMissing.Location = new System.Drawing.Point(12, 178);
            this.chkEmulateMissing.Name = "chkEmulateMissing";
            this.chkEmulateMissing.Size = new System.Drawing.Size(296, 17);
            this.chkEmulateMissing.TabIndex = 3;
            this.chkEmulateMissing.Text = "Emulated missing effects (translated to torque commands)";
            this.chkEmulateMissing.UseVisualStyleBackColor = true;
            this.chkEmulateMissing.Click += new System.EventHandler(this.chkEmulateMissing_Click);
            // 
            // chkPulsedTrq
            // 
            this.chkPulsedTrq.AutoSize = true;
            this.chkPulsedTrq.Enabled = false;
            this.chkPulsedTrq.Location = new System.Drawing.Point(12, 155);
            this.chkPulsedTrq.Name = "chkPulsedTrq";
            this.chkPulsedTrq.Size = new System.Drawing.Size(380, 17);
            this.chkPulsedTrq.TabIndex = 2;
            this.chkPulsedTrq.Text = "Use quarter-pulsed Torque (increase torque resolution but introduce ripples)";
            this.chkPulsedTrq.UseVisualStyleBackColor = true;
            this.chkPulsedTrq.Click += new System.EventHandler(this.chkPulsedTrq_Click);
            // 
            // chkInvertTorque
            // 
            this.chkInvertTorque.AutoSize = true;
            this.chkInvertTorque.Location = new System.Drawing.Point(12, 64);
            this.chkInvertTorque.Name = "chkInvertTorque";
            this.chkInvertTorque.Size = new System.Drawing.Size(190, 17);
            this.chkInvertTorque.TabIndex = 1;
            this.chkInvertTorque.Text = "Invert Torque (change torque sign)";
            this.chkInvertTorque.UseVisualStyleBackColor = true;
            this.chkInvertTorque.Click += new System.EventHandler(this.chkInvertTorque_Click);
            // 
            // chkInvertWheel
            // 
            this.chkInvertWheel.AutoSize = true;
            this.chkInvertWheel.Location = new System.Drawing.Point(12, 41);
            this.chkInvertWheel.Name = "chkInvertWheel";
            this.chkInvertWheel.Size = new System.Drawing.Size(230, 17);
            this.chkInvertWheel.TabIndex = 0;
            this.chkInvertWheel.Text = "Invert Wheel Direction (change wheel sign)";
            this.chkInvertWheel.UseVisualStyleBackColor = true;
            this.chkInvertWheel.Click += new System.EventHandler(this.chkInvertWheel_Click);
            // 
            // txtTrqDeadBand
            // 
            this.txtTrqDeadBand.Location = new System.Drawing.Point(540, 91);
            this.txtTrqDeadBand.Name = "txtTrqDeadBand";
            this.txtTrqDeadBand.Size = new System.Drawing.Size(57, 20);
            this.txtTrqDeadBand.TabIndex = 32;
            this.txtTrqDeadBand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrqDeadBand_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(364, 94);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Trq deadband";
            // 
            // tbTrqDeadBand
            // 
            this.tbTrqDeadBand.LargeChange = 10;
            this.tbTrqDeadBand.Location = new System.Drawing.Point(430, 89);
            this.tbTrqDeadBand.Maximum = 50;
            this.tbTrqDeadBand.Name = "tbTrqDeadBand";
            this.tbTrqDeadBand.Size = new System.Drawing.Size(104, 45);
            this.tbTrqDeadBand.TabIndex = 30;
            this.tbTrqDeadBand.TickFrequency = 5;
            this.tbTrqDeadBand.Scroll += new System.EventHandler(this.tbTrqDeadBand_Scroll);
            // 
            // TargetHdwForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(634, 436);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TargetHdwForm";
            this.Text = "vJoyIOFeeder";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TargetHdwForm_FormClosed);
            this.Load += new System.EventHandler(this.TargetHdwForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.Button btnOpenvJoyConfig;
        private System.Windows.Forms.Button btnOpenvJoyMonitor;
        private System.Windows.Forms.Button btnOpenJoyCPL;
        private System.Windows.Forms.Button btnStartStopManager;
        private System.Windows.Forms.ComboBox cmbSelectMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkEmulateMissing;
        private System.Windows.Forms.CheckBox chkPulsedTrq;
        private System.Windows.Forms.CheckBox chkInvertTorque;
        private System.Windows.Forms.CheckBox chkInvertWheel;
        private System.Windows.Forms.Button btnDeviceReady;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtGlobalGain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar tbGlobalGain;
        private System.Windows.Forms.CheckBox chkSkipStopEffect;
        private System.Windows.Forms.TextBox txtPowerLaw;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar tbPowerLaw;
        private System.Windows.Forms.TextBox txtTrqDeadBand;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar tbTrqDeadBand;
    }
}

