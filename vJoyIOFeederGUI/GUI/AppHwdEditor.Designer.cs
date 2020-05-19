namespace vJoyIOFeederGUI.GUI
{
    partial class AppHwdEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppHwdEditor));
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.cmbSelectMode = new System.Windows.Forms.ComboBox();
            this.btnStartStopManager = new System.Windows.Forms.Button();
            this.btnOpenvJoyConfig = new System.Windows.Forms.Button();
            this.btnOpenvJoyMonitor = new System.Windows.Forms.Button();
            this.btnOpenJoyCPL = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chkOutputOnly = new System.Windows.Forms.CheckBox();
            this.chkDumpLogToFile = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkAutodetectControlSet = new System.Windows.Forms.CheckBox();
            this.chkBoxStartWithWindows = new System.Windows.Forms.CheckBox();
            this.chkBoxStartMinimized = new System.Windows.Forms.CheckBox();
            this.btnDebugMode = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chkDigitalPWM = new System.Windows.Forms.CheckBox();
            this.btnCommit = new System.Windows.Forms.Button();
            this.chkDualModePWM = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbBaudrate = new System.Windows.Forms.ComboBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtWheelCenter = new System.Windows.Forms.TextBox();
            this.chkInvertTorque = new System.Windows.Forms.CheckBox();
            this.txtWheelScale = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkInvertWheel = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelWheelCenter = new System.Windows.Forms.Label();
            this.btnDeviceReady = new System.Windows.Forms.Button();
            this.labelWheelScale = new System.Windows.Forms.Label();
            this.btnWheelCalibrate = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.cmbSelectMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectMode.FormattingEnabled = true;
            this.cmbSelectMode.Location = new System.Drawing.Point(8, 39);
            this.cmbSelectMode.Name = "cmbSelectMode";
            this.cmbSelectMode.Size = new System.Drawing.Size(121, 21);
            this.cmbSelectMode.TabIndex = 18;
            this.cmbSelectMode.SelectedIndexChanged += new System.EventHandler(this.cmbSelectMode_SelectedIndexChanged);
            // 
            // btnStartStopManager
            // 
            this.btnStartStopManager.Location = new System.Drawing.Point(139, 39);
            this.btnStartStopManager.Name = "btnStartStopManager";
            this.btnStartStopManager.Size = new System.Drawing.Size(121, 21);
            this.btnStartStopManager.TabIndex = 17;
            this.btnStartStopManager.Text = "--";
            this.btnStartStopManager.UseVisualStyleBackColor = true;
            this.btnStartStopManager.Click += new System.EventHandler(this.btnStartStopManager_Click);
            // 
            // btnOpenvJoyConfig
            // 
            this.btnOpenvJoyConfig.Location = new System.Drawing.Point(467, 12);
            this.btnOpenvJoyConfig.Name = "btnOpenvJoyConfig";
            this.btnOpenvJoyConfig.Size = new System.Drawing.Size(105, 21);
            this.btnOpenvJoyConfig.TabIndex = 15;
            this.btnOpenvJoyConfig.Text = "Open vJoy Conf";
            this.btnOpenvJoyConfig.UseVisualStyleBackColor = true;
            this.btnOpenvJoyConfig.Click += new System.EventHandler(this.btnOpenvJoyConfig_Click);
            // 
            // btnOpenvJoyMonitor
            // 
            this.btnOpenvJoyMonitor.Location = new System.Drawing.Point(467, 39);
            this.btnOpenvJoyMonitor.Name = "btnOpenvJoyMonitor";
            this.btnOpenvJoyMonitor.Size = new System.Drawing.Size(105, 21);
            this.btnOpenvJoyMonitor.TabIndex = 14;
            this.btnOpenvJoyMonitor.Text = "Open vJoy Monitor";
            this.btnOpenvJoyMonitor.UseVisualStyleBackColor = true;
            this.btnOpenvJoyMonitor.Click += new System.EventHandler(this.btnOpenvJoyMonitor_Click);
            // 
            // btnOpenJoyCPL
            // 
            this.btnOpenJoyCPL.Location = new System.Drawing.Point(467, 66);
            this.btnOpenJoyCPL.Name = "btnOpenJoyCPL";
            this.btnOpenJoyCPL.Size = new System.Drawing.Size(105, 21);
            this.btnOpenJoyCPL.TabIndex = 13;
            this.btnOpenJoyCPL.Text = "Open Joy.cpl";
            this.btnOpenJoyCPL.UseVisualStyleBackColor = true;
            this.btnOpenJoyCPL.Click += new System.EventHandler(this.btnOpenJoyCPL_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Target hardware :";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chkOutputOnly);
            this.splitContainer1.Panel1.Controls.Add(this.chkDumpLogToFile);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.chkAutodetectControlSet);
            this.splitContainer1.Panel1.Controls.Add(this.chkBoxStartWithWindows);
            this.splitContainer1.Panel1.Controls.Add(this.chkBoxStartMinimized);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenvJoyMonitor);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenvJoyConfig);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenJoyCPL);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnDebugMode);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.label7);
            this.splitContainer1.Panel2.Controls.Add(this.chkDigitalPWM);
            this.splitContainer1.Panel2.Controls.Add(this.btnCommit);
            this.splitContainer1.Panel2.Controls.Add(this.chkDualModePWM);
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.cmbBaudrate);
            this.splitContainer1.Panel2.Controls.Add(this.btnReset);
            this.splitContainer1.Panel2.Controls.Add(this.btnClose);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.txtWheelCenter);
            this.splitContainer1.Panel2.Controls.Add(this.chkInvertTorque);
            this.splitContainer1.Panel2.Controls.Add(this.txtWheelScale);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.chkInvertWheel);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.labelWheelCenter);
            this.splitContainer1.Panel2.Controls.Add(this.btnDeviceReady);
            this.splitContainer1.Panel2.Controls.Add(this.labelWheelScale);
            this.splitContainer1.Panel2.Controls.Add(this.cmbSelectMode);
            this.splitContainer1.Panel2.Controls.Add(this.btnWheelCalibrate);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.btnStartStopManager);
            this.splitContainer1.Size = new System.Drawing.Size(584, 385);
            this.splitContainer1.SplitterDistance = 98;
            this.splitContainer1.TabIndex = 20;
            // 
            // chkOutputOnly
            // 
            this.chkOutputOnly.AutoSize = true;
            this.chkOutputOnly.Location = new System.Drawing.Point(197, 55);
            this.chkOutputOnly.Name = "chkOutputOnly";
            this.chkOutputOnly.Size = new System.Drawing.Size(212, 17);
            this.chkOutputOnly.TabIndex = 42;
            this.chkOutputOnly.Text = "Lights mode only (must restart manager)";
            this.chkOutputOnly.UseVisualStyleBackColor = true;
            this.chkOutputOnly.Click += new System.EventHandler(this.chkOutputOnly_Click);
            // 
            // chkDumpLogToFile
            // 
            this.chkDumpLogToFile.AutoSize = true;
            this.chkDumpLogToFile.Location = new System.Drawing.Point(197, 35);
            this.chkDumpLogToFile.Name = "chkDumpLogToFile";
            this.chkDumpLogToFile.Size = new System.Drawing.Size(165, 17);
            this.chkDumpLogToFile.TabIndex = 41;
            this.chkDumpLogToFile.Text = "Dump log to file (need reboot)";
            this.chkDumpLogToFile.UseVisualStyleBackColor = true;
            this.chkDumpLogToFile.Click += new System.EventHandler(this.chkDumpLogToFile_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(169, 16);
            this.label5.TabIndex = 40;
            this.label5.Text = "Application parameters";
            // 
            // chkAutodetectControlSet
            // 
            this.chkAutodetectControlSet.AutoSize = true;
            this.chkAutodetectControlSet.Location = new System.Drawing.Point(9, 75);
            this.chkAutodetectControlSet.Name = "chkAutodetectControlSet";
            this.chkAutodetectControlSet.Size = new System.Drawing.Size(182, 17);
            this.chkAutodetectControlSet.TabIndex = 40;
            this.chkAutodetectControlSet.Text = "Auto detect control set at runtime";
            this.chkAutodetectControlSet.UseVisualStyleBackColor = true;
            this.chkAutodetectControlSet.Click += new System.EventHandler(this.chkAutodetectControlSet_Click);
            // 
            // chkBoxStartWithWindows
            // 
            this.chkBoxStartWithWindows.AutoSize = true;
            this.chkBoxStartWithWindows.Location = new System.Drawing.Point(9, 35);
            this.chkBoxStartWithWindows.Name = "chkBoxStartWithWindows";
            this.chkBoxStartWithWindows.Size = new System.Drawing.Size(117, 17);
            this.chkBoxStartWithWindows.TabIndex = 34;
            this.chkBoxStartWithWindows.Text = "Start with Windows";
            this.chkBoxStartWithWindows.UseVisualStyleBackColor = true;
            this.chkBoxStartWithWindows.Click += new System.EventHandler(this.chkBoxStartWithWindows_Click);
            // 
            // chkBoxStartMinimized
            // 
            this.chkBoxStartMinimized.AutoSize = true;
            this.chkBoxStartMinimized.Location = new System.Drawing.Point(9, 55);
            this.chkBoxStartMinimized.Name = "chkBoxStartMinimized";
            this.chkBoxStartMinimized.Size = new System.Drawing.Size(175, 17);
            this.chkBoxStartMinimized.TabIndex = 33;
            this.chkBoxStartMinimized.Text = "Minimized window when started";
            this.chkBoxStartMinimized.UseVisualStyleBackColor = true;
            this.chkBoxStartMinimized.Click += new System.EventHandler(this.chkBoxStartMinimized_Click);
            // 
            // btnDebugMode
            // 
            this.btnDebugMode.Location = new System.Drawing.Point(306, 67);
            this.btnDebugMode.Name = "btnDebugMode";
            this.btnDebugMode.Size = new System.Drawing.Size(121, 21);
            this.btnDebugMode.TabIndex = 50;
            this.btnDebugMode.Text = "DebugMode";
            this.btnDebugMode.UseVisualStyleBackColor = true;
            this.btnDebugMode.Visible = false;
            this.btnDebugMode.Click += new System.EventHandler(this.btnDebugMode_Click);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(8, 167);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(561, 84);
            this.label9.TabIndex = 49;
            this.label9.Text = resources.GetString("label9.Text");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(138, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 13);
            this.label7.TabIndex = 48;
            this.label7.Text = "(Click it to stop manager)";
            // 
            // chkDigitalPWM
            // 
            this.chkDigitalPWM.AutoSize = true;
            this.chkDigitalPWM.Location = new System.Drawing.Point(269, 151);
            this.chkDigitalPWM.Name = "chkDigitalPWM";
            this.chkDigitalPWM.Size = new System.Drawing.Size(314, 17);
            this.chkDigitalPWM.TabIndex = 47;
            this.chkDigitalPWM.Text = "Digital PWM (FFB Converter - PWM2M2, need commit&&reset)";
            this.chkDigitalPWM.UseVisualStyleBackColor = true;
            this.chkDigitalPWM.Click += new System.EventHandler(this.chkDigitalPWM_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Location = new System.Drawing.Point(440, 66);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(128, 21);
            this.btnCommit.TabIndex = 46;
            this.btnCommit.Text = "Commit eeprom && reset";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // chkDualModePWM
            // 
            this.chkDualModePWM.AutoSize = true;
            this.chkDualModePWM.Location = new System.Drawing.Point(9, 151);
            this.chkDualModePWM.Name = "chkDualModePWM";
            this.chkDualModePWM.Size = new System.Drawing.Size(147, 17);
            this.chkDualModePWM.TabIndex = 45;
            this.chkDualModePWM.Text = "Dual mode PWM (L620X)";
            this.chkDualModePWM.UseVisualStyleBackColor = true;
            this.chkDualModePWM.Click += new System.EventHandler(this.chkDualModePWM_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(306, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Serial port baudrate";
            // 
            // cmbBaudrate
            // 
            this.cmbBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaudrate.FormattingEnabled = true;
            this.cmbBaudrate.Location = new System.Drawing.Point(306, 40);
            this.cmbBaudrate.Name = "cmbBaudrate";
            this.cmbBaudrate.Size = new System.Drawing.Size(121, 21);
            this.cmbBaudrate.TabIndex = 43;
            this.cmbBaudrate.SelectedIndexChanged += new System.EventHandler(this.cmbBaudrate_SelectedIndexChanged);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(8, 253);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(94, 21);
            this.btnReset.TabIndex = 42;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(474, 253);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 21);
            this.btnClose.TabIndex = 41;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(503, 16);
            this.label4.TabIndex = 23;
            this.label4.Text = "Hardware parameters (some may be changed when manager is running)";
            // 
            // txtWheelCenter
            // 
            this.txtWheelCenter.Location = new System.Drawing.Point(307, 101);
            this.txtWheelCenter.Name = "txtWheelCenter";
            this.txtWheelCenter.Size = new System.Drawing.Size(86, 20);
            this.txtWheelCenter.TabIndex = 39;
            this.txtWheelCenter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWheelCenter_KeyPress);
            // 
            // chkInvertTorque
            // 
            this.chkInvertTorque.AutoSize = true;
            this.chkInvertTorque.Location = new System.Drawing.Point(269, 128);
            this.chkInvertTorque.Name = "chkInvertTorque";
            this.chkInvertTorque.Size = new System.Drawing.Size(190, 17);
            this.chkInvertTorque.TabIndex = 1;
            this.chkInvertTorque.Text = "Invert Torque (change torque sign)";
            this.chkInvertTorque.UseVisualStyleBackColor = true;
            this.chkInvertTorque.Click += new System.EventHandler(this.chkInvertTorque_Click);
            // 
            // txtWheelScale
            // 
            this.txtWheelScale.Location = new System.Drawing.Point(174, 101);
            this.txtWheelScale.Name = "txtWheelScale";
            this.txtWheelScale.Size = new System.Drawing.Size(86, 20);
            this.txtWheelScale.TabIndex = 34;
            this.txtWheelScale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtWheelScale_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(139, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Manager status";
            // 
            // chkInvertWheel
            // 
            this.chkInvertWheel.AutoSize = true;
            this.chkInvertWheel.Location = new System.Drawing.Point(9, 128);
            this.chkInvertWheel.Name = "chkInvertWheel";
            this.chkInvertWheel.Size = new System.Drawing.Size(230, 17);
            this.chkInvertWheel.TabIndex = 0;
            this.chkInvertWheel.Text = "Invert Wheel Direction (change wheel sign)";
            this.chkInvertWheel.UseVisualStyleBackColor = true;
            this.chkInvertWheel.Click += new System.EventHandler(this.chkInvertWheel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(440, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Slave device status";
            // 
            // labelWheelCenter
            // 
            this.labelWheelCenter.AutoSize = true;
            this.labelWheelCenter.Location = new System.Drawing.Point(271, 104);
            this.labelWheelCenter.Name = "labelWheelCenter";
            this.labelWheelCenter.Size = new System.Drawing.Size(38, 13);
            this.labelWheelCenter.TabIndex = 38;
            this.labelWheelCenter.Text = "Center";
            // 
            // btnDeviceReady
            // 
            this.btnDeviceReady.Enabled = false;
            this.btnDeviceReady.Location = new System.Drawing.Point(440, 39);
            this.btnDeviceReady.Name = "btnDeviceReady";
            this.btnDeviceReady.Size = new System.Drawing.Size(128, 21);
            this.btnDeviceReady.TabIndex = 20;
            this.btnDeviceReady.Text = "--";
            this.btnDeviceReady.UseVisualStyleBackColor = true;
            // 
            // labelWheelScale
            // 
            this.labelWheelScale.AutoSize = true;
            this.labelWheelScale.Location = new System.Drawing.Point(138, 104);
            this.labelWheelScale.Name = "labelWheelScale";
            this.labelWheelScale.Size = new System.Drawing.Size(34, 13);
            this.labelWheelScale.TabIndex = 37;
            this.labelWheelScale.Text = "Scale";
            // 
            // btnWheelCalibrate
            // 
            this.btnWheelCalibrate.Location = new System.Drawing.Point(8, 101);
            this.btnWheelCalibrate.Name = "btnWheelCalibrate";
            this.btnWheelCalibrate.Size = new System.Drawing.Size(121, 21);
            this.btnWheelCalibrate.TabIndex = 35;
            this.btnWheelCalibrate.Text = "Calibrate wheel";
            this.btnWheelCalibrate.UseVisualStyleBackColor = true;
            this.btnWheelCalibrate.Click += new System.EventHandler(this.btnWheelCalibrate_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 85);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 13);
            this.label8.TabIndex = 36;
            this.label8.Text = "Wheel scale and center";
            // 
            // AppHwdEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(584, 385);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AppHwdEditor";
            this.Text = "Application and Hardware editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TargetHdwForm_FormClosed);
            this.Load += new System.EventHandler(this.TargetHdwForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox chkInvertTorque;
        private System.Windows.Forms.CheckBox chkInvertWheel;
        private System.Windows.Forms.Button btnDeviceReady;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkBoxStartMinimized;
        private System.Windows.Forms.CheckBox chkBoxStartWithWindows;
        private System.Windows.Forms.Label labelWheelCenter;
        private System.Windows.Forms.Label labelWheelScale;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnWheelCalibrate;
        private System.Windows.Forms.TextBox txtWheelCenter;
        private System.Windows.Forms.TextBox txtWheelScale;
        private System.Windows.Forms.CheckBox chkAutodetectControlSet;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cmbBaudrate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkDumpLogToFile;
        private System.Windows.Forms.CheckBox chkDualModePWM;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.CheckBox chkDigitalPWM;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnDebugMode;
        private System.Windows.Forms.CheckBox chkOutputOnly;
    }
}

