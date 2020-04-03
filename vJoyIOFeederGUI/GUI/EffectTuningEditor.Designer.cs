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
            this.chkForceTorque = new System.Windows.Forms.CheckBox();
            this.txtTrqDeadBand = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTrqDeadBand = new System.Windows.Forms.TrackBar();
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
            this.label1 = new System.Windows.Forms.Label();
            this.tpSpring_Kp = new System.Windows.Forms.TrackBar();
            this.txtSpring_Kp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpSpring_Kp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
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
            // chkForceTorque
            // 
            this.chkForceTorque.AutoSize = true;
            this.chkForceTorque.Enabled = false;
            this.chkForceTorque.Location = new System.Drawing.Point(12, 82);
            this.chkForceTorque.Name = "chkForceTorque";
            this.chkForceTorque.Size = new System.Drawing.Size(203, 17);
            this.chkForceTorque.TabIndex = 33;
            this.chkForceTorque.Text = "Force translation to torque commands";
            this.chkForceTorque.UseVisualStyleBackColor = true;
            this.chkForceTorque.Click += new System.EventHandler(this.chkForceTorque_Click);
            // 
            // txtTrqDeadBand
            // 
            this.txtTrqDeadBand.Location = new System.Drawing.Point(569, 107);
            this.txtTrqDeadBand.Name = "txtTrqDeadBand";
            this.txtTrqDeadBand.Size = new System.Drawing.Size(57, 20);
            this.txtTrqDeadBand.TabIndex = 32;
            this.txtTrqDeadBand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTrqDeadBand_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(393, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 13);
            this.label7.TabIndex = 31;
            this.label7.Text = "Trq deadband";
            // 
            // tbTrqDeadBand
            // 
            this.tbTrqDeadBand.LargeChange = 10;
            this.tbTrqDeadBand.Location = new System.Drawing.Point(459, 105);
            this.tbTrqDeadBand.Maximum = 50;
            this.tbTrqDeadBand.Name = "tbTrqDeadBand";
            this.tbTrqDeadBand.Size = new System.Drawing.Size(104, 45);
            this.tbTrqDeadBand.TabIndex = 30;
            this.tbTrqDeadBand.TickFrequency = 5;
            this.tbTrqDeadBand.Scroll += new System.EventHandler(this.tbTrqDeadBand_Scroll);
            // 
            // txtPowerLaw
            // 
            this.txtPowerLaw.Location = new System.Drawing.Point(569, 70);
            this.txtPowerLaw.Name = "txtPowerLaw";
            this.txtPowerLaw.Size = new System.Drawing.Size(57, 20);
            this.txtPowerLaw.TabIndex = 29;
            this.txtPowerLaw.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPowerLaw_KeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(393, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 28;
            this.label6.Text = "Power law";
            // 
            // tbPowerLaw
            // 
            this.tbPowerLaw.Location = new System.Drawing.Point(459, 68);
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
            this.chkSkipStopEffect.Location = new System.Drawing.Point(12, 38);
            this.chkSkipStopEffect.Name = "chkSkipStopEffect";
            this.chkSkipStopEffect.Size = new System.Drawing.Size(108, 17);
            this.chkSkipStopEffect.TabIndex = 26;
            this.chkSkipStopEffect.Text = "Skip Stop Effects";
            this.chkSkipStopEffect.UseVisualStyleBackColor = true;
            this.chkSkipStopEffect.Click += new System.EventHandler(this.chkSkipStopEffect_Click);
            // 
            // txtGlobalGain
            // 
            this.txtGlobalGain.Location = new System.Drawing.Point(569, 33);
            this.txtGlobalGain.Name = "txtGlobalGain";
            this.txtGlobalGain.Size = new System.Drawing.Size(57, 20);
            this.txtGlobalGain.TabIndex = 25;
            this.txtGlobalGain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGlobalGain_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(393, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Global gain";
            // 
            // tbGlobalGain
            // 
            this.tbGlobalGain.Location = new System.Drawing.Point(459, 31);
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
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(558, 16);
            this.label4.TabIndex = 23;
            this.label4.Text = "General effect parameters (some may not be changed when manager is running)";
            // 
            // chkEmulateMissing
            // 
            this.chkEmulateMissing.AutoSize = true;
            this.chkEmulateMissing.Enabled = false;
            this.chkEmulateMissing.Location = new System.Drawing.Point(12, 59);
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
            this.chkPulsedTrq.Location = new System.Drawing.Point(12, 105);
            this.chkPulsedTrq.Name = "chkPulsedTrq";
            this.chkPulsedTrq.Size = new System.Drawing.Size(380, 17);
            this.chkPulsedTrq.TabIndex = 2;
            this.chkPulsedTrq.Text = "Use quarter-pulsed Torque (increase torque resolution but introduce ripples)";
            this.chkPulsedTrq.UseVisualStyleBackColor = true;
            this.chkPulsedTrq.Click += new System.EventHandler(this.chkPulsedTrq_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Spring Kp gain";
            // 
            // tpSpring_Kp
            // 
            this.tpSpring_Kp.Location = new System.Drawing.Point(81, 14);
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
            this.txtSpring_Kp.Location = new System.Drawing.Point(191, 16);
            this.txtSpring_Kp.Name = "txtSpring_Kp";
            this.txtSpring_Kp.Size = new System.Drawing.Size(57, 20);
            this.txtSpring_Kp.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "Spring Bv";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(81, 47);
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
            this.textBox1.Location = new System.Drawing.Point(191, 49);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(57, 20);
            this.textBox1.TabIndex = 31;
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
            ((System.ComponentModel.ISupportInitialize)(this.tbTrqDeadBand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbPowerLaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbGlobalGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tpSpring_Kp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
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
    }
}

