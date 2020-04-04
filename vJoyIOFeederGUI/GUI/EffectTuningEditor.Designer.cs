namespace vJoyIOFeederGUI.GUI
{
    partial class EffectTuningEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbCurrentControlSet = new System.Windows.Forms.Label();
            this.chkForceTorque = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.chkEmulateMissing = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkPulsedTrq = new System.Windows.Forms.CheckBox();
            this.chkSkipStopEffect = new System.Windows.Forms.CheckBox();
            this.txtTrqDeadBand = new System.Windows.Forms.TextBox();
            this.tbGlobalGain = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGlobalGain = new System.Windows.Forms.TextBox();
            this.tbTrqDeadBand = new System.Windows.Forms.TrackBar();
            this.tbPowerLaw = new System.Windows.Forms.TrackBar();
            this.txtPowerLaw = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tpSpring_Kp = new System.Windows.Forms.TrackBar();
            this.txtSpring_Kp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.trackBar4 = new System.Windows.Forms.TrackBar();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.trackBar5 = new System.Windows.Forms.TrackBar();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.trackBar6 = new System.Windows.Forms.TrackBar();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.trackBar7 = new System.Windows.Forms.TrackBar();
            this.textBox7 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpSpring_Kp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar7)).BeginInit();
            this.SuspendLayout();
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 500;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
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
            this.splitContainer1.Panel1.Controls.Add(this.lbCurrentControlSet);
            this.splitContainer1.Panel1.Controls.Add(this.chkForceTorque);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.chkEmulateMissing);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.chkPulsedTrq);
            this.splitContainer1.Panel1.Controls.Add(this.chkSkipStopEffect);
            this.splitContainer1.Panel1.Controls.Add(this.txtTrqDeadBand);
            this.splitContainer1.Panel1.Controls.Add(this.tbGlobalGain);
            this.splitContainer1.Panel1.Controls.Add(this.label7);
            this.splitContainer1.Panel1.Controls.Add(this.txtGlobalGain);
            this.splitContainer1.Panel1.Controls.Add(this.tbTrqDeadBand);
            this.splitContainer1.Panel1.Controls.Add(this.tbPowerLaw);
            this.splitContainer1.Panel1.Controls.Add(this.txtPowerLaw);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label12);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar7);
            this.splitContainer1.Panel2.Controls.Add(this.textBox7);
            this.splitContainer1.Panel2.Controls.Add(this.label11);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar6);
            this.splitContainer1.Panel2.Controls.Add(this.textBox6);
            this.splitContainer1.Panel2.Controls.Add(this.label10);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar5);
            this.splitContainer1.Panel2.Controls.Add(this.textBox5);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar4);
            this.splitContainer1.Panel2.Controls.Add(this.textBox4);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar3);
            this.splitContainer1.Panel2.Controls.Add(this.textBox3);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar2);
            this.splitContainer1.Panel2.Controls.Add(this.textBox2);
            this.splitContainer1.Panel2.Controls.Add(this.btnReset);
            this.splitContainer1.Panel2.Controls.Add(this.btnClose);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar1);
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.tpSpring_Kp);
            this.splitContainer1.Panel2.Controls.Add(this.txtSpring_Kp);
            this.splitContainer1.Size = new System.Drawing.Size(634, 436);
            this.splitContainer1.SplitterDistance = 159;
            this.splitContainer1.TabIndex = 20;
            // 
            // lbCurrentControlSet
            // 
            this.lbCurrentControlSet.AutoSize = true;
            this.lbCurrentControlSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentControlSet.ForeColor = System.Drawing.SystemColors.Highlight;
            this.lbCurrentControlSet.Location = new System.Drawing.Point(9, 33);
            this.lbCurrentControlSet.Name = "lbCurrentControlSet";
            this.lbCurrentControlSet.Size = new System.Drawing.Size(80, 15);
            this.lbCurrentControlSet.TabIndex = 34;
            this.lbCurrentControlSet.Text = "Control set:";
            // 
            // chkForceTorque
            // 
            this.chkForceTorque.AutoSize = true;
            this.chkForceTorque.Enabled = false;
            this.chkForceTorque.Location = new System.Drawing.Point(12, 104);
            this.chkForceTorque.Name = "chkForceTorque";
            this.chkForceTorque.Size = new System.Drawing.Size(203, 17);
            this.chkForceTorque.TabIndex = 33;
            this.chkForceTorque.Text = "Force translation to torque commands";
            this.chkForceTorque.UseVisualStyleBackColor = true;
            this.chkForceTorque.Click += new System.EventHandler(this.chkForceTorque_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(393, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Global gain";
            // 
            // chkEmulateMissing
            // 
            this.chkEmulateMissing.AutoSize = true;
            this.chkEmulateMissing.Enabled = false;
            this.chkEmulateMissing.Location = new System.Drawing.Point(12, 81);
            this.chkEmulateMissing.Name = "chkEmulateMissing";
            this.chkEmulateMissing.Size = new System.Drawing.Size(296, 17);
            this.chkEmulateMissing.TabIndex = 3;
            this.chkEmulateMissing.Text = "Emulated missing effects (translated to torque commands)";
            this.chkEmulateMissing.UseVisualStyleBackColor = true;
            this.chkEmulateMissing.Click += new System.EventHandler(this.chkEmulateMissing_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(558, 16);
            this.label4.TabIndex = 23;
            this.label4.Text = "General effect parameters (some may not be changed when manager is running)";
            // 
            // chkPulsedTrq
            // 
            this.chkPulsedTrq.AutoSize = true;
            this.chkPulsedTrq.Enabled = false;
            this.chkPulsedTrq.Location = new System.Drawing.Point(12, 127);
            this.chkPulsedTrq.Name = "chkPulsedTrq";
            this.chkPulsedTrq.Size = new System.Drawing.Size(380, 17);
            this.chkPulsedTrq.TabIndex = 2;
            this.chkPulsedTrq.Text = "Use quarter-pulsed Torque (increase torque resolution but introduce ripples)";
            this.chkPulsedTrq.UseVisualStyleBackColor = true;
            this.chkPulsedTrq.Click += new System.EventHandler(this.chkPulsedTrq_Click);
            // 
            // chkSkipStopEffect
            // 
            this.chkSkipStopEffect.AutoSize = true;
            this.chkSkipStopEffect.Location = new System.Drawing.Point(12, 60);
            this.chkSkipStopEffect.Name = "chkSkipStopEffect";
            this.chkSkipStopEffect.Size = new System.Drawing.Size(108, 17);
            this.chkSkipStopEffect.TabIndex = 26;
            this.chkSkipStopEffect.Text = "Skip Stop Effects";
            this.chkSkipStopEffect.UseVisualStyleBackColor = true;
            this.chkSkipStopEffect.Click += new System.EventHandler(this.chkSkipStopEffect_Click);
            // 
            // txtTrqDeadBand
            // 
            this.txtTrqDeadBand.Location = new System.Drawing.Point(569, 129);
            this.txtTrqDeadBand.Name = "txtTrqDeadBand";
            this.txtTrqDeadBand.Size = new System.Drawing.Size(57, 20);
            this.txtTrqDeadBand.TabIndex = 32;
            this.txtTrqDeadBand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrqDeadBand_KeyPress);
            // 
            // tbGlobalGain
            // 
            this.tbGlobalGain.Location = new System.Drawing.Point(459, 53);
            this.tbGlobalGain.Maximum = 30;
            this.tbGlobalGain.Minimum = 1;
            this.tbGlobalGain.Name = "tbGlobalGain";
            this.tbGlobalGain.Size = new System.Drawing.Size(104, 45);
            this.tbGlobalGain.TabIndex = 23;
            this.tbGlobalGain.TickFrequency = 3;
            this.tbGlobalGain.Value = 10;
            this.tbGlobalGain.Scroll += new System.EventHandler(this.tbGlobalGain_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(393, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Trq deadband";
            // 
            // txtGlobalGain
            // 
            this.txtGlobalGain.Location = new System.Drawing.Point(569, 55);
            this.txtGlobalGain.Name = "txtGlobalGain";
            this.txtGlobalGain.Size = new System.Drawing.Size(57, 20);
            this.txtGlobalGain.TabIndex = 25;
            this.txtGlobalGain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGlobalGain_KeyPress);
            // 
            // tbTrqDeadBand
            // 
            this.tbTrqDeadBand.LargeChange = 10;
            this.tbTrqDeadBand.Location = new System.Drawing.Point(459, 127);
            this.tbTrqDeadBand.Maximum = 50;
            this.tbTrqDeadBand.Name = "tbTrqDeadBand";
            this.tbTrqDeadBand.Size = new System.Drawing.Size(104, 45);
            this.tbTrqDeadBand.TabIndex = 30;
            this.tbTrqDeadBand.TickFrequency = 5;
            this.tbTrqDeadBand.Scroll += new System.EventHandler(this.tbTrqDeadBand_Scroll);
            // 
            // tbPowerLaw
            // 
            this.tbPowerLaw.Location = new System.Drawing.Point(459, 90);
            this.tbPowerLaw.Maximum = 30;
            this.tbPowerLaw.Minimum = 1;
            this.tbPowerLaw.Name = "tbPowerLaw";
            this.tbPowerLaw.Size = new System.Drawing.Size(104, 45);
            this.tbPowerLaw.TabIndex = 27;
            this.tbPowerLaw.TickFrequency = 3;
            this.tbPowerLaw.Value = 10;
            this.tbPowerLaw.Scroll += new System.EventHandler(this.tbPowerLaw_Scroll);
            // 
            // txtPowerLaw
            // 
            this.txtPowerLaw.Location = new System.Drawing.Point(569, 92);
            this.txtPowerLaw.Name = "txtPowerLaw";
            this.txtPowerLaw.Size = new System.Drawing.Size(57, 20);
            this.txtPowerLaw.TabIndex = 29;
            this.txtPowerLaw.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPowerLaw_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(393, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Power law";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(14, 238);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 33;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(551, 238);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 32;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(9, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Spring Bv (frict.)";
            // 
            // trackBar1
            // 
            this.trackBar1.Enabled = false;
            this.trackBar1.Location = new System.Drawing.Point(81, 104);
            this.trackBar1.Maximum = 30;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 45);
            this.trackBar1.TabIndex = 29;
            this.trackBar1.TickFrequency = 3;
            this.trackBar1.Value = 10;
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(191, 106);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(57, 20);
            this.textBox1.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(9, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Spring Kp gain";
            // 
            // tpSpring_Kp
            // 
            this.tpSpring_Kp.Enabled = false;
            this.tpSpring_Kp.Location = new System.Drawing.Point(81, 71);
            this.tpSpring_Kp.Maximum = 30;
            this.tpSpring_Kp.Minimum = 1;
            this.tpSpring_Kp.Name = "tpSpring_Kp";
            this.tpSpring_Kp.Size = new System.Drawing.Size(104, 45);
            this.tpSpring_Kp.TabIndex = 26;
            this.tpSpring_Kp.TickFrequency = 3;
            this.tpSpring_Kp.Value = 10;
            // 
            // txtSpring_Kp
            // 
            this.txtSpring_Kp.Enabled = false;
            this.txtSpring_Kp.Location = new System.Drawing.Point(191, 73);
            this.txtSpring_Kp.Name = "txtSpring_Kp";
            this.txtSpring_Kp.Size = new System.Drawing.Size(57, 20);
            this.txtSpring_Kp.TabIndex = 28;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(255, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Friction Bv";
            // 
            // trackBar2
            // 
            this.trackBar2.Enabled = false;
            this.trackBar2.Location = new System.Drawing.Point(327, 71);
            this.trackBar2.Maximum = 30;
            this.trackBar2.Minimum = 1;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(104, 45);
            this.trackBar2.TabIndex = 34;
            this.trackBar2.TickFrequency = 3;
            this.trackBar2.Value = 10;
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(437, 73);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(57, 20);
            this.textBox2.TabIndex = 36;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(9, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Spring Deadband";
            // 
            // trackBar3
            // 
            this.trackBar3.Enabled = false;
            this.trackBar3.Location = new System.Drawing.Point(81, 135);
            this.trackBar3.Maximum = 30;
            this.trackBar3.Minimum = 1;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(104, 45);
            this.trackBar3.TabIndex = 37;
            this.trackBar3.TickFrequency = 3;
            this.trackBar3.Value = 10;
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(191, 137);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(57, 20);
            this.textBox3.TabIndex = 39;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Enabled = false;
            this.label9.Location = new System.Drawing.Point(255, 109);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "Damper Bv";
            // 
            // trackBar4
            // 
            this.trackBar4.Enabled = false;
            this.trackBar4.Location = new System.Drawing.Point(327, 104);
            this.trackBar4.Maximum = 30;
            this.trackBar4.Minimum = 1;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(104, 45);
            this.trackBar4.TabIndex = 40;
            this.trackBar4.TickFrequency = 3;
            this.trackBar4.Value = 10;
            // 
            // textBox4
            // 
            this.textBox4.Enabled = false;
            this.textBox4.Location = new System.Drawing.Point(437, 106);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(57, 20);
            this.textBox4.TabIndex = 42;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Enabled = false;
            this.label10.Location = new System.Drawing.Point(255, 140);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 44;
            this.label10.Text = "Damper J (Iner.)";
            // 
            // trackBar5
            // 
            this.trackBar5.Enabled = false;
            this.trackBar5.Location = new System.Drawing.Point(327, 135);
            this.trackBar5.Maximum = 30;
            this.trackBar5.Minimum = 1;
            this.trackBar5.Name = "trackBar5";
            this.trackBar5.Size = new System.Drawing.Size(104, 45);
            this.trackBar5.TabIndex = 43;
            this.trackBar5.TickFrequency = 3;
            this.trackBar5.Value = 10;
            // 
            // textBox5
            // 
            this.textBox5.Enabled = false;
            this.textBox5.Location = new System.Drawing.Point(437, 137);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(57, 20);
            this.textBox5.TabIndex = 45;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Enabled = false;
            this.label11.Location = new System.Drawing.Point(9, 8);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Min velocity";
            // 
            // trackBar6
            // 
            this.trackBar6.Enabled = false;
            this.trackBar6.Location = new System.Drawing.Point(81, 3);
            this.trackBar6.Maximum = 30;
            this.trackBar6.Minimum = 1;
            this.trackBar6.Name = "trackBar6";
            this.trackBar6.Size = new System.Drawing.Size(104, 45);
            this.trackBar6.TabIndex = 46;
            this.trackBar6.TickFrequency = 3;
            this.trackBar6.Value = 10;
            // 
            // textBox6
            // 
            this.textBox6.Enabled = false;
            this.textBox6.Location = new System.Drawing.Point(191, 5);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(57, 20);
            this.textBox6.TabIndex = 48;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Enabled = false;
            this.label12.Location = new System.Drawing.Point(255, 8);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 50;
            this.label12.Text = "Min accel";
            // 
            // trackBar7
            // 
            this.trackBar7.Enabled = false;
            this.trackBar7.Location = new System.Drawing.Point(327, 3);
            this.trackBar7.Maximum = 30;
            this.trackBar7.Minimum = 1;
            this.trackBar7.Name = "trackBar7";
            this.trackBar7.Size = new System.Drawing.Size(104, 45);
            this.trackBar7.TabIndex = 49;
            this.trackBar7.TickFrequency = 3;
            this.trackBar7.Value = 10;
            // 
            // textBox7
            // 
            this.textBox7.Enabled = false;
            this.textBox7.Location = new System.Drawing.Point(437, 5);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(57, 20);
            this.textBox7.TabIndex = 51;
            // 
            // EffectTuningEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(634, 436);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EffectTuningEditor";
            this.Text = "Effects tuning editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TargetHdwForm_FormClosed);
            this.Load += new System.EventHandler(this.TargetHdwForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpSpring_Kp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar7)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox chkEmulateMissing;
        private System.Windows.Forms.CheckBox chkPulsedTrq;
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
        private System.Windows.Forms.CheckBox chkForceTorque;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tpSpring_Kp;
        private System.Windows.Forms.TextBox txtSpring_Kp;
        private System.Windows.Forms.Label lbCurrentControlSet;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar trackBar7;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TrackBar trackBar6;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TrackBar trackBar5;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar trackBar4;
        private System.Windows.Forms.TextBox textBox4;
    }
}

