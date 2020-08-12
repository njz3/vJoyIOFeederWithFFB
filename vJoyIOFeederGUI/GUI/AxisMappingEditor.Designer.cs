namespace vJoyIOFeederGUI.GUI
{
    partial class AxisMappingEditor
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
            this.grpAxisMap = new LiveCharts.WinForms.CartesianChart();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lbSelectedPoint = new System.Windows.Forms.Label();
            this.trValueX = new System.Windows.Forms.TrackBar();
            this.trValueY = new System.Windows.Forms.TrackBar();
            this.chkFullRangePedal = new System.Windows.Forms.CheckBox();
            this.chkNegPedal = new System.Windows.Forms.CheckBox();
            this.btnCalibratePedal = new System.Windows.Forms.Button();
            this.btCalibrateWheel = new System.Windows.Forms.Button();
            this.btnDeleteCP = new System.Windows.Forms.Button();
            this.btnAddCP = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trValueX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trValueY)).BeginInit();
            this.SuspendLayout();
            // 
            // grpAxisMap
            // 
            this.grpAxisMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAxisMap.Location = new System.Drawing.Point(3, 36);
            this.grpAxisMap.Name = "grpAxisMap";
            this.grpAxisMap.Size = new System.Drawing.Size(501, 286);
            this.grpAxisMap.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(395, 35);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(476, 35);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.lbSelectedPoint);
            this.splitContainer1.Panel1.Controls.Add(this.trValueX);
            this.splitContainer1.Panel1.Controls.Add(this.trValueY);
            this.splitContainer1.Panel1.Controls.Add(this.grpAxisMap);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chkFullRangePedal);
            this.splitContainer1.Panel2.Controls.Add(this.chkNegPedal);
            this.splitContainer1.Panel2.Controls.Add(this.btnCalibratePedal);
            this.splitContainer1.Panel2.Controls.Add(this.btCalibrateWheel);
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteCP);
            this.splitContainer1.Panel2.Controls.Add(this.btnAddCP);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnOK);
            this.splitContainer1.Size = new System.Drawing.Size(575, 445);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.TabIndex = 25;
            // 
            // lbSelectedPoint
            // 
            this.lbSelectedPoint.AutoSize = true;
            this.lbSelectedPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSelectedPoint.Location = new System.Drawing.Point(46, 12);
            this.lbSelectedPoint.Name = "lbSelectedPoint";
            this.lbSelectedPoint.Size = new System.Drawing.Size(156, 16);
            this.lbSelectedPoint.TabIndex = 27;
            this.lbSelectedPoint.Text = "Click to select a point";
            // 
            // trValueX
            // 
            this.trValueX.Location = new System.Drawing.Point(30, 328);
            this.trValueX.Maximum = 100;
            this.trValueX.Name = "trValueX";
            this.trValueX.Size = new System.Drawing.Size(460, 45);
            this.trValueX.TabIndex = 26;
            this.trValueX.TickFrequency = 10;
            this.trValueX.ValueChanged += new System.EventHandler(this.txValueX_ValueChanged);
            // 
            // trValueY
            // 
            this.trValueY.Location = new System.Drawing.Point(518, 33);
            this.trValueY.Maximum = 100;
            this.trValueY.Name = "trValueY";
            this.trValueY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trValueY.Size = new System.Drawing.Size(45, 262);
            this.trValueY.TabIndex = 25;
            this.trValueY.TickFrequency = 10;
            this.trValueY.ValueChanged += new System.EventHandler(this.trValueY_ValueChanged);
            // 
            // chkFullRangePedal
            // 
            this.chkFullRangePedal.AutoSize = true;
            this.chkFullRangePedal.Location = new System.Drawing.Point(198, 39);
            this.chkFullRangePedal.Name = "chkFullRangePedal";
            this.chkFullRangePedal.Size = new System.Drawing.Size(108, 17);
            this.chkFullRangePedal.TabIndex = 35;
            this.chkFullRangePedal.Text = "Full range Pedal?";
            this.chkFullRangePedal.UseVisualStyleBackColor = true;
            // 
            // chkNegPedal
            // 
            this.chkNegPedal.AutoSize = true;
            this.chkNegPedal.Location = new System.Drawing.Point(198, 10);
            this.chkNegPedal.Name = "chkNegPedal";
            this.chkNegPedal.Size = new System.Drawing.Size(105, 17);
            this.chkNegPedal.TabIndex = 34;
            this.chkNegPedal.Text = "Negative Pedal?";
            this.chkNegPedal.UseVisualStyleBackColor = true;
            // 
            // btnCalibratePedal
            // 
            this.btnCalibratePedal.Location = new System.Drawing.Point(105, 35);
            this.btnCalibratePedal.Name = "btnCalibratePedal";
            this.btnCalibratePedal.Size = new System.Drawing.Size(87, 23);
            this.btnCalibratePedal.TabIndex = 31;
            this.btnCalibratePedal.Text = "Calibrate pedal";
            this.btnCalibratePedal.UseVisualStyleBackColor = true;
            this.btnCalibratePedal.Click += new System.EventHandler(this.btnCalibratePedal_Click);
            // 
            // btCalibrateWheel
            // 
            this.btCalibrateWheel.Location = new System.Drawing.Point(105, 6);
            this.btCalibrateWheel.Name = "btCalibrateWheel";
            this.btCalibrateWheel.Size = new System.Drawing.Size(87, 23);
            this.btCalibrateWheel.TabIndex = 30;
            this.btCalibrateWheel.Text = "Calibrate wheel";
            this.btCalibrateWheel.UseVisualStyleBackColor = true;
            this.btCalibrateWheel.Click += new System.EventHandler(this.btCalibrateWheel_Click);
            // 
            // btnDeleteCP
            // 
            this.btnDeleteCP.Location = new System.Drawing.Point(12, 35);
            this.btnDeleteCP.Name = "btnDeleteCP";
            this.btnDeleteCP.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteCP.TabIndex = 29;
            this.btnDeleteCP.Text = "Delete Control";
            this.btnDeleteCP.UseVisualStyleBackColor = true;
            this.btnDeleteCP.Click += new System.EventHandler(this.btnDeleteCP_Click);
            // 
            // btnAddCP
            // 
            this.btnAddCP.Location = new System.Drawing.Point(12, 6);
            this.btnAddCP.Name = "btnAddCP";
            this.btnAddCP.Size = new System.Drawing.Size(87, 23);
            this.btnAddCP.TabIndex = 27;
            this.btnAddCP.Text = "Add control";
            this.btnAddCP.UseVisualStyleBackColor = true;
            this.btnAddCP.Click += new System.EventHandler(this.btnAddCP_Click);
            // 
            // AxisMappingEditor
            // 
            this.ClientSize = new System.Drawing.Size(575, 445);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AxisMappingEditor";
            this.Text = "Axis mapping editor";
            this.Load += new System.EventHandler(this.AxisMappingEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trValueX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trValueY)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private LiveCharts.WinForms.CartesianChart grpAxisMap;
        private System.Windows.Forms.TrackBar trValueY;
        private System.Windows.Forms.TrackBar trValueX;
        private System.Windows.Forms.Button btnDeleteCP;
        private System.Windows.Forms.Button btnAddCP;
        private System.Windows.Forms.Label lbSelectedPoint;
        private System.Windows.Forms.Button btCalibrateWheel;
        private System.Windows.Forms.Button btnCalibratePedal;
        private System.Windows.Forms.CheckBox chkNegPedal;
        private System.Windows.Forms.CheckBox chkFullRangePedal;
    }
}

