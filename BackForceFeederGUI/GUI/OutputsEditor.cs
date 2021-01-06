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
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.Outputs;
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
            cmbLampBit.Items.Clear();
            for (int i = 1; i <= EditedControlSet.RawOutputDBs.Count; i++) {
                // Display
                var lampBit = new CheckBox();
                lampBit.AutoSize = true;
                lampBit.Enabled = true; // Allow to force value for testing
                lampBit.Location = new System.Drawing.Point(6 + 48*((i-1)>>3), 16 + 20*((i-1)&0b111));
                lampBit.Name = "chkLampBit" + i;
                lampBit.Size = new System.Drawing.Size(32, 17);
                lampBit.TabIndex = i;
                lampBit.Text = i.ToString();
                lampBit.Tag = i;
                lampBit.UseVisualStyleBackColor = true;
                lampBit.Click += chkGameOut_Click;

                AllLampChkBox.Add(lampBit);

                panel.Controls.Add(lampBit);

                // Combo box
                cmbLampBit.Items.Add(i.ToString());
            }

            int nbRawOutputs = 16;
            cmbBtnMapTo.Items.Clear();
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

            cmbIsSequenced.Items.Clear();
            foreach (var item in Enum.GetValues(typeof(OutputSequence))) {
                cmbIsSequenced.Items.Add(item.ToString());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SharedData.Manager.SaveControlSetFiles();
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (!SharedData.Manager.IsRunning)
                return;
            var outputs = SharedData.Manager.Outputs;
            if (outputs==null)
                return;
            // Lamps output bits
            for (int i = 0; i < AllLampChkBox.Count; i++) {
                var chk = AllLampChkBox[i];
                if ((outputs.GameOutputsValues & ((UInt64)1 << i)) != 0)
                    chk.Checked = true;
                else
                    chk.Checked = false;
            }
            // Raw output bits
            for (int i = 0; i < AllRawOutChkBox.Count; i++) {
                var chk = AllRawOutChkBox[i];
                if ((outputs.RawOutputsStates & ((UInt64)1 << i)) != 0)
                    chk.Checked = true;
                else
                    chk.Checked = false;
            }

        }


        /// <summary>
        /// Selected lamp output in the left list
        /// </summary>
        int SelectedGameOutputBit = -1;
        private void cmbBtnMapFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbLampBit.SelectedItem.ToString(), out SelectedGameOutputBit)) {
                SelectedGameOutputBit = -1;
            }
            RefresListOfOptions();
        }


        void RefresListOfOptions()
        {
            if ((SelectedGameOutputBit>0) && (SelectedGameOutputBit<=EditedControlSet.RawOutputDBs.Count)) {
                lstRawBits.Items.Clear();
                var raw = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1];
                chkInvertLampLogic.Checked = raw.IsInvertedLogic;
                txtSequenceDelay.Text = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1].SequenceDelay_ms.ToString();

                cmbIsSequenced.SelectedItem = raw.Sequence.ToString();

                var btns = raw.MappedRawOutputBit;
                foreach (var btn in btns) {
                    lstRawBits.Items.Add((btn+1).ToString());
                }
            }
        }

        /// <summary>
        /// Selected raw output in the right list
        /// </summary>
        int SelectedRawOutputBit = -1;
        private void cmbBtnMapTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbBtnMapTo.SelectedItem.ToString(), out SelectedRawOutputBit)) {
                SelectedRawOutputBit = -1;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if ((SelectedGameOutputBit>0) && (SelectedGameOutputBit<=EditedControlSet.RawOutputDBs.Count)) {
                if (SelectedRawOutputBit>0) {
                    var rawOutbit = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1].MappedRawOutputBit;
                    if (!rawOutbit.Exists(x => (x==(SelectedRawOutputBit-1)))) {
                        rawOutbit.Add(SelectedRawOutputBit-1);
                        rawOutbit.Sort();
                        RefresListOfOptions();
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if ((SelectedGameOutputBit>0) && (SelectedGameOutputBit<=EditedControlSet.RawOutputDBs.Count)) {
                if (SelectedRawOutputBit>0) {
                    var rawOutbit = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1].MappedRawOutputBit;
                    if (rawOutbit.Exists(x => (x==(SelectedRawOutputBit-1)))) {
                        rawOutbit.Remove((SelectedRawOutputBit-1));
                        rawOutbit.Sort();
                        RefresListOfOptions();
                    }
                }
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            if ((SelectedGameOutputBit>0) && (SelectedGameOutputBit<=EditedControlSet.RawOutputDBs.Count)) {
                var rawOutbit = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1].MappedRawOutputBit;
                rawOutbit.Clear();
                RefresListOfOptions();
            }
        }


        private void chkInvertLampLogic_Click(object sender, EventArgs e)
        {
            if ((SelectedGameOutputBit>0) && (SelectedGameOutputBit<=EditedControlSet.RawOutputDBs.Count)) {
                var raw = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1];
                raw.IsInvertedLogic = chkInvertLampLogic.Checked;
            }
        }

        private void chkGameOut_Click(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            var rawout = SharedData.Manager.Outputs;
            if (chk!=null && rawout!=null) {
                if (chk.Tag!=null) {
                    var idx = (int)chk.Tag;
                    rawout.Enforce(idx-1, chk.Checked);
                }
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
                SelectedGameOutputBit = -1;
                EditedControlSet.RawOutputDBs.Clear();
                for (int i = 0; i<OutputsManager.MAXOUTPUTS; i++) {
                    var db = new RawOutputDB();
                    db.MappedRawOutputBit = new List<int>(1) { i };
                    EditedControlSet.RawOutputDBs.Add(db);
                }
                FillPanelWithChkBox();
                RefresListOfOptions();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cmbIsSequenced_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((SelectedGameOutputBit<=0) || (SelectedGameOutputBit>EditedControlSet.RawOutputDBs.Count)) {
                MessageBox.Show("Please select a game output first", "Error", MessageBoxButtons.OK);
            } else {
                var raw = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1];
                Enum.TryParse<OutputSequence>(cmbIsSequenced.SelectedItem.ToString(), out raw.Sequence);
            }
        }

        private void UpdatetxtSequenceDelay()
        {
            // Check an item is selected
            if (EditedControlSet.RawOutputDBs==null)
                return;
            if ((SelectedGameOutputBit<=0) || (SelectedGameOutputBit>EditedControlSet.RawOutputDBs.Count)) {
                return;
            }
            // Retrieve item and check only one exist
            if (int.TryParse(txtSequenceDelay.Text, out var value)) {
                EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1].SequenceDelay_ms = value;
            }
            txtSequenceDelay.Text = EditedControlSet.RawOutputDBs[SelectedGameOutputBit-1].SequenceDelay_ms.ToString();

        }

        private void txtSequenceDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            UpdatetxtSequenceDelay();
        }

        private void txtSequenceDelay_Leave(object sender, EventArgs e)
        {
            UpdatetxtSequenceDelay();
        }
    }
}
