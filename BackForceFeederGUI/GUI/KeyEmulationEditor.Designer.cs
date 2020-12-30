namespace BackForceFeederGUI.GUI
{
    partial class KeyEmulationEditor
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
            this.btnAddNew = new System.Windows.Forms.Button();
            this.txtKeyRuleName = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbKeyStroke2 = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtAxisTolerance_pct = new System.Windows.Forms.TextBox();
            this.txtExpr = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSourceIndex3 = new System.Windows.Forms.TextBox();
            this.txtSourceIndex2 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSourceIndex1 = new System.Windows.Forms.TextBox();
            this.chkSign3 = new System.Windows.Forms.CheckBox();
            this.chkSign2 = new System.Windows.Forms.CheckBox();
            this.txtThreshold3 = new System.Windows.Forms.TextBox();
            this.txtThreshold2 = new System.Windows.Forms.TextBox();
            this.cmbKeyAPI = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbKeyStroke1 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkSign1 = new System.Windows.Forms.CheckBox();
            this.cmbSourceType3 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbCombine2 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHoldTimes_ms = new System.Windows.Forms.TextBox();
            this.chkTestValue = new System.Windows.Forms.CheckBox();
            this.chkIsInversed = new System.Windows.Forms.CheckBox();
            this.cmbSourceType2 = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbCombine1 = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbSourceType1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtThreshold1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDuplicate = new System.Windows.Forms.Button();
            this.lsvKeyRulesSets = new System.Windows.Forms.ListView();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddNew
            // 
            this.btnAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNew.Location = new System.Drawing.Point(325, 312);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(67, 23);
            this.btnAddNew.TabIndex = 13;
            this.btnAddNew.Text = "Add new";
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtKeyRuleName
            // 
            this.txtKeyRuleName.Location = new System.Drawing.Point(147, 7);
            this.txtKeyRuleName.Name = "txtKeyRuleName";
            this.txtKeyRuleName.Size = new System.Drawing.Size(162, 20);
            this.txtKeyRuleName.TabIndex = 2;
            this.txtKeyRuleName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRuleUniqueName_KeyPress);
            this.txtKeyRuleName.Leave += new System.EventHandler(this.txtRuleUniqueName_Leave);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(810, 312);
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
            this.btnRemove.Location = new System.Drawing.Point(398, 312);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(67, 23);
            this.btnRemove.TabIndex = 14;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.cmbKeyStroke2);
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.txtAxisTolerance_pct);
            this.panel1.Controls.Add(this.txtExpr);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.txtSourceIndex3);
            this.panel1.Controls.Add(this.txtSourceIndex2);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.txtSourceIndex1);
            this.panel1.Controls.Add(this.chkSign3);
            this.panel1.Controls.Add(this.chkSign2);
            this.panel1.Controls.Add(this.txtThreshold3);
            this.panel1.Controls.Add(this.txtThreshold2);
            this.panel1.Controls.Add(this.cmbKeyAPI);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.cmbKeyStroke1);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.chkSign1);
            this.panel1.Controls.Add(this.cmbSourceType3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cmbCombine2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtHoldTimes_ms);
            this.panel1.Controls.Add(this.chkTestValue);
            this.panel1.Controls.Add(this.chkIsInversed);
            this.panel1.Controls.Add(this.cmbSourceType2);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.cmbCombine1);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cmbSourceType1);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtThreshold1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtKeyRuleName);
            this.panel1.Location = new System.Drawing.Point(325, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(547, 294);
            this.panel1.TabIndex = 6;
            // 
            // cmbKeyStroke2
            // 
            this.cmbKeyStroke2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKeyStroke2.FormattingEnabled = true;
            this.cmbKeyStroke2.Location = new System.Drawing.Point(231, 232);
            this.cmbKeyStroke2.Name = "cmbKeyStroke2";
            this.cmbKeyStroke2.Size = new System.Drawing.Size(77, 21);
            this.cmbKeyStroke2.TabIndex = 45;
            this.cmbKeyStroke2.SelectedIndexChanged += new System.EventHandler(this.cmbKeyStroke2_SelectedIndexChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(2, 184);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(90, 13);
            this.label14.TabIndex = 44;
            this.label14.Text = "Axis tolerance (%)";
            // 
            // txtAxisTolerance_pct
            // 
            this.txtAxisTolerance_pct.Location = new System.Drawing.Point(147, 181);
            this.txtAxisTolerance_pct.Name = "txtAxisTolerance_pct";
            this.txtAxisTolerance_pct.Size = new System.Drawing.Size(113, 20);
            this.txtAxisTolerance_pct.TabIndex = 43;
            this.txtAxisTolerance_pct.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAxisTolerance_pct_KeyPress);
            this.txtAxisTolerance_pct.Leave += new System.EventHandler(this.txtAxisTolerance_pct_Leave);
            // 
            // txtExpr
            // 
            this.txtExpr.Enabled = false;
            this.txtExpr.Location = new System.Drawing.Point(147, 152);
            this.txtExpr.Name = "txtExpr";
            this.txtExpr.Size = new System.Drawing.Size(388, 20);
            this.txtExpr.TabIndex = 42;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(2, 153);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(58, 13);
            this.label13.TabIndex = 41;
            this.label13.Text = "Expression";
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(337, 175);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(208, 90);
            this.label12.TabIndex = 40;
            this.label12.Text = "Notes on axis threshold:\r\nValue is expected to be a percent between 0 and 1.0.\r\nF" +
    "or Wheel or pedal axis, typical values are 0.25 (inv.dir) or 0.75.\r\n";
            // 
            // txtSourceIndex3
            // 
            this.txtSourceIndex3.Location = new System.Drawing.Point(269, 104);
            this.txtSourceIndex3.Name = "txtSourceIndex3";
            this.txtSourceIndex3.Size = new System.Drawing.Size(63, 20);
            this.txtSourceIndex3.TabIndex = 38;
            this.txtSourceIndex3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSourceIndex3_KeyPress);
            this.txtSourceIndex3.Leave += new System.EventHandler(this.txtSourceIndex3_Leave);
            // 
            // txtSourceIndex2
            // 
            this.txtSourceIndex2.Location = new System.Drawing.Point(269, 77);
            this.txtSourceIndex2.Name = "txtSourceIndex2";
            this.txtSourceIndex2.Size = new System.Drawing.Size(63, 20);
            this.txtSourceIndex2.TabIndex = 37;
            this.txtSourceIndex2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSourceIndex2_KeyPress);
            this.txtSourceIndex2.Leave += new System.EventHandler(this.txtSourceIndex2_Leave);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(272, 33);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(44, 13);
            this.label11.TabIndex = 36;
            this.label11.Text = "Number";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(150, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Type";
            // 
            // txtSourceIndex1
            // 
            this.txtSourceIndex1.Location = new System.Drawing.Point(269, 51);
            this.txtSourceIndex1.Name = "txtSourceIndex1";
            this.txtSourceIndex1.Size = new System.Drawing.Size(63, 20);
            this.txtSourceIndex1.TabIndex = 34;
            this.txtSourceIndex1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSourceIndex1_KeyPress);
            this.txtSourceIndex1.Leave += new System.EventHandler(this.txtSourceIndex1_Leave);
            // 
            // chkSign3
            // 
            this.chkSign3.AutoSize = true;
            this.chkSign3.Location = new System.Drawing.Point(420, 105);
            this.chkSign3.Name = "chkSign3";
            this.chkSign3.Size = new System.Drawing.Size(44, 17);
            this.chkSign3.TabIndex = 33;
            this.chkSign3.Text = "Inv.";
            this.chkSign3.UseVisualStyleBackColor = true;
            this.chkSign3.Click += new System.EventHandler(this.chkSign3_Click);
            // 
            // chkSign2
            // 
            this.chkSign2.AutoSize = true;
            this.chkSign2.Location = new System.Drawing.Point(420, 78);
            this.chkSign2.Name = "chkSign2";
            this.chkSign2.Size = new System.Drawing.Size(44, 17);
            this.chkSign2.TabIndex = 32;
            this.chkSign2.Text = "Inv.";
            this.chkSign2.UseVisualStyleBackColor = true;
            this.chkSign2.Click += new System.EventHandler(this.chkSign2_Click);
            // 
            // txtThreshold3
            // 
            this.txtThreshold3.Location = new System.Drawing.Point(341, 104);
            this.txtThreshold3.Name = "txtThreshold3";
            this.txtThreshold3.Size = new System.Drawing.Size(63, 20);
            this.txtThreshold3.TabIndex = 31;
            this.txtThreshold3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtThreshold3_KeyPress);
            this.txtThreshold3.Leave += new System.EventHandler(this.txtThreshold3_Leave);
            // 
            // txtThreshold2
            // 
            this.txtThreshold2.Location = new System.Drawing.Point(341, 77);
            this.txtThreshold2.Name = "txtThreshold2";
            this.txtThreshold2.Size = new System.Drawing.Size(63, 20);
            this.txtThreshold2.TabIndex = 30;
            this.txtThreshold2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtThreshold2_KeyPress);
            this.txtThreshold2.Leave += new System.EventHandler(this.txtThreshold2_Leave);
            // 
            // cmbKeyAPI
            // 
            this.cmbKeyAPI.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKeyAPI.FormattingEnabled = true;
            this.cmbKeyAPI.Location = new System.Drawing.Point(147, 259);
            this.cmbKeyAPI.Name = "cmbKeyAPI";
            this.cmbKeyAPI.Size = new System.Drawing.Size(162, 21);
            this.cmbKeyAPI.TabIndex = 28;
            this.cmbKeyAPI.SelectedIndexChanged += new System.EventHandler(this.cmbKeyAPI_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(2, 262);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "Key API";
            // 
            // cmbKeyStroke1
            // 
            this.cmbKeyStroke1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKeyStroke1.FormattingEnabled = true;
            this.cmbKeyStroke1.Location = new System.Drawing.Point(147, 232);
            this.cmbKeyStroke1.Name = "cmbKeyStroke1";
            this.cmbKeyStroke1.Size = new System.Drawing.Size(77, 21);
            this.cmbKeyStroke1.TabIndex = 26;
            this.cmbKeyStroke1.SelectedIndexChanged += new System.EventHandler(this.cmbKeyStroke1_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(2, 235);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 27;
            this.label7.Text = "Key stroke";
            // 
            // chkSign1
            // 
            this.chkSign1.AutoSize = true;
            this.chkSign1.Location = new System.Drawing.Point(420, 52);
            this.chkSign1.Name = "chkSign1";
            this.chkSign1.Size = new System.Drawing.Size(44, 17);
            this.chkSign1.TabIndex = 25;
            this.chkSign1.Text = "Inv.";
            this.chkSign1.UseVisualStyleBackColor = true;
            this.chkSign1.Click += new System.EventHandler(this.chkSign1_Click);
            // 
            // cmbSourceType3
            // 
            this.cmbSourceType3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceType3.FormattingEnabled = true;
            this.cmbSourceType3.Location = new System.Drawing.Point(147, 104);
            this.cmbSourceType3.Name = "cmbSourceType3";
            this.cmbSourceType3.Size = new System.Drawing.Size(113, 21);
            this.cmbSourceType3.TabIndex = 23;
            this.cmbSourceType3.SelectedIndexChanged += new System.EventHandler(this.cmbSourceType3_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 13);
            this.label5.TabIndex = 24;
            this.label5.Text = "Third source (if combined)";
            // 
            // cmbCombine2
            // 
            this.cmbCombine2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCombine2.FormattingEnabled = true;
            this.cmbCombine2.Location = new System.Drawing.Point(472, 89);
            this.cmbCombine2.Name = "cmbCombine2";
            this.cmbCombine2.Size = new System.Drawing.Size(63, 21);
            this.cmbCombine2.TabIndex = 22;
            this.cmbCombine2.SelectedIndexChanged += new System.EventHandler(this.cmbCombine2_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 209);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 21;
            this.label2.Text = "Hold time (ms)";
            // 
            // txtHoldTimes_ms
            // 
            this.txtHoldTimes_ms.Location = new System.Drawing.Point(147, 206);
            this.txtHoldTimes_ms.Name = "txtHoldTimes_ms";
            this.txtHoldTimes_ms.Size = new System.Drawing.Size(113, 20);
            this.txtHoldTimes_ms.TabIndex = 20;
            this.txtHoldTimes_ms.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHoldTimes_ms_KeyPress);
            this.txtHoldTimes_ms.Leave += new System.EventHandler(this.txtHoldTimes_ms_Leave);
            // 
            // chkTestValue
            // 
            this.chkTestValue.AutoSize = true;
            this.chkTestValue.Enabled = false;
            this.chkTestValue.Location = new System.Drawing.Point(220, 131);
            this.chkTestValue.Name = "chkTestValue";
            this.chkTestValue.Size = new System.Drawing.Size(76, 17);
            this.chkTestValue.TabIndex = 19;
            this.chkTestValue.Text = "Test value";
            this.chkTestValue.UseVisualStyleBackColor = true;
            this.chkTestValue.Click += new System.EventHandler(this.chkTestValue_Click);
            // 
            // chkIsInversed
            // 
            this.chkIsInversed.AutoSize = true;
            this.chkIsInversed.Location = new System.Drawing.Point(147, 131);
            this.chkIsInversed.Name = "chkIsInversed";
            this.chkIsInversed.Size = new System.Drawing.Size(67, 17);
            this.chkIsInversed.TabIndex = 18;
            this.chkIsInversed.Text = "Inversed";
            this.chkIsInversed.UseVisualStyleBackColor = true;
            this.chkIsInversed.Click += new System.EventHandler(this.chkIsInversed_Click);
            // 
            // cmbSourceType2
            // 
            this.cmbSourceType2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceType2.FormattingEnabled = true;
            this.cmbSourceType2.Location = new System.Drawing.Point(147, 77);
            this.cmbSourceType2.Name = "cmbSourceType2";
            this.cmbSourceType2.Size = new System.Drawing.Size(113, 21);
            this.cmbSourceType2.TabIndex = 7;
            this.cmbSourceType2.SelectedIndexChanged += new System.EventHandler(this.cmbSourceType2_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(2, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Second source (if combined)";
            // 
            // cmbCombine1
            // 
            this.cmbCombine1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCombine1.FormattingEnabled = true;
            this.cmbCombine1.Location = new System.Drawing.Point(472, 62);
            this.cmbCombine1.Name = "cmbCombine1";
            this.cmbCombine1.Size = new System.Drawing.Size(63, 21);
            this.cmbCombine1.TabIndex = 8;
            this.cmbCombine1.SelectedIndexChanged += new System.EventHandler(this.cmbCombine1_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(459, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Logical operation";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Threshold (Axis %)";
            // 
            // cmbSourceType1
            // 
            this.cmbSourceType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSourceType1.FormattingEnabled = true;
            this.cmbSourceType1.Location = new System.Drawing.Point(147, 50);
            this.cmbSourceType1.Name = "cmbSourceType1";
            this.cmbSourceType1.Size = new System.Drawing.Size(113, 21);
            this.cmbSourceType1.TabIndex = 4;
            this.cmbSourceType1.SelectedIndexChanged += new System.EventHandler(this.cmbSourceType1_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "First source (raw input, axes)";
            // 
            // txtThreshold1
            // 
            this.txtThreshold1.Location = new System.Drawing.Point(341, 50);
            this.txtThreshold1.Name = "txtThreshold1";
            this.txtThreshold1.Size = new System.Drawing.Size(63, 20);
            this.txtThreshold1.TabIndex = 5;
            this.txtThreshold1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtThreshold1_KeyPress);
            this.txtThreshold1.Leave += new System.EventHandler(this.txtThreshold1_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Rule name";
            // 
            // btnDuplicate
            // 
            this.btnDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDuplicate.Location = new System.Drawing.Point(471, 312);
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(67, 23);
            this.btnDuplicate.TabIndex = 15;
            this.btnDuplicate.Text = "Duplicate";
            this.btnDuplicate.UseVisualStyleBackColor = true;
            this.btnDuplicate.Click += new System.EventHandler(this.btnDuplicate_Click);
            // 
            // lsvKeyRulesSets
            // 
            this.lsvKeyRulesSets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lsvKeyRulesSets.HideSelection = false;
            this.lsvKeyRulesSets.Location = new System.Drawing.Point(12, 12);
            this.lsvKeyRulesSets.MultiSelect = false;
            this.lsvKeyRulesSets.Name = "lsvKeyRulesSets";
            this.lsvKeyRulesSets.Size = new System.Drawing.Size(309, 323);
            this.lsvKeyRulesSets.TabIndex = 1;
            this.lsvKeyRulesSets.UseCompatibleStateImageBehavior = false;
            this.lsvKeyRulesSets.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsvKeyRules_ColumnClick);
            this.lsvKeyRulesSets.SelectedIndexChanged += new System.EventHandler(this.lsvControlSets_SelectedIndexChanged);
            // 
            // KeyEmulationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 347);
            this.Controls.Add(this.lsvKeyRulesSets);
            this.Controls.Add(this.btnDuplicate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAddNew);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "KeyEmulationEditor";
            this.Text = "Keystroke emulation editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlSetEditor_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.TextBox txtKeyRuleName;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbSourceType1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtThreshold1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDuplicate;
        private System.Windows.Forms.ListView lsvKeyRulesSets;
        private System.Windows.Forms.ComboBox cmbCombine1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbSourceType2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkSign1;
        private System.Windows.Forms.ComboBox cmbSourceType3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbCombine2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHoldTimes_ms;
        private System.Windows.Forms.CheckBox chkTestValue;
        private System.Windows.Forms.CheckBox chkIsInversed;
        private System.Windows.Forms.CheckBox chkSign3;
        private System.Windows.Forms.CheckBox chkSign2;
        private System.Windows.Forms.TextBox txtThreshold3;
        private System.Windows.Forms.TextBox txtThreshold2;
        private System.Windows.Forms.ComboBox cmbKeyAPI;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmbKeyStroke1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSourceIndex3;
        private System.Windows.Forms.TextBox txtSourceIndex2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSourceIndex1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtExpr;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtAxisTolerance_pct;
        private System.Windows.Forms.ComboBox cmbKeyStroke2;
    }
}