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
using BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.Utils;
using BackForceFeeder.vJoyIOFeederAPI;

namespace BackForceFeederGUI.GUI
{

    public partial class OutputsEditor : Form
    {
        protected ControlSetDB EditedControlSet;

        public OutputsEditor(ControlSetDB controlSet)
        {
            EditedControlSet = controlSet;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            FillPanelWithChkBox();
        }

        List<CheckBox> AllLampChkBox = new List<CheckBox>();
        List<CheckBox> AllRawOutChkBox = new List<CheckBox>();

        private void FillPanelWithChkBox()
        {
            var panel = this.splitContainerMain.Panel1;
            // Clean checkboxes
            for (int i = 0; i<AllLampChkBox.Count; i++) {
                var chk = AllLampChkBox[i];
                panel.Controls.Remove(chk);
                chk.Dispose();
            }
            AllLampChkBox.Clear();
            for (int i = 0; i<AllRawOutChkBox.Count; i++) {
                var chk = AllRawOutChkBox[i];
                panel.Controls.Remove(chk);
                chk.Dispose();
            }
            AllRawOutChkBox.Clear();

            // Create new ones
            for (int i = 1; i <= EditedControlSet.RawOutputBitMap.Count; i++) {
                // Display
                var lampBit = new CheckBox();
                lampBit.AutoSize = true;
                lampBit.Enabled = false;
                lampBit.Location = new System.Drawing.Point(6 + 48*((i-1)>>3), 16 + 20*((i-1)&0b111));
                lampBit.Name = "chkLampBit" + i;
                lampBit.Size = new System.Drawing.Size(32, 17);
                lampBit.TabIndex = i;
                lampBit.Text = i.ToString();
                lampBit.Tag = i;
                lampBit.UseVisualStyleBackColor = true;
                AllLampChkBox.Add(lampBit);

                panel.Controls.Add(lampBit);

                // Combo box
                cmbLampBit.Items.Add(i.ToString());
            }

            int nbRawOutputs = 16;
            for (int i = 1; i <= nbRawOutputs; i++) {
                // Display
                var rawOutBit = new CheckBox();
                rawOutBit.AutoSize = true;
                rawOutBit.Enabled = false;
                rawOutBit.Location = new System.Drawing.Point(6 + 48* ((i-1)>>3) + 200, 16 + 20* ((i-1)&0b111));
                rawOutBit.Name = "chkRawOutBit" + i;
                rawOutBit.Size = new System.Drawing.Size(32, 17);
                rawOutBit.TabIndex = i;
                rawOutBit.Text = i.ToString();
                rawOutBit.Tag = i;
                rawOutBit.UseVisualStyleBackColor = true;
                AllRawOutChkBox.Add(rawOutBit);

                panel.Controls.Add(rawOutBit);

                // Combo box
                cmbBtnMapTo.Items.Add(i.ToString());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Manager.SaveControlSetFiles();
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (Program.Manager.IsRunning) {
                // Lamps output bits
                for (int i = 0; i < AllLampChkBox.Count; i++) {
                    var chk = AllLampChkBox[i];
                    if ((Program.Manager.RawLampOutput & (UInt64)(1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
                // Raw output bits
                for (int i = 0; i < AllRawOutChkBox.Count; i++) {
                    var chk = AllRawOutChkBox[i];
                    if ((Program.Manager.RawOutputsStates & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }
        }

        int SelectedLampOutputBit = -1;
        private void cmbBtnMapFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbLampBit.SelectedItem.ToString(), out SelectedLampOutputBit)) {
                SelectedLampOutputBit = -1;
            }
            RefresList();
        }


        void RefresList()
        {
            if ((SelectedLampOutputBit>0) && (SelectedLampOutputBit<=EditedControlSet.RawOutputBitMap.Count)) {
                var raw = EditedControlSet.RawOutputBitMap[SelectedLampOutputBit-1];
                chkInvertLampLogic.Checked = raw.IsInvertedLogic;

                var btns = raw.MappedRawOutputBit;
                lstRawBits.Items.Clear();
                foreach (var btn in btns) {
                    lstRawBits.Items.Add((btn+1).ToString());
                }
            }
        }

        int SelectedRawOutputBit = -1;
        private void cmbBtnMapTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbBtnMapTo.SelectedItem.ToString(), out SelectedRawOutputBit)) {
                SelectedRawOutputBit = -1;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if ((SelectedLampOutputBit>0) && (SelectedLampOutputBit<=EditedControlSet.vJoyMapping.RawInputTovJoyMap.Count)) {
                if (SelectedRawOutputBit>0) {
                    var rawOutbit = EditedControlSet.RawOutputBitMap[SelectedLampOutputBit-1].MappedRawOutputBit;
                    if (!rawOutbit.Exists(x => (x==(SelectedRawOutputBit-1)))) {
                        rawOutbit.Add(SelectedRawOutputBit-1);
                        rawOutbit.Sort();
                        RefresList();
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if ((SelectedLampOutputBit>0) && (SelectedLampOutputBit<=EditedControlSet.vJoyMapping.RawInputTovJoyMap.Count)) {
                if (SelectedRawOutputBit>0) {
                    var rawOutbit = EditedControlSet.RawOutputBitMap[SelectedLampOutputBit-1].MappedRawOutputBit;
                    if (rawOutbit.Exists(x => (x==(SelectedRawOutputBit-1)))) {
                        rawOutbit.Remove((SelectedRawOutputBit-1));
                        rawOutbit.Sort();
                        RefresList();
                    }
                }
            }
        }

        private void chkInvertLampLogic_Click(object sender, EventArgs e)
        {
            if ((SelectedLampOutputBit>0) && (SelectedLampOutputBit<=EditedControlSet.RawOutputBitMap.Count)) {
                var raw = EditedControlSet.RawOutputBitMap[SelectedLampOutputBit-1];
                raw.IsInvertedLogic = chkInvertLampLogic.Checked;
            }
        }



        private void lstRawBit_SelectedValueChanged(object sender, EventArgs e)
        {
            if (lstRawBits.SelectedItem!=null && int.TryParse(lstRawBits.SelectedItem.ToString(), out var joyBtn)) {
                cmbBtnMapTo.SelectedIndex = (joyBtn-1)%cmbBtnMapTo.Items.Count;
            }
        }

        private void btnResetAll_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Reset configuration\nAre you sure ?", "Reset configuration", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK) {
                EditedControlSet.RawOutputBitMap.Clear();
                for (int i = 0; i<16; i++) {
                    var db = new RawOutputDB();
                    db.MappedRawOutputBit = new List<int>(1) { i };
                    EditedControlSet.RawOutputBitMap.Add(db);
                }
                FillPanelWithChkBox();
                RefresList();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
