namespace BackForceFeederGUI.GUI
{
    partial class CalibratePedalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lbInstructions = new System.Windows.Forms.Label();
            this.lbRawValue = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbInstructions
            // 
            this.lbInstructions.AutoSize = true;
            this.lbInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbInstructions.Location = new System.Drawing.Point(12, 9);
            this.lbInstructions.Name = "lbInstructions";
            this.lbInstructions.Size = new System.Drawing.Size(177, 16);
            this.lbInstructions.TabIndex = 0;
            this.lbInstructions.Text = "Press pedal to maximum";
            // 
            // lbRawValue
            // 
            this.lbRawValue.AutoSize = true;
            this.lbRawValue.Location = new System.Drawing.Point(12, 33);
            this.lbRawValue.Name = "lbRawValue";
            this.lbRawValue.Size = new System.Drawing.Size(64, 13);
            this.lbRawValue.TabIndex = 1;
            this.lbRawValue.Text = "Raw value=";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(16, 112);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(95, 32);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(165, 112);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(94, 32);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbResult
            // 
            this.lbResult.AutoSize = true;
            this.lbResult.Location = new System.Drawing.Point(12, 52);
            this.lbResult.Name = "lbResult";
            this.lbResult.Size = new System.Drawing.Size(40, 13);
            this.lbResult.TabIndex = 4;
            this.lbResult.Text = "Result:";
            // 
            // CalibratePedalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 158);
            this.Controls.Add(this.lbResult);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbRawValue);
            this.Controls.Add(this.lbInstructions);
            this.Name = "CalibratePedalForm";
            this.Text = "Pedal calibration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbInstructions;
        private System.Windows.Forms.Label lbRawValue;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbResult;
    }
}