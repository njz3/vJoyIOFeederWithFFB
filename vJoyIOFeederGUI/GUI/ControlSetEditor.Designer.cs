namespace vJoyIOFeederGUI.GUI
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtControlSetUniqueName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.txtGameName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMainWindowTitle = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbExecTypeOutput = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtExecProcessName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.lsvControlSets = new System.Windows.Forms.ListView();
            this.btnCurrent = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(356, 282);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add new";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtControlSetUniqueName
            // 
            this.txtControlSetUniqueName.Location = new System.Drawing.Point(111, 7);
            this.txtControlSetUniqueName.Name = "txtControlSetUniqueName";
            this.txtControlSetUniqueName.Size = new System.Drawing.Size(143, 20);
            this.txtControlSetUniqueName.TabIndex = 2;
            this.txtControlSetUniqueName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtControlSetUniqueName_KeyPress);
            this.txtControlSetUniqueName.Leave += new System.EventHandler(this.txtControlSetUniqueName_Leave);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(581, 282);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(437, 282);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(63, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // txtGameName
            // 
            this.txtGameName.Location = new System.Drawing.Point(111, 33);
            this.txtGameName.Name = "txtGameName";
            this.txtGameName.Size = new System.Drawing.Size(143, 20);
            this.txtGameName.TabIndex = 5;
            this.txtGameName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGameName_KeyPress);
            this.txtGameName.Leave += new System.EventHandler(this.txtGameName_Leave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtMainWindowTitle);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cmbExecTypeOutput);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtExecProcessName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtGameName);
            this.panel1.Controls.Add(this.txtControlSetUniqueName);
            this.panel1.Location = new System.Drawing.Point(268, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(374, 261);
            this.panel1.TabIndex = 6;
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
            this.txtMainWindowTitle.Location = new System.Drawing.Point(111, 112);
            this.txtMainWindowTitle.Name = "txtMainWindowTitle";
            this.txtMainWindowTitle.Size = new System.Drawing.Size(143, 20);
            this.txtMainWindowTitle.TabIndex = 12;
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
            // cmbExecTypeOutput
            // 
            this.cmbExecTypeOutput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExecTypeOutput.FormattingEnabled = true;
            this.cmbExecTypeOutput.Location = new System.Drawing.Point(111, 59);
            this.cmbExecTypeOutput.Name = "cmbExecTypeOutput";
            this.cmbExecTypeOutput.Size = new System.Drawing.Size(143, 21);
            this.cmbExecTypeOutput.TabIndex = 10;
            this.cmbExecTypeOutput.SelectedIndexChanged += new System.EventHandler(this.cmbExecTypeOutput_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Exec. type (output)";
            // 
            // txtExecProcessName
            // 
            this.txtExecProcessName.Location = new System.Drawing.Point(111, 86);
            this.txtExecProcessName.Name = "txtExecProcessName";
            this.txtExecProcessName.Size = new System.Drawing.Size(143, 20);
            this.txtExecProcessName.TabIndex = 8;
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
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Controlset name";
            // 
            // btnDuplicate
            // 
            this.btnDuplicate.Location = new System.Drawing.Point(506, 282);
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(69, 23);
            this.btnDuplicate.TabIndex = 7;
            this.btnDuplicate.Text = "Duplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            this.btnDuplicate.Click += new System.EventHandler(this.btnDuplicate_Click);
            // 
            // lsvControlSets
            // 
            this.lsvControlSets.HideSelection = false;
            this.lsvControlSets.Location = new System.Drawing.Point(12, 12);
            this.lsvControlSets.MultiSelect = false;
            this.lsvControlSets.Name = "lsvControlSets";
            this.lsvControlSets.Size = new System.Drawing.Size(250, 293);
            this.lsvControlSets.TabIndex = 14;
            this.lsvControlSets.UseCompatibleStateImageBehavior = false;
            this.lsvControlSets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsvControlSets_ColumnClick);
            this.lsvControlSets.SelectedIndexChanged += new System.EventHandler(this.lsvControlSets_SelectedIndexChanged);
            // 
            // btnCurrent
            // 
            this.btnCurrent.Location = new System.Drawing.Point(268, 282);
            this.btnCurrent.Name = "btnCurrent";
            this.btnCurrent.Size = new System.Drawing.Size(67, 23);
            this.btnCurrent.TabIndex = 15;
            this.btnCurrent.Text = "Current";
            this.btnCurrent.UseVisualStyleBackColor = true;
            this.btnCurrent.Click += new System.EventHandler(this.btnCurrent_Click);
            // 
            // ControlSetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(654, 317);
            this.Controls.Add(this.btnCurrent);
            this.Controls.Add(this.lsvControlSets);
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ControlSetEditor";
            this.Text = "Control set editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlSetEditor_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtControlSetUniqueName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.TextBox txtGameName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMainWindowTitle;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbExecTypeOutput;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtExecProcessName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDuplicate;
        private System.Windows.Forms.ListView lsvControlSets;
        private System.Windows.Forms.Button btnCurrent;
    }
}