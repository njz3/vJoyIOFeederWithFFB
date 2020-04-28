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
            this.tbTrqDeadBand = new System.Windows.Forms.TrackBar();
            this.tbPowerLaw = new System.Windows.Forms.TrackBar();
            this.chkAllowEffectTuning = new System.Windows.Forms.CheckBox();
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
            this.txtPowerLaw = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFriction_Bv = new System.Windows.Forms.TrackBar();
            this.tbMinDamperForActive = new System.Windows.Forms.TrackBar();
            this.label16 = new System.Windows.Forms.Label();
            this.txtFriction_Bv = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbInertia_J = new System.Windows.Forms.TrackBar();
            this.txtInertia_J = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tbInertia_BvRaw = new System.Windows.Forms.TrackBar();
            this.txtInertia_BvRaw = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tbInertia_Bv = new System.Windows.Forms.TrackBar();
            this.txtInertia_Bv = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tbMinAccelThreshold = new System.Windows.Forms.TrackBar();
            this.txtMinAccelThreshold = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.tbMinVelThreshold = new System.Windows.Forms.TrackBar();
            this.txtMinVelThreshold = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbDamper_J = new System.Windows.Forms.TrackBar();
            this.txtDamper_J = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbDamper_Bv = new System.Windows.Forms.TrackBar();
            this.txtDamper_Bv = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbSpring_TrqDeadband = new System.Windows.Forms.TrackBar();
            this.txtSpring_TrqDeadband = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMinDamperForActive = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbSpring_Bv = new System.Windows.Forms.TrackBar();
            this.txtSpring_Bv = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbSpring_Kp = new System.Windows.Forms.TrackBar();
            this.txtSpring_Kp = new System.Windows.Forms.TextBox();
            this.tbPermanentSpring = new System.Windows.Forms.TrackBar();
            this.label17 = new System.Windows.Forms.Label();
            this.txtPermanentSpring = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFriction_Bv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinDamperForActive)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbInertia_J)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbInertia_BvRaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbInertia_Bv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinAccelThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinVelThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamper_J)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamper_Bv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpring_TrqDeadband)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpring_Bv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpring_Kp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPermanentSpring)).BeginInit();
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
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tbTrqDeadBand);
            this.splitContainer1.Panel1.Controls.Add(this.tbPowerLaw);
            this.splitContainer1.Panel1.Controls.Add(this.chkAllowEffectTuning);
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
            this.splitContainer1.Panel1.Controls.Add(this.txtPowerLaw);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbPermanentSpring);
            this.splitContainer1.Panel2.Controls.Add(this.tbMinDamperForActive);
            this.splitContainer1.Panel2.Controls.Add(this.label17);
            this.splitContainer1.Panel2.Controls.Add(this.txtPermanentSpring);
            this.splitContainer1.Panel2.Controls.Add(this.tbFriction_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.label16);
            this.splitContainer1.Panel2.Controls.Add(this.txtFriction_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.label13);
            this.splitContainer1.Panel2.Controls.Add(this.tbInertia_J);
            this.splitContainer1.Panel2.Controls.Add(this.txtInertia_J);
            this.splitContainer1.Panel2.Controls.Add(this.label14);
            this.splitContainer1.Panel2.Controls.Add(this.tbInertia_BvRaw);
            this.splitContainer1.Panel2.Controls.Add(this.txtInertia_BvRaw);
            this.splitContainer1.Panel2.Controls.Add(this.label15);
            this.splitContainer1.Panel2.Controls.Add(this.tbInertia_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.txtInertia_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.label12);
            this.splitContainer1.Panel2.Controls.Add(this.tbMinAccelThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.txtMinAccelThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.label11);
            this.splitContainer1.Panel2.Controls.Add(this.tbMinVelThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.txtMinVelThreshold);
            this.splitContainer1.Panel2.Controls.Add(this.label10);
            this.splitContainer1.Panel2.Controls.Add(this.tbDamper_J);
            this.splitContainer1.Panel2.Controls.Add(this.txtDamper_J);
            this.splitContainer1.Panel2.Controls.Add(this.label9);
            this.splitContainer1.Panel2.Controls.Add(this.tbDamper_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.txtDamper_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.label8);
            this.splitContainer1.Panel2.Controls.Add(this.tbSpring_TrqDeadband);
            this.splitContainer1.Panel2.Controls.Add(this.txtSpring_TrqDeadband);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.txtMinDamperForActive);
            this.splitContainer1.Panel2.Controls.Add(this.btnReset);
            this.splitContainer1.Panel2.Controls.Add(this.btnClose);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.tbSpring_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.txtSpring_Bv);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.tbSpring_Kp);
            this.splitContainer1.Panel2.Controls.Add(this.txtSpring_Kp);
            this.splitContainer1.Size = new System.Drawing.Size(634, 491);
            this.splitContainer1.SplitterDistance = 179;
            this.splitContainer1.TabIndex = 20;
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
            this.tbPowerLaw.Value = 1;
            this.tbPowerLaw.Scroll += new System.EventHandler(this.tbPowerLaw_Scroll);
            // 
            // chkAllowEffectTuning
            // 
            this.chkAllowEffectTuning.AutoSize = true;
            this.chkAllowEffectTuning.Location = new System.Drawing.Point(12, 59);
            this.chkAllowEffectTuning.Name = "chkAllowEffectTuning";
            this.chkAllowEffectTuning.Size = new System.Drawing.Size(186, 17);
            this.chkAllowEffectTuning.TabIndex = 35;
            this.chkAllowEffectTuning.Text = "Allow detailled effect tuning below";
            this.chkAllowEffectTuning.UseVisualStyleBackColor = true;
            this.chkAllowEffectTuning.Click += new System.EventHandler(this.chkAllowEffectTuning_Click);
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
            this.chkForceTorque.Location = new System.Drawing.Point(12, 125);
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
            this.chkEmulateMissing.Location = new System.Drawing.Point(12, 102);
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
            this.label4.Size = new System.Drawing.Size(532, 16);
            this.label4.TabIndex = 23;
            this.label4.Text = "General effect parameters (some are not availabledepends on all hardware)";
            // 
            // chkPulsedTrq
            // 
            this.chkPulsedTrq.AutoSize = true;
            this.chkPulsedTrq.Enabled = false;
            this.chkPulsedTrq.Location = new System.Drawing.Point(12, 148);
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
            this.chkSkipStopEffect.Location = new System.Drawing.Point(12, 81);
            this.chkSkipStopEffect.Name = "chkSkipStopEffect";
            this.chkSkipStopEffect.Size = new System.Drawing.Size(255, 17);
            this.chkSkipStopEffect.TabIndex = 26;
            this.chkSkipStopEffect.Text = "Skip Stop Effects Commands (effects are infinite)";
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
            this.tbGlobalGain.Value = 1;
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
            // tbFriction_Bv
            // 
            this.tbFriction_Bv.Enabled = false;
            this.tbFriction_Bv.Location = new System.Drawing.Point(105, 179);
            this.tbFriction_Bv.Maximum = 50;
            this.tbFriction_Bv.Name = "tbFriction_Bv";
            this.tbFriction_Bv.Size = new System.Drawing.Size(104, 45);
            this.tbFriction_Bv.TabIndex = 61;
            this.tbFriction_Bv.TickFrequency = 5;
            this.tbFriction_Bv.Scroll += new System.EventHandler(this.tbFriction_Bv_Scroll);
            // 
            // tbMinDamperForActive
            // 
            this.tbMinDamperForActive.Enabled = false;
            this.tbMinDamperForActive.Location = new System.Drawing.Point(105, 212);
            this.tbMinDamperForActive.Maximum = 50;
            this.tbMinDamperForActive.Name = "tbMinDamperForActive";
            this.tbMinDamperForActive.Size = new System.Drawing.Size(104, 45);
            this.tbMinDamperForActive.TabIndex = 34;
            this.tbMinDamperForActive.TickFrequency = 5;
            this.tbMinDamperForActive.Scroll += new System.EventHandler(this.tbMinDamperForActive_Scroll);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(24, 184);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 13);
            this.label16.TabIndex = 62;
            this.label16.Text = "Friction Bv";
            // 
            // txtFriction_Bv
            // 
            this.txtFriction_Bv.Enabled = false;
            this.txtFriction_Bv.Location = new System.Drawing.Point(215, 181);
            this.txtFriction_Bv.Name = "txtFriction_Bv";
            this.txtFriction_Bv.Size = new System.Drawing.Size(57, 20);
            this.txtFriction_Bv.TabIndex = 63;
            this.txtFriction_Bv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFriction_Bv_KeyPress);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(318, 135);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(44, 13);
            this.label13.TabIndex = 59;
            this.label13.Text = "Inertia J";
            // 
            // tbInertia_J
            // 
            this.tbInertia_J.Enabled = false;
            this.tbInertia_J.Location = new System.Drawing.Point(399, 130);
            this.tbInertia_J.Maximum = 100;
            this.tbInertia_J.Name = "tbInertia_J";
            this.tbInertia_J.Size = new System.Drawing.Size(104, 45);
            this.tbInertia_J.TabIndex = 58;
            this.tbInertia_J.TickFrequency = 10;
            this.tbInertia_J.Scroll += new System.EventHandler(this.tbInertia_J_Scroll);
            // 
            // txtInertia_J
            // 
            this.txtInertia_J.Enabled = false;
            this.txtInertia_J.Location = new System.Drawing.Point(509, 132);
            this.txtInertia_J.Name = "txtInertia_J";
            this.txtInertia_J.Size = new System.Drawing.Size(57, 20);
            this.txtInertia_J.TabIndex = 60;
            this.txtInertia_J.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInertia_J_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(318, 102);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 56;
            this.label14.Text = "Inertia BvRaw";
            // 
            // tbInertia_BvRaw
            // 
            this.tbInertia_BvRaw.Enabled = false;
            this.tbInertia_BvRaw.Location = new System.Drawing.Point(399, 99);
            this.tbInertia_BvRaw.Maximum = 50;
            this.tbInertia_BvRaw.Name = "tbInertia_BvRaw";
            this.tbInertia_BvRaw.Size = new System.Drawing.Size(104, 45);
            this.tbInertia_BvRaw.TabIndex = 55;
            this.tbInertia_BvRaw.TickFrequency = 5;
            this.tbInertia_BvRaw.Scroll += new System.EventHandler(this.tbInertia_BvRaw_Scroll);
            // 
            // txtInertia_BvRaw
            // 
            this.txtInertia_BvRaw.Enabled = false;
            this.txtInertia_BvRaw.Location = new System.Drawing.Point(509, 99);
            this.txtInertia_BvRaw.Name = "txtInertia_BvRaw";
            this.txtInertia_BvRaw.Size = new System.Drawing.Size(57, 20);
            this.txtInertia_BvRaw.TabIndex = 57;
            this.txtInertia_BvRaw.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInertia_BvRaw_KeyPress);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(318, 66);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(52, 13);
            this.label15.TabIndex = 53;
            this.label15.Text = "Inertia Bv";
            // 
            // tbInertia_Bv
            // 
            this.tbInertia_Bv.Enabled = false;
            this.tbInertia_Bv.Location = new System.Drawing.Point(399, 61);
            this.tbInertia_Bv.Maximum = 25;
            this.tbInertia_Bv.Name = "tbInertia_Bv";
            this.tbInertia_Bv.Size = new System.Drawing.Size(104, 45);
            this.tbInertia_Bv.TabIndex = 52;
            this.tbInertia_Bv.TickFrequency = 3;
            this.tbInertia_Bv.Scroll += new System.EventHandler(this.tbInertia_Bv_Scroll);
            // 
            // txtInertia_Bv
            // 
            this.txtInertia_Bv.Enabled = false;
            this.txtInertia_Bv.Location = new System.Drawing.Point(509, 63);
            this.txtInertia_Bv.Name = "txtInertia_Bv";
            this.txtInertia_Bv.Size = new System.Drawing.Size(57, 20);
            this.txtInertia_Bv.TabIndex = 54;
            this.txtInertia_Bv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInertia_Bv_KeyPress);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(318, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(53, 13);
            this.label12.TabIndex = 50;
            this.label12.Text = "Min accel";
            // 
            // tbMinAccelThreshold
            // 
            this.tbMinAccelThreshold.Enabled = false;
            this.tbMinAccelThreshold.Location = new System.Drawing.Point(399, 16);
            this.tbMinAccelThreshold.Maximum = 1000;
            this.tbMinAccelThreshold.Minimum = 1;
            this.tbMinAccelThreshold.Name = "tbMinAccelThreshold";
            this.tbMinAccelThreshold.Size = new System.Drawing.Size(104, 45);
            this.tbMinAccelThreshold.TabIndex = 49;
            this.tbMinAccelThreshold.TickFrequency = 100;
            this.tbMinAccelThreshold.Value = 1;
            this.tbMinAccelThreshold.Scroll += new System.EventHandler(this.tbMinAccelThreshold_Scroll);
            // 
            // txtMinAccelThreshold
            // 
            this.txtMinAccelThreshold.Enabled = false;
            this.txtMinAccelThreshold.Location = new System.Drawing.Point(509, 18);
            this.txtMinAccelThreshold.Name = "txtMinAccelThreshold";
            this.txtMinAccelThreshold.Size = new System.Drawing.Size(57, 20);
            this.txtMinAccelThreshold.TabIndex = 51;
            this.txtMinAccelThreshold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinAccelThreshold_KeyPress);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(22, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 47;
            this.label11.Text = "Min velocity";
            // 
            // tbMinVelThreshold
            // 
            this.tbMinVelThreshold.Enabled = false;
            this.tbMinVelThreshold.Location = new System.Drawing.Point(105, 16);
            this.tbMinVelThreshold.Maximum = 1000;
            this.tbMinVelThreshold.Minimum = 1;
            this.tbMinVelThreshold.Name = "tbMinVelThreshold";
            this.tbMinVelThreshold.Size = new System.Drawing.Size(104, 45);
            this.tbMinVelThreshold.TabIndex = 46;
            this.tbMinVelThreshold.TickFrequency = 100;
            this.tbMinVelThreshold.Value = 1;
            this.tbMinVelThreshold.Scroll += new System.EventHandler(this.tbMinVelThreshold_Scroll);
            // 
            // txtMinVelThreshold
            // 
            this.txtMinVelThreshold.Enabled = false;
            this.txtMinVelThreshold.Location = new System.Drawing.Point(215, 18);
            this.txtMinVelThreshold.Name = "txtMinVelThreshold";
            this.txtMinVelThreshold.Size = new System.Drawing.Size(57, 20);
            this.txtMinVelThreshold.TabIndex = 48;
            this.txtMinVelThreshold.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinVelThreshold_KeyPress);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(316, 214);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 44;
            this.label10.Text = "Damper J (Iner.)";
            // 
            // tbDamper_J
            // 
            this.tbDamper_J.Enabled = false;
            this.tbDamper_J.Location = new System.Drawing.Point(399, 209);
            this.tbDamper_J.Maximum = 20;
            this.tbDamper_J.Name = "tbDamper_J";
            this.tbDamper_J.Size = new System.Drawing.Size(104, 45);
            this.tbDamper_J.TabIndex = 43;
            this.tbDamper_J.TickFrequency = 2;
            this.tbDamper_J.Scroll += new System.EventHandler(this.tbDamper_J_Scroll);
            // 
            // txtDamper_J
            // 
            this.txtDamper_J.Enabled = false;
            this.txtDamper_J.Location = new System.Drawing.Point(509, 211);
            this.txtDamper_J.Name = "txtDamper_J";
            this.txtDamper_J.Size = new System.Drawing.Size(57, 20);
            this.txtDamper_J.TabIndex = 45;
            this.txtDamper_J.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDamper_J_KeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(316, 181);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "Damper Bv";
            // 
            // tbDamper_Bv
            // 
            this.tbDamper_Bv.Enabled = false;
            this.tbDamper_Bv.Location = new System.Drawing.Point(399, 176);
            this.tbDamper_Bv.Maximum = 40;
            this.tbDamper_Bv.Name = "tbDamper_Bv";
            this.tbDamper_Bv.Size = new System.Drawing.Size(104, 45);
            this.tbDamper_Bv.TabIndex = 40;
            this.tbDamper_Bv.TickFrequency = 4;
            this.tbDamper_Bv.Scroll += new System.EventHandler(this.tbDamper_Bv_Scroll);
            // 
            // txtDamper_Bv
            // 
            this.txtDamper_Bv.Enabled = false;
            this.txtDamper_Bv.Location = new System.Drawing.Point(509, 178);
            this.txtDamper_Bv.Name = "txtDamper_Bv";
            this.txtDamper_Bv.Size = new System.Drawing.Size(57, 20);
            this.txtDamper_Bv.TabIndex = 42;
            this.txtDamper_Bv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDamper_Bv_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(22, 134);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 38;
            this.label8.Text = "Spring Deadband";
            // 
            // tbSpring_TrqDeadband
            // 
            this.tbSpring_TrqDeadband.Enabled = false;
            this.tbSpring_TrqDeadband.Location = new System.Drawing.Point(105, 129);
            this.tbSpring_TrqDeadband.Maximum = 50;
            this.tbSpring_TrqDeadband.Name = "tbSpring_TrqDeadband";
            this.tbSpring_TrqDeadband.Size = new System.Drawing.Size(104, 45);
            this.tbSpring_TrqDeadband.TabIndex = 37;
            this.tbSpring_TrqDeadband.TickFrequency = 5;
            this.tbSpring_TrqDeadband.Scroll += new System.EventHandler(this.tbSpring_TrqDeadband_Scroll);
            // 
            // txtSpring_TrqDeadband
            // 
            this.txtSpring_TrqDeadband.Enabled = false;
            this.txtSpring_TrqDeadband.Location = new System.Drawing.Point(215, 131);
            this.txtSpring_TrqDeadband.Name = "txtSpring_TrqDeadband";
            this.txtSpring_TrqDeadband.Size = new System.Drawing.Size(57, 20);
            this.txtSpring_TrqDeadband.TabIndex = 39;
            this.txtSpring_TrqDeadband.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSpring_TrqDeadband_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 217);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 35;
            this.label3.Text = "Active Damper";
            // 
            // txtMinDamperForActive
            // 
            this.txtMinDamperForActive.Enabled = false;
            this.txtMinDamperForActive.Location = new System.Drawing.Point(215, 214);
            this.txtMinDamperForActive.Name = "txtMinDamperForActive";
            this.txtMinDamperForActive.Size = new System.Drawing.Size(57, 20);
            this.txtMinDamperForActive.TabIndex = 36;
            this.txtMinDamperForActive.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinDamperForActive_KeyPress);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 273);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 33;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(547, 273);
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
            this.label2.Location = new System.Drawing.Point(22, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Spring Bv (frict.)";
            // 
            // tbSpring_Bv
            // 
            this.tbSpring_Bv.Enabled = false;
            this.tbSpring_Bv.Location = new System.Drawing.Point(105, 96);
            this.tbSpring_Bv.Maximum = 50;
            this.tbSpring_Bv.Name = "tbSpring_Bv";
            this.tbSpring_Bv.Size = new System.Drawing.Size(104, 45);
            this.tbSpring_Bv.TabIndex = 29;
            this.tbSpring_Bv.TickFrequency = 5;
            this.tbSpring_Bv.Scroll += new System.EventHandler(this.tbSpring_Bv_Scroll);
            // 
            // txtSpring_Bv
            // 
            this.txtSpring_Bv.Enabled = false;
            this.txtSpring_Bv.Location = new System.Drawing.Point(215, 98);
            this.txtSpring_Bv.Name = "txtSpring_Bv";
            this.txtSpring_Bv.Size = new System.Drawing.Size(57, 20);
            this.txtSpring_Bv.TabIndex = 31;
            this.txtSpring_Bv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSpring_Bv_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Spring Kp gain";
            // 
            // tbSpring_Kp
            // 
            this.tbSpring_Kp.Enabled = false;
            this.tbSpring_Kp.Location = new System.Drawing.Point(105, 61);
            this.tbSpring_Kp.Maximum = 200;
            this.tbSpring_Kp.Minimum = 1;
            this.tbSpring_Kp.Name = "tbSpring_Kp";
            this.tbSpring_Kp.Size = new System.Drawing.Size(104, 45);
            this.tbSpring_Kp.TabIndex = 26;
            this.tbSpring_Kp.TickFrequency = 20;
            this.tbSpring_Kp.Value = 1;
            this.tbSpring_Kp.Scroll += new System.EventHandler(this.tbSpring_Kp_Scroll);
            // 
            // txtSpring_Kp
            // 
            this.txtSpring_Kp.Enabled = false;
            this.txtSpring_Kp.Location = new System.Drawing.Point(215, 63);
            this.txtSpring_Kp.Name = "txtSpring_Kp";
            this.txtSpring_Kp.Size = new System.Drawing.Size(57, 20);
            this.txtSpring_Kp.TabIndex = 28;
            this.txtSpring_Kp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSpring_Kp_KeyPress);
            // 
            // tbPermanentSpring
            // 
            this.tbPermanentSpring.Enabled = false;
            this.tbPermanentSpring.Location = new System.Drawing.Point(105, 246);
            this.tbPermanentSpring.Maximum = 50;
            this.tbPermanentSpring.Name = "tbPermanentSpring";
            this.tbPermanentSpring.Size = new System.Drawing.Size(104, 45);
            this.tbPermanentSpring.TabIndex = 64;
            this.tbPermanentSpring.TickFrequency = 5;
            this.tbPermanentSpring.Scroll += new System.EventHandler(this.tbPermanentSpring_Scroll);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 251);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(91, 13);
            this.label17.TabIndex = 65;
            this.label17.Text = "Permanent Spring";
            // 
            // txtPermanentSpring
            // 
            this.txtPermanentSpring.Enabled = false;
            this.txtPermanentSpring.Location = new System.Drawing.Point(215, 248);
            this.txtPermanentSpring.Name = "txtPermanentSpring";
            this.txtPermanentSpring.Size = new System.Drawing.Size(57, 20);
            this.txtPermanentSpring.TabIndex = 66;
            this.txtPermanentSpring.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPermanentSpring_KeyPress);
            // 
            // EffectTuningEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(634, 491);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EffectTuningEditor";
            this.Text = "Effects tuning editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EffectTuningEditor_FormClosed);
            this.Load += new System.EventHandler(this.EffectTuningEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFriction_Bv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinDamperForActive)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbInertia_J)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbInertia_BvRaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbInertia_Bv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinAccelThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbMinVelThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamper_J)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDamper_Bv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpring_TrqDeadband)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpring_Bv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbSpring_Kp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPermanentSpring)).EndInit();
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
        private System.Windows.Forms.TrackBar tbSpring_Bv;
        private System.Windows.Forms.TextBox txtSpring_Bv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tbSpring_Kp;
        private System.Windows.Forms.TextBox txtSpring_Kp;
        private System.Windows.Forms.Label lbCurrentControlSet;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar tbMinDamperForActive;
        private System.Windows.Forms.TextBox txtMinDamperForActive;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TrackBar tbSpring_TrqDeadband;
        private System.Windows.Forms.TextBox txtSpring_TrqDeadband;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar tbMinAccelThreshold;
        private System.Windows.Forms.TextBox txtMinAccelThreshold;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TrackBar tbMinVelThreshold;
        private System.Windows.Forms.TextBox txtMinVelThreshold;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TrackBar tbDamper_J;
        private System.Windows.Forms.TextBox txtDamper_J;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar tbDamper_Bv;
        private System.Windows.Forms.TextBox txtDamper_Bv;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TrackBar tbInertia_J;
        private System.Windows.Forms.TextBox txtInertia_J;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TrackBar tbInertia_BvRaw;
        private System.Windows.Forms.TextBox txtInertia_BvRaw;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TrackBar tbInertia_Bv;
        private System.Windows.Forms.TextBox txtInertia_Bv;
        private System.Windows.Forms.CheckBox chkAllowEffectTuning;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TrackBar tbFriction_Bv;
        private System.Windows.Forms.TextBox txtFriction_Bv;
        private System.Windows.Forms.TrackBar tbPermanentSpring;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtPermanentSpring;
    }
}

