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
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tooltipContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuShow = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.slJoyAxis = new System.Windows.Forms.ProgressBar();
            this.slRawAxis = new System.Windows.Forms.ProgressBar();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.chkBtn1 = new System.Windows.Forms.CheckBox();
            this.chkBtn2 = new System.Windows.Forms.CheckBox();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerAxes = new System.Windows.Forms.SplitContainer();
            this.cmbSelectMode = new System.Windows.Forms.ComboBox();
            this.btnStartStopManager = new System.Windows.Forms.Button();
            this.axesJoyGauge = new LiveCharts.WinForms.AngularGauge();
            this.btnShowLogWindow = new System.Windows.Forms.Button();
            this.btnOpenvJoyConfig = new System.Windows.Forms.Button();
            this.btnOpenvJoyMonitor = new System.Windows.Forms.Button();
            this.btnOpenJoyCPL = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSelectedAxis = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtJoyAxisValue = new System.Windows.Forms.TextBox();
            this.btnAxisMappingEditor = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.txtRawAxisValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.splitContainerButtons = new System.Windows.Forms.SplitContainer();
            this.chkBtn8 = new System.Windows.Forms.CheckBox();
            this.chkBtn7 = new System.Windows.Forms.CheckBox();
            this.chkBtn6 = new System.Windows.Forms.CheckBox();
            this.chkBtn5 = new System.Windows.Forms.CheckBox();
            this.chkBtn4 = new System.Windows.Forms.CheckBox();
            this.chkBtn3 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbBtnMapFrom = new System.Windows.Forms.ComboBox();
            this.chkAutofire = new System.Windows.Forms.CheckBox();
            this.chkToggling = new System.Windows.Forms.CheckBox();
            this.cmbBtnMapTo = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tooltipContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAxes)).BeginInit();
            this.splitContainerAxes.Panel1.SuspendLayout();
            this.splitContainerAxes.Panel2.SuspendLayout();
            this.splitContainerAxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerButtons)).BeginInit();
            this.splitContainerButtons.Panel1.SuspendLayout();
            this.splitContainerButtons.Panel2.SuspendLayout();
            this.splitContainerButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon.ContextMenuStrip = this.tooltipContextMenuStrip;
            this.notifyIcon.Text = "vJoyIOFeeder";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // tooltipContextMenuStrip
            // 
            this.tooltipContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuShow,
            this.menuExit});
            this.tooltipContextMenuStrip.Name = "tooltipContextMenuStrip";
            this.tooltipContextMenuStrip.Size = new System.Drawing.Size(104, 48);
            // 
            // menuShow
            // 
            this.menuShow.Name = "menuShow";
            this.menuShow.Size = new System.Drawing.Size(103, 22);
            this.menuShow.Text = "Show";
            this.menuShow.Click += new System.EventHandler(this.menuShow_Click);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.Size = new System.Drawing.Size(103, 22);
            this.menuExit.Text = "Exit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // slJoyAxis
            // 
            this.slJoyAxis.Location = new System.Drawing.Point(170, 135);
            this.slJoyAxis.Maximum = 255;
            this.slJoyAxis.Name = "slJoyAxis";
            this.slJoyAxis.Size = new System.Drawing.Size(79, 20);
            this.slJoyAxis.Step = 255;
            this.slJoyAxis.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.slJoyAxis.TabIndex = 1;
            // 
            // slRawAxis
            // 
            this.slRawAxis.Location = new System.Drawing.Point(196, 9);
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
            // chkBtn1
            // 
            this.chkBtn1.AutoSize = true;
            this.chkBtn1.Enabled = false;
            this.chkBtn1.Location = new System.Drawing.Point(6, 21);
            this.chkBtn1.Name = "chkBtn1";
            this.chkBtn1.Size = new System.Drawing.Size(32, 17);
            this.chkBtn1.TabIndex = 3;
            this.chkBtn1.Text = "1";
            this.chkBtn1.UseVisualStyleBackColor = true;
            // 
            // chkBtn2
            // 
            this.chkBtn2.AutoSize = true;
            this.chkBtn2.Enabled = false;
            this.chkBtn2.Location = new System.Drawing.Point(6, 44);
            this.chkBtn2.Name = "chkBtn2";
            this.chkBtn2.Size = new System.Drawing.Size(32, 17);
            this.chkBtn2.TabIndex = 4;
            this.chkBtn2.Text = "2";
            this.chkBtn2.UseVisualStyleBackColor = true;
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
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerAxes);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.splitContainerButtons);
            this.splitContainerMain.Size = new System.Drawing.Size(776, 426);
            this.splitContainerMain.SplitterDistance = 200;
            this.splitContainerMain.TabIndex = 6;
            // 
            // splitContainerAxes
            // 
            this.splitContainerAxes.Location = new System.Drawing.Point(3, 3);
            this.splitContainerAxes.Name = "splitContainerAxes";
            // 
            // splitContainerAxes.Panel1
            // 
            this.splitContainerAxes.Panel1.Controls.Add(this.cmbSelectMode);
            this.splitContainerAxes.Panel1.Controls.Add(this.btnStartStopManager);
            this.splitContainerAxes.Panel1.Controls.Add(this.axesJoyGauge);
            this.splitContainerAxes.Panel1.Controls.Add(this.btnShowLogWindow);
            this.splitContainerAxes.Panel1.Controls.Add(this.btnOpenvJoyConfig);
            this.splitContainerAxes.Panel1.Controls.Add(this.btnOpenvJoyMonitor);
            this.splitContainerAxes.Panel1.Controls.Add(this.btnOpenJoyCPL);
            this.splitContainerAxes.Panel1.Controls.Add(this.label2);
            this.splitContainerAxes.Panel1.Controls.Add(this.cmbSelectedAxis);
            this.splitContainerAxes.Panel1.Controls.Add(this.slJoyAxis);
            this.splitContainerAxes.Panel1.Controls.Add(this.label6);
            this.splitContainerAxes.Panel1.Controls.Add(this.txtJoyAxisValue);
            // 
            // splitContainerAxes.Panel2
            // 
            this.splitContainerAxes.Panel2.Controls.Add(this.btnAxisMappingEditor);
            this.splitContainerAxes.Panel2.Controls.Add(this.label7);
            this.splitContainerAxes.Panel2.Controls.Add(this.label8);
            this.splitContainerAxes.Panel2.Controls.Add(this.textBox4);
            this.splitContainerAxes.Panel2.Controls.Add(this.label5);
            this.splitContainerAxes.Panel2.Controls.Add(this.textBox5);
            this.splitContainerAxes.Panel2.Controls.Add(this.txtRawAxisValue);
            this.splitContainerAxes.Panel2.Controls.Add(this.label4);
            this.splitContainerAxes.Panel2.Controls.Add(this.slRawAxis);
            this.splitContainerAxes.Size = new System.Drawing.Size(770, 194);
            this.splitContainerAxes.SplitterDistance = 427;
            this.splitContainerAxes.TabIndex = 17;
            // 
            // cmbSelectMode
            // 
            this.cmbSelectMode.AllowDrop = true;
            this.cmbSelectMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelectMode.FormattingEnabled = true;
            this.cmbSelectMode.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Rx",
            "Ry",
            "Rz",
            "Sl0",
            "Sl1/Dial"});
            this.cmbSelectMode.Location = new System.Drawing.Point(3, 61);
            this.cmbSelectMode.Name = "cmbSelectMode";
            this.cmbSelectMode.Size = new System.Drawing.Size(121, 21);
            this.cmbSelectMode.TabIndex = 18;
            this.cmbSelectMode.SelectedIndexChanged += new System.EventHandler(this.cmbSelectMode_SelectedIndexChanged);
            // 
            // btnStartStopManager
            // 
            this.btnStartStopManager.Location = new System.Drawing.Point(128, 61);
            this.btnStartStopManager.Name = "btnStartStopManager";
            this.btnStartStopManager.Size = new System.Drawing.Size(121, 23);
            this.btnStartStopManager.TabIndex = 17;
            this.btnStartStopManager.UseVisualStyleBackColor = true;
            this.btnStartStopManager.Click += new System.EventHandler(this.btnStartStopManager_Click);
            // 
            // axesJoyGauge
            // 
            this.axesJoyGauge.Location = new System.Drawing.Point(311, 80);
            this.axesJoyGauge.Name = "axesJoyGauge";
            this.axesJoyGauge.Size = new System.Drawing.Size(113, 94);
            this.axesJoyGauge.TabIndex = 17;
            this.axesJoyGauge.Text = "angularGaugeAxis";
            // 
            // btnShowLogWindow
            // 
            this.btnShowLogWindow.Location = new System.Drawing.Point(3, 32);
            this.btnShowLogWindow.Name = "btnShowLogWindow";
            this.btnShowLogWindow.Size = new System.Drawing.Size(121, 23);
            this.btnShowLogWindow.TabIndex = 16;
            this.btnShowLogWindow.Text = "Log window";
            this.btnShowLogWindow.UseVisualStyleBackColor = true;
            this.btnShowLogWindow.Click += new System.EventHandler(this.btnShowLogWindow_Click);
            // 
            // btnOpenvJoyConfig
            // 
            this.btnOpenvJoyConfig.Location = new System.Drawing.Point(128, 32);
            this.btnOpenvJoyConfig.Name = "btnOpenvJoyConfig";
            this.btnOpenvJoyConfig.Size = new System.Drawing.Size(121, 23);
            this.btnOpenvJoyConfig.TabIndex = 15;
            this.btnOpenvJoyConfig.Text = "Open vJoy Conf";
            this.btnOpenvJoyConfig.UseVisualStyleBackColor = true;
            this.btnOpenvJoyConfig.Click += new System.EventHandler(this.btnOpenvJoyConfig_Click);
            // 
            // btnOpenvJoyMonitor
            // 
            this.btnOpenvJoyMonitor.Location = new System.Drawing.Point(128, 3);
            this.btnOpenvJoyMonitor.Name = "btnOpenvJoyMonitor";
            this.btnOpenvJoyMonitor.Size = new System.Drawing.Size(121, 23);
            this.btnOpenvJoyMonitor.TabIndex = 14;
            this.btnOpenvJoyMonitor.Text = "Open vJoy Monitor";
            this.btnOpenvJoyMonitor.UseVisualStyleBackColor = true;
            this.btnOpenvJoyMonitor.Click += new System.EventHandler(this.btnOpenvJoyMonitor_Click);
            // 
            // btnOpenJoyCPL
            // 
            this.btnOpenJoyCPL.Location = new System.Drawing.Point(3, 3);
            this.btnOpenJoyCPL.Name = "btnOpenJoyCPL";
            this.btnOpenJoyCPL.Size = new System.Drawing.Size(121, 23);
            this.btnOpenJoyCPL.TabIndex = 13;
            this.btnOpenJoyCPL.Text = "Open Joy.cpl";
            this.btnOpenJoyCPL.UseVisualStyleBackColor = true;
            this.btnOpenJoyCPL.Click += new System.EventHandler(this.btnOpenJoyCPL_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Selected axis";
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
            this.cmbSelectedAxis.Location = new System.Drawing.Point(77, 105);
            this.cmbSelectedAxis.Name = "cmbSelectedAxis";
            this.cmbSelectedAxis.Size = new System.Drawing.Size(89, 21);
            this.cmbSelectedAxis.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "vJoy value";
            // 
            // txtJoyAxisValue
            // 
            this.txtJoyAxisValue.Location = new System.Drawing.Point(77, 132);
            this.txtJoyAxisValue.Name = "txtJoyAxisValue";
            this.txtJoyAxisValue.Size = new System.Drawing.Size(89, 20);
            this.txtJoyAxisValue.TabIndex = 12;
            // 
            // btnAxisMappingEditor
            // 
            this.btnAxisMappingEditor.Location = new System.Drawing.Point(8, 119);
            this.btnAxisMappingEditor.Name = "btnAxisMappingEditor";
            this.btnAxisMappingEditor.Size = new System.Drawing.Size(121, 23);
            this.btnAxisMappingEditor.TabIndex = 18;
            this.btnAxisMappingEditor.Text = "Axis mapping Editor";
            this.btnAxisMappingEditor.UseVisualStyleBackColor = true;
            this.btnAxisMappingEditor.Click += new System.EventHandler(this.btnAxisMappingEditor_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Min sat";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(165, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Max sat";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(57, 80);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 20);
            this.textBox4.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Scale function";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(212, 80);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 20);
            this.textBox5.TabIndex = 15;
            // 
            // txtRawAxisValue
            // 
            this.txtRawAxisValue.Location = new System.Drawing.Point(90, 9);
            this.txtRawAxisValue.Name = "txtRawAxisValue";
            this.txtRawAxisValue.Size = new System.Drawing.Size(100, 20);
            this.txtRawAxisValue.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Raw value";
            // 
            // splitContainerButtons
            // 
            this.splitContainerButtons.Location = new System.Drawing.Point(3, 3);
            this.splitContainerButtons.Name = "splitContainerButtons";
            // 
            // splitContainerButtons.Panel1
            // 
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn8);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn7);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn6);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn5);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn4);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn3);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn2);
            this.splitContainerButtons.Panel1.Controls.Add(this.label3);
            this.splitContainerButtons.Panel1.Controls.Add(this.chkBtn1);
            // 
            // splitContainerButtons.Panel2
            // 
            this.splitContainerButtons.Panel2.Controls.Add(this.label11);
            this.splitContainerButtons.Panel2.Controls.Add(this.label10);
            this.splitContainerButtons.Panel2.Controls.Add(this.cmbBtnMapFrom);
            this.splitContainerButtons.Panel2.Controls.Add(this.chkAutofire);
            this.splitContainerButtons.Panel2.Controls.Add(this.chkToggling);
            this.splitContainerButtons.Panel2.Controls.Add(this.cmbBtnMapTo);
            this.splitContainerButtons.Panel2.Controls.Add(this.label9);
            this.splitContainerButtons.Size = new System.Drawing.Size(770, 216);
            this.splitContainerButtons.SplitterDistance = 108;
            this.splitContainerButtons.TabIndex = 18;
            // 
            // chkBtn8
            // 
            this.chkBtn8.AutoSize = true;
            this.chkBtn8.Enabled = false;
            this.chkBtn8.Location = new System.Drawing.Point(6, 182);
            this.chkBtn8.Name = "chkBtn8";
            this.chkBtn8.Size = new System.Drawing.Size(32, 17);
            this.chkBtn8.TabIndex = 14;
            this.chkBtn8.Text = "8";
            this.chkBtn8.UseVisualStyleBackColor = true;
            // 
            // chkBtn7
            // 
            this.chkBtn7.AutoSize = true;
            this.chkBtn7.Enabled = false;
            this.chkBtn7.Location = new System.Drawing.Point(6, 159);
            this.chkBtn7.Name = "chkBtn7";
            this.chkBtn7.Size = new System.Drawing.Size(32, 17);
            this.chkBtn7.TabIndex = 13;
            this.chkBtn7.Text = "7";
            this.chkBtn7.UseVisualStyleBackColor = true;
            // 
            // chkBtn6
            // 
            this.chkBtn6.AutoSize = true;
            this.chkBtn6.Enabled = false;
            this.chkBtn6.Location = new System.Drawing.Point(6, 136);
            this.chkBtn6.Name = "chkBtn6";
            this.chkBtn6.Size = new System.Drawing.Size(32, 17);
            this.chkBtn6.TabIndex = 12;
            this.chkBtn6.Text = "6";
            this.chkBtn6.UseVisualStyleBackColor = true;
            // 
            // chkBtn5
            // 
            this.chkBtn5.AutoSize = true;
            this.chkBtn5.Enabled = false;
            this.chkBtn5.Location = new System.Drawing.Point(6, 113);
            this.chkBtn5.Name = "chkBtn5";
            this.chkBtn5.Size = new System.Drawing.Size(32, 17);
            this.chkBtn5.TabIndex = 11;
            this.chkBtn5.Text = "5";
            this.chkBtn5.UseVisualStyleBackColor = true;
            // 
            // chkBtn4
            // 
            this.chkBtn4.AutoSize = true;
            this.chkBtn4.Enabled = false;
            this.chkBtn4.Location = new System.Drawing.Point(6, 90);
            this.chkBtn4.Name = "chkBtn4";
            this.chkBtn4.Size = new System.Drawing.Size(32, 17);
            this.chkBtn4.TabIndex = 10;
            this.chkBtn4.Text = "4";
            this.chkBtn4.UseVisualStyleBackColor = true;
            // 
            // chkBtn3
            // 
            this.chkBtn3.AutoSize = true;
            this.chkBtn3.Enabled = false;
            this.chkBtn3.Location = new System.Drawing.Point(6, 67);
            this.chkBtn3.Name = "chkBtn3";
            this.chkBtn3.Size = new System.Drawing.Size(32, 17);
            this.chkBtn3.TabIndex = 9;
            this.chkBtn3.Tag = "";
            this.chkBtn3.Text = "3";
            this.chkBtn3.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Raw buttons";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(108, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 21;
            this.label11.Text = "vJoy";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(74, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "to";
            // 
            // cmbBtnMapFrom
            // 
            this.cmbBtnMapFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapFrom.FormattingEnabled = true;
            this.cmbBtnMapFrom.Location = new System.Drawing.Point(6, 15);
            this.cmbBtnMapFrom.Name = "cmbBtnMapFrom";
            this.cmbBtnMapFrom.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapFrom.TabIndex = 19;
            // 
            // chkAutofire
            // 
            this.chkAutofire.AutoSize = true;
            this.chkAutofire.Location = new System.Drawing.Point(281, 18);
            this.chkAutofire.Name = "chkAutofire";
            this.chkAutofire.Size = new System.Drawing.Size(100, 17);
            this.chkAutofire.TabIndex = 17;
            this.chkAutofire.Text = "Autofire mode ?";
            this.chkAutofire.UseVisualStyleBackColor = true;
            // 
            // chkToggling
            // 
            this.chkToggling.AutoSize = true;
            this.chkToggling.Location = new System.Drawing.Point(166, 18);
            this.chkToggling.Name = "chkToggling";
            this.chkToggling.Size = new System.Drawing.Size(105, 17);
            this.chkToggling.TabIndex = 16;
            this.chkToggling.Text = "Toggling mode ?";
            this.chkToggling.UseVisualStyleBackColor = true;
            // 
            // cmbBtnMapTo
            // 
            this.cmbBtnMapTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBtnMapTo.FormattingEnabled = true;
            this.cmbBtnMapTo.Location = new System.Drawing.Point(96, 15);
            this.cmbBtnMapTo.Name = "cmbBtnMapTo";
            this.cmbBtnMapTo.Size = new System.Drawing.Size(62, 21);
            this.cmbBtnMapTo.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Mapping IO";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainerMain);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "vJoyIOFeeder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.tooltipContextMenuStrip.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerAxes.Panel1.ResumeLayout(false);
            this.splitContainerAxes.Panel1.PerformLayout();
            this.splitContainerAxes.Panel2.ResumeLayout(false);
            this.splitContainerAxes.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAxes)).EndInit();
            this.splitContainerAxes.ResumeLayout(false);
            this.splitContainerButtons.Panel1.ResumeLayout(false);
            this.splitContainerButtons.Panel1.PerformLayout();
            this.splitContainerButtons.Panel2.ResumeLayout(false);
            this.splitContainerButtons.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerButtons)).EndInit();
            this.splitContainerButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip tooltipContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuShow;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.ProgressBar slJoyAxis;
        private System.Windows.Forms.ProgressBar slRawAxis;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.CheckBox chkBtn1;
        private System.Windows.Forms.CheckBox chkBtn2;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TextBox txtRawAxisValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSelectedAxis;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.SplitContainer splitContainerAxes;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox txtJoyAxisValue;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SplitContainer splitContainerButtons;
        private System.Windows.Forms.CheckBox chkAutofire;
        private System.Windows.Forms.CheckBox chkToggling;
        private System.Windows.Forms.ComboBox cmbBtnMapTo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnOpenvJoyConfig;
        private System.Windows.Forms.Button btnOpenvJoyMonitor;
        private System.Windows.Forms.Button btnOpenJoyCPL;
        private LiveCharts.WinForms.AngularGauge axesJoyGauge;
        private System.Windows.Forms.Button btnShowLogWindow;
        private System.Windows.Forms.Button btnStartStopManager;
        private System.Windows.Forms.CheckBox chkBtn8;
        private System.Windows.Forms.CheckBox chkBtn7;
        private System.Windows.Forms.CheckBox chkBtn6;
        private System.Windows.Forms.CheckBox chkBtn5;
        private System.Windows.Forms.CheckBox chkBtn4;
        private System.Windows.Forms.CheckBox chkBtn3;
        private System.Windows.Forms.ComboBox cmbBtnMapFrom;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnAxisMappingEditor;
        private System.Windows.Forms.ComboBox cmbSelectMode;
    }
}

