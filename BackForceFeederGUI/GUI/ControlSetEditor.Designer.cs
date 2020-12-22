namespace BackForceFeederGUI.GUI
{
    partial class ControlSetEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlSetEditor));
            this.btnAddNew = new System.Windows.Forms.Button();
            this.txtControlSetUniqueName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.txtGameName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbPriorityLevel = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbOutputType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMainWindowTitle = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbExecType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtExecProcessName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.lsvControlSets = new System.Windows.Forms.ListView();
            this.btnSelectCurrent = new System.Windows.Forms.Button();
            this.btnCopyButtonsTo = new System.Windows.Forms.Button();
            this.btnCopyAxesTo = new System.Windows.Forms.Button();
            this.btnCopyOutputsTo = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddNew
            // 
            this.btnAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNew.Location = new System.Drawing.Point(387, 454);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(67, 23);
            this.btnAddNew.TabIndex = 13;
            this.btnAddNew.Text = "Add new";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtControlSetUniqueName
            // 
            this.txtControlSetUniqueName.Location = new System.Drawing.Point(197, 7);
            this.txtControlSetUniqueName.Name = "txtControlSetUniqueName";
            this.txtControlSetUniqueName.Size = new System.Drawing.Size(162, 20);
            this.txtControlSetUniqueName.TabIndex = 2;
            this.txtControlSetUniqueName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtControlSetUniqueName_KeyPress);
            this.txtControlSetUniqueName.Leave += new System.EventHandler(this.txtControlSetUniqueName_Leave);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(694, 454);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 23);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Location = new System.Drawing.Point(460, 454);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(67, 23);
            this.btnRemove.TabIndex = 14;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // txtGameName
            // 
            this.txtGameName.Location = new System.Drawing.Point(197, 33);
            this.txtGameName.Name = "txtGameName";
            this.txtGameName.Size = new System.Drawing.Size(162, 20);
            this.txtGameName.TabIndex = 3;
            this.txtGameName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGameName_KeyPress);
            this.txtGameName.Leave += new System.EventHandler(this.txtGameName_Leave);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.cmbPriorityLevel);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cmbOutputType);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtMainWindowTitle);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cmbExecType);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtExecProcessName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtGameName);
            this.panel1.Controls.Add(this.txtControlSetUniqueName);
            this.panel1.Location = new System.Drawing.Point(387, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(368, 403);
            this.panel1.TabIndex = 6;
            // 
            // cmbPriorityLevel
            // 
            this.cmbPriorityLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPriorityLevel.FormattingEnabled = true;
            this.cmbPriorityLevel.Location = new System.Drawing.Point(197, 138);
            this.cmbPriorityLevel.Name = "cmbPriorityLevel";
            this.cmbPriorityLevel.Size = new System.Drawing.Size(162, 21);
            this.cmbPriorityLevel.TabIndex = 7;
            this.cmbPriorityLevel.SelectedIndexChanged += new System.EventHandler(this.cmbPriorityLevel_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(161, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Order priority (if multiple matches)";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 204);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(360, 199);
            this.label7.TabIndex = 16;
            this.label7.Text = resources.GetString("label7.Text");
            // 
            // cmbOutputType
            // 
            this.cmbOutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOutputType.FormattingEnabled = true;
            this.cmbOutputType.Location = new System.Drawing.Point(197, 165);
            this.cmbOutputType.Name = "cmbOutputType";
            this.cmbOutputType.Size = new System.Drawing.Size(162, 21);
            this.cmbOutputType.TabIndex = 8;
            this.cmbOutputType.SelectedIndexChanged += new System.EventHandler(this.cmbOutputType_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Output type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Main window title";
            // 
            // txtMainWindowTitle
            // 
            this.txtMainWindowTitle.Location = new System.Drawing.Point(197, 112);
            this.txtMainWindowTitle.Name = "txtMainWindowTitle";
            this.txtMainWindowTitle.Size = new System.Drawing.Size(162, 20);
            this.txtMainWindowTitle.TabIndex = 6;
            this.txtMainWindowTitle.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMainWindowTitle_KeyPress);
            this.txtMainWindowTitle.Leave += new System.EventHandler(this.txtMainWindowTitle_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 89);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Exec. process name";
            // 
            // cmbExecType
            // 
            this.cmbExecType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExecType.FormattingEnabled = true;
            this.cmbExecType.Location = new System.Drawing.Point(197, 59);
            this.cmbExecType.Name = "cmbExecType";
            this.cmbExecType.Size = new System.Drawing.Size(162, 21);
            this.cmbExecType.TabIndex = 4;
            this.cmbExecType.SelectedIndexChanged += new System.EventHandler(this.cmbExecTypeOutput_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Exec. type (for autodetection)";
            // 
            // txtExecProcessName
            // 
            this.txtExecProcessName.Location = new System.Drawing.Point(197, 86);
            this.txtExecProcessName.Name = "txtExecProcessName";
            this.txtExecProcessName.Size = new System.Drawing.Size(162, 20);
            this.txtExecProcessName.TabIndex = 5;
            this.txtExecProcessName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExecProcessName_KeyPress);
            this.txtExecProcessName.Leave += new System.EventHandler(this.txtExecProcessName_Leave);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Game name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Controlset name (filename)";
            // 
            // btnDuplicate
            // 
            this.btnDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDuplicate.Location = new System.Drawing.Point(533, 454);
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(67, 23);
            this.btnDuplicate.TabIndex = 15;
            this.btnDuplicate.Text = "Duplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            this.btnDuplicate.Click += new System.EventHandler(this.btnDuplicate_Click);
            // 
            // lsvControlSets
            // 
            this.lsvControlSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvControlSets.HideSelection = false;
            this.lsvControlSets.Location = new System.Drawing.Point(12, 12);
            this.lsvControlSets.MultiSelect = false;
            this.lsvControlSets.Name = "lsvControlSets";
            this.lsvControlSets.Size = new System.Drawing.Size(369, 465);
            this.lsvControlSets.TabIndex = 1;
            this.lsvControlSets.UseCompatibleStateImageBehavior = false;
            this.lsvControlSets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsvControlSets_ColumnClick);
            this.lsvControlSets.SelectedIndexChanged += new System.EventHandler(this.lsvControlSets_SelectedIndexChanged);
            // 
            // btnSelectCurrent
            // 
            this.btnSelectCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectCurrent.Location = new System.Drawing.Point(387, 425);
            this.btnSelectCurrent.Name = "btnSelectCurrent";
            this.btnSelectCurrent.Size = new System.Drawing.Size(86, 23);
            this.btnSelectCurrent.TabIndex = 9;
            this.btnSelectCurrent.Text = "Select Current";
            this.btnSelectCurrent.UseVisualStyleBackColor = true;
            this.btnSelectCurrent.Click += new System.EventHandler(this.btnCurrent_Click);
            // 
            // btnCopyButtonsTo
            // 
            this.btnCopyButtonsTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyButtonsTo.Location = new System.Drawing.Point(479, 425);
            this.btnCopyButtonsTo.Name = "btnCopyButtonsTo";
            this.btnCopyButtonsTo.Size = new System.Drawing.Size(86, 23);
            this.btnCopyButtonsTo.TabIndex = 10;
            this.btnCopyButtonsTo.Text = "Copy Buttons";
            this.btnCopyButtonsTo.UseVisualStyleBackColor = true;
            this.btnCopyButtonsTo.Visible = false;
            // 
            // btnCopyAxesTo
            // 
            this.btnCopyAxesTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyAxesTo.Location = new System.Drawing.Point(571, 425);
            this.btnCopyAxesTo.Name = "btnCopyAxesTo";
            this.btnCopyAxesTo.Size = new System.Drawing.Size(86, 23);
            this.btnCopyAxesTo.TabIndex = 11;
            this.btnCopyAxesTo.Text = "Copy axes";
            this.btnCopyAxesTo.UseVisualStyleBackColor = true;
            this.btnCopyAxesTo.Visible = false;
            // 
            // btnCopyOutputsTo
            // 
            this.btnCopyOutputsTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyOutputsTo.Location = new System.Drawing.Point(663, 425);
            this.btnCopyOutputsTo.Name = "btnCopyOutputsTo";
            this.btnCopyOutputsTo.Size = new System.Drawing.Size(86, 23);
            this.btnCopyOutputsTo.TabIndex = 12;
            this.btnCopyOutputsTo.Text = "Copy outputs";
            this.btnCopyOutputsTo.UseVisualStyleBackColor = true;
            this.btnCopyOutputsTo.Visible = false;
            // 
            // ControlSetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 489);
            this.Controls.Add(this.btnCopyOutputsTo);
            this.Controls.Add(this.btnCopyAxesTo);
            this.Controls.Add(this.btnCopyButtonsTo);
            this.Controls.Add(this.btnSelectCurrent);
            this.Controls.Add(this.lsvControlSets);
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAddNew);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ControlSetEditor";
            this.Text = "Control set editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlSetEditor_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.TextBox txtControlSetUniqueName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.TextBox txtGameName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMainWindowTitle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbExecType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtExecProcessName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDuplicate;
        private System.Windows.Forms.ListView lsvControlSets;
        private System.Windows.Forms.Button btnSelectCurrent;
        private System.Windows.Forms.ComboBox cmbOutputType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnCopyButtonsTo;
        private System.Windows.Forms.Button btnCopyAxesTo;
        private System.Windows.Forms.Button btnCopyOutputsTo;
        private System.Windows.Forms.ComboBox cmbPriorityLevel;
        private System.Windows.Forms.Label label8;
    }
}