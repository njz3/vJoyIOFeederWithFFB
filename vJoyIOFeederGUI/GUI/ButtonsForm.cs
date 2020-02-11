using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;
using vJoyIOFeeder.Utils;

namespace IOFeederGUI.GUI
{

    public partial class ButtonsForm : Form
    {

        public ButtonsForm()
        {
            InitializeComponent();
        }


        List<CheckBox> AllChkBox = new List<CheckBox>();
        private void MainForm_Load(object sender, EventArgs e)
        {
            var panel = this.splitContainerMain.Panel1;

            for (int i = 1; i <= Program.Manager.vJoy.GetNumberOfButtons(); i++) {
                var chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(6 + 64*((i-1)>>3), 16 + 20*((i-1)&0b111));
                chkBox.Name = "chkBtn" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllChkBox.Add(chkBox);

                panel.Controls.Add(chkBox);

                cmbBtnMapFrom.Items.Add(i.ToString());
                cmbBtnMapTo.Items.Add(i.ToString());
            }

        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Manager.SaveConfigurationFiles(Program.ConfigPath);
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (Program.Manager.vJoy != null) {
                for (int i = 0; i < AllChkBox.Count; i++) {
                    var chk = AllChkBox[i];
                    if ((Program.Manager.vJoy.Report.Buttons & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }

            }
        }

        private void cmbBtnMapFrom_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbBtnMapTo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
