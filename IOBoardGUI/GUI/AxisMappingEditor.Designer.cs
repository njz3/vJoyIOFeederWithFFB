namespace IOFeederGUI.GUI
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
            this.trValueX = new System.Windows.Forms.TrackBar();
            this.trValueY = new System.Windows.Forms.TrackBar();
            this.btnDeleteCP = new System.Windows.Forms.Button();
            this.btnAddCP = new System.Windows.Forms.Button();
            this.lbSelectedPoint = new System.Windows.Forms.Label();
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
            this.grpAxisMap.Location = new System.Drawing.Point(3, 5);
            this.grpAxisMap.Name = "grpAxisMap";
            this.grpAxisMap.Size = new System.Drawing.Size(389, 284);
            this.grpAxisMap.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(279, 11);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(360, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
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
            this.splitContainer1.Panel2.Controls.Add(this.btnDeleteCP);
            this.splitContainer1.Panel2.Controls.Add(this.btnAddCP);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnOK);
            this.splitContainer1.Size = new System.Drawing.Size(446, 388);
            this.splitContainer1.SplitterDistance = 340;
            this.splitContainer1.TabIndex = 25;
            // 
            // trValueX
            // 
            this.trValueX.Location = new System.Drawing.Point(37, 286);
            this.trValueX.Maximum = 100;
            this.trValueX.Name = "trValueX";
            this.trValueX.Size = new System.Drawing.Size(336, 45);
            this.trValueX.TabIndex = 26;
            this.trValueX.TickFrequency = 10;
            this.trValueX.ValueChanged += new System.EventHandler(this.txValueX_ValueChanged);
            // 
            // trValueY
            // 
            this.trValueY.Location = new System.Drawing.Point(398, 5);
            this.trValueY.Maximum = 100;
            this.trValueY.Name = "trValueY";
            this.trValueY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trValueY.Size = new System.Drawing.Size(45, 256);
            this.trValueY.TabIndex = 25;
            this.trValueY.TickFrequency = 10;
            this.trValueY.ValueChanged += new System.EventHandler(this.trValueY_ValueChanged);
            // 
            // btnDeleteCP
            // 
            this.btnDeleteCP.Location = new System.Drawing.Point(96, 11);
            this.btnDeleteCP.Name = "btnDeleteCP";
            this.btnDeleteCP.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteCP.TabIndex = 29;
            this.btnDeleteCP.Text = "Delete Control";
            this.btnDeleteCP.UseVisualStyleBackColor = true;
            this.btnDeleteCP.Click += new System.EventHandler(this.btnDeleteCP_Click);
            // 
            // btnAddCP
            // 
            this.btnAddCP.Location = new System.Drawing.Point(3, 11);
            this.btnAddCP.Name = "btnAddCP";
            this.btnAddCP.Size = new System.Drawing.Size(87, 23);
            this.btnAddCP.TabIndex = 27;
            this.btnAddCP.Text = "Add control";
            this.btnAddCP.UseVisualStyleBackColor = true;
            this.btnAddCP.Click += new System.EventHandler(this.btnAddCP_Click);
            // 
            // lbSelectedPoint
            // 
            this.lbSelectedPoint.AutoSize = true;
            this.lbSelectedPoint.Location = new System.Drawing.Point(132, 5);
            this.lbSelectedPoint.Name = "lbSelectedPoint";
            this.lbSelectedPoint.Size = new System.Drawing.Size(78, 13);
            this.lbSelectedPoint.TabIndex = 27;
            this.lbSelectedPoint.Text = "Selected point:";
            // 
            // AxisMappingEditor
            // 
            this.ClientSize = new System.Drawing.Size(470, 412);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AxisMappingEditor";
            this.Load += new System.EventHandler(this.AxisMappingEditor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
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
    }
}

