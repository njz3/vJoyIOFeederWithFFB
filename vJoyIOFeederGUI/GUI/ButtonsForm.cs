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

            for (int i = 1; i <= Program.Manager.Config.RawInputTovJoyMap.Count; i++) {
                var rawInput = new CheckBox();
                rawInput.AutoSize = true;
                rawInput.Enabled = false;
                rawInput.Location = new System.Drawing.Point(6 + 64*((i-1)>>3), 16 + 20*((i-1)&0b111));
                rawInput.Name = "chkBtn" + i;
                rawInput.Size = new System.Drawing.Size(32, 17);
                rawInput.TabIndex = i;
                rawInput.Text = i.ToString();
                rawInput.Tag = i;
                rawInput.UseVisualStyleBackColor = true;
                AllChkBox.Add(rawInput);

                panel.Controls.Add(rawInput);

                cmbBtnMapFrom.Items.Add(i.ToString());
            }

            for (int i = 1; i <= Program.Manager.vJoy.GetNumberOfButtons(); i++) {
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

        int SelectedRawInput = -1;
        private void cmbBtnMapFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbBtnMapFrom.SelectedItem.ToString(), out SelectedRawInput)) {
                SelectedRawInput = -1;
            }
            RefresList();
        }


        void RefresList()
        {
            if ((SelectedRawInput>0) && (SelectedRawInput<=Program.Manager.Config.RawInputTovJoyMap.Count)) {
                lstJoyBtn.Items.Clear();
                var btns = Program.Manager.Config.RawInputTovJoyMap[SelectedRawInput-1].vJoyBtns;
                foreach (var btn in btns) {
                    lstJoyBtn.Items.Add((btn+1).ToString());
                }
            }
        }

        int SelectedJoyBtn = -1;
        private void cmbBtnMapTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbBtnMapTo.SelectedItem.ToString(), out SelectedJoyBtn)) {
                SelectedJoyBtn = -1;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if ((SelectedRawInput>0) && (SelectedRawInput<=Program.Manager.Config.RawInputTovJoyMap.Count)) {
                if (SelectedJoyBtn>0) {
                    var btns = Program.Manager.Config.RawInputTovJoyMap[SelectedRawInput-1].vJoyBtns;
                    if (!btns.Exists(x => (x==(SelectedJoyBtn-1)))) {
                        btns.Add(SelectedJoyBtn-1);
                        btns.Sort();
                        RefresList();
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if ((SelectedRawInput>0) && (SelectedRawInput<=Program.Manager.Config.RawInputTovJoyMap.Count)) {
                if (SelectedJoyBtn>0) {
                    var btns = Program.Manager.Config.RawInputTovJoyMap[SelectedRawInput-1].vJoyBtns;
                    if (btns.Exists(x => (x==(SelectedJoyBtn-1)))) {
                        btns.Remove((SelectedJoyBtn-1));
                        btns.Sort();
                        RefresList();
                    }
                }
            }
        }

        private void chkToggling_Click(object sender, EventArgs e)
        {

        }

        private void chkAutofire_Click(object sender, EventArgs e)
        {

        }

        private void lstJoyBtn_SelectedValueChanged(object sender, EventArgs e)
        {
            if (int.TryParse(lstJoyBtn.SelectedItem.ToString(), out var joyBtn)) {
                cmbBtnMapTo.SelectedIndex = joyBtn-1;
            }
        }
    }
}
