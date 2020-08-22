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

namespace BackForceFeederGUI.GUI
{

    public partial class MainForm : Form
    {
        /// <summary>
        /// Always here to save logs
        /// </summary>
        public LogForm Log;

        public MainForm()
        {
            InitializeComponent();

            Log = new LogForm();
        }

        List<CheckBox> AllvJoyBtn = new List<CheckBox>();
        List<CheckBox> AllRawBtn = new List<CheckBox>();
        List<CheckBox> AllOutputs = new List<CheckBox>();
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "BackForceFeeder v" +typeof(vJoyManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";

            // Must do this to create controls and allow for Log() to have 
            // right thread ID when calling InvokeReduired
            Log.Show();
            Log.Hide();

            ToolTip tooltip = new ToolTip();

            axesJoyGauge.FromValue = -135;
            axesJoyGauge.ToValue = 135;
            axesJoyGauge.Wedge = 270;
            axesJoyGauge.LabelsStep = 270.0 / 4.0;
            axesJoyGauge.TickStep = 135 / 10.0;
            //axesGauge.DisableAnimations = true;
            axesJoyGauge.AnimationsSpeed = new TimeSpan(0, 0, 0, 0, 100);
            //axesJoyGauge.RightToLeft = RightToLeft.Yes;

            cmbSelectedAxis.Items.Clear();
            cmbSelectedAxis.SelectedIndex = -1;

            // Only display first 24 buttons/io
            for (int i = 1; i <= 16; i++) {
                var chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(320 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
                chkBox.Name = "vJoyBtn" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllvJoyBtn.Add(chkBox);
                this.splitContainerMain.Panel2.Controls.Add(chkBox);


                chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(440 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
                chkBox.Name = "RawBtn" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllRawBtn.Add(chkBox);
                this.splitContainerMain.Panel2.Controls.Add(chkBox);

                chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(560 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
                chkBox.Name = "Output" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllOutputs.Add(chkBox);
                this.splitContainerMain.Panel2.Controls.Add(chkBox);
            }

            FillControlSet();
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            // Scan vJoy used axis to refresh list of axes
            int comboidx = 0;
            for (int i = 0; i<Program.Manager.vJoy.NbUsedAxis; i++) {
                var axisinfo = Program.Manager.vJoy.SafeGetUsedAxis(i);
                if (axisinfo==null)
                    return;
                var name = axisinfo.vJoyAxisInfo.Name.ToString().Replace("HID_USAGE_", "");
                if (comboidx>=cmbSelectedAxis.Items.Count) {
                    cmbSelectedAxis.Items.Add(name);
                } else {
                    cmbSelectedAxis.Items[comboidx] = name;
                }
                comboidx++;
            }

            int selectedvJoyAxis = cmbSelectedAxis.SelectedIndex;

            if (!cmbConfigSet.DroppedDown) {
                cmbConfigSet.SelectedItem = vJoyManager.Config.CurrentControlSet.UniqueName;
                this.lblCurrentGame.Text = vJoyManager.Config.CurrentControlSet.GameName;
            }

            if (Program.Manager.vJoy != null) {
                var axis = Program.Manager.vJoy.SafeGetUsedAxis(selectedvJoyAxis);
                if (axis!=null) {

                    txtRawAxisValue.Text = axis.vJoyAxisInfo.RawValue.ToString();
                    slRawAxis.Maximum = 4095;
                    slRawAxis.Value = (int)axis.vJoyAxisInfo.RawValue;

                    txtJoyAxisValue.Text = axis.vJoyAxisInfo.CorrectedValue.ToString();
                    slJoyAxis.Maximum = (int)axis.vJoyAxisInfo.MaxValue;
                    slJoyAxis.Value = (int)axis.vJoyAxisInfo.CorrectedValue;

                    axesJoyGauge.Value = (((double)slJoyAxis.Value / (double)slJoyAxis.Maximum) - 0.5) * 270;
                } else {
                    if (Program.Manager.vJoy.NbUsedAxis>0)
                        cmbSelectedAxis.SelectedIndex = 0;
                }
            } else {

                txtRawAxisValue.Text = "NA";
                txtJoyAxisValue.Text = "NA";
                slRawAxis.Value = 0;
                slJoyAxis.Value = 0;
                axesJoyGauge.Value = 0;
            }


            if ((Program.Manager.vJoy != null)) {
                var buttons = Program.Manager.vJoy.GetButtonsState();
                for (int i = 0; i < AllvJoyBtn.Count; i++) {
                    var chk = AllvJoyBtn[i];
                    if ((buttons & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }

            if (Program.Manager.IOboard != null) {
                var inputs = Program.Manager.RawInputsStates;
                for (int i = 0; i < AllvJoyBtn.Count; i++) {
                    var chk = AllRawBtn[i];
                    if ((inputs & (UInt64)(1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }

            if (Program.Manager.Outputs != null) {
                var outputs = Program.Manager.RawOutputsStates;
                for (int i = 0; i < 16; i++) {
                    var chk = AllOutputs[i];
                    if ((outputs & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }

            // IOBoards
            if (Program.Manager.IOboard ==null) {
                // No IO BOard
                this.labelStatus.ForeColor = Color.Red;
                this.labelStatus.Text = "IOBoard scanning, not found yet (check cables or baudrate)";
            } else {
                // Outputs mode only?
                if (vJoyManager.Config.Application.OutputOnly) {
                    // Check manager state only
                    this.labelStatus.ForeColor = Color.Black;
                    if (Program.Manager.IsRunning)
                        this.labelStatus.Text = "Running (outputs only)";
                    else
                        this.labelStatus.Text = "Stopped (outputs only)";
                } else {
                    // vJoy ?
                    if (Program.Manager.vJoy!=null &&
                           !Program.Manager.vJoy.vJoyVersionMatch) {
                        // Wrong vJoy
                        this.labelStatus.ForeColor = Color.Red;
                        this.labelStatus.Text = "vJoy error, driver version=" + String.Format("{0:X}", Program.Manager.vJoy.vJoyVersionDriver)
                            + ", dll version=" + String.Format("{0:X}", Program.Manager.vJoy.vJoyVersionDll);
                    } else {
                        // All good
                        /*
                        this.labelStatus.ForeColor = Color.Red;
                        this.labelStatus.Text = "vJoy error, wrong Driver version=" + String.Format("{0:X}", Program.Manager.vJoy.vJoyVersionDriver)
                            + " expecting dll version=" + String.Format("{0:X}", Program.Manager.vJoy.vJoyVersionDll);
                        */
                        this.labelStatus.ForeColor = Color.Black;
                        if (Program.Manager.IsRunning)
                            this.labelStatus.Text = "Running";
                        else
                            this.labelStatus.Text = "Stopped";
                    }
                }
            }
        }


        private void btnConfigureHardware_Click(object sender, EventArgs e)
        {
            AppHwdEditor editor = new AppHwdEditor();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
        }

        private void btnShowLogWindow_Click(object sender, EventArgs e)
        {
            Log.Show();
        }


        private void btnAxisMappingEditor_Click(object sender, EventArgs e)
        {
            int selectedvJoyIndexAxis = cmbSelectedAxis.SelectedIndex;

            if (Program.Manager.vJoy == null) return;

            var axis = Program.Manager.vJoy.SafeGetUsedAxis(selectedvJoyIndexAxis);
            if (axis==null) return;

            AxisMappingEditor editor = new AxisMappingEditor();
            editor.SelectedAxis = selectedvJoyIndexAxis;
            editor.InputRawDB = (RawAxisDB)axis.RawAxisDB.Clone();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
                axis.RawAxisDB = editor.ResultRawDB;
                Program.Manager.SaveControlSetFiles();
            }
        }

        private void btnButtons_Click(object sender, EventArgs e)
        {
            ButtonsEditor editor = new ButtonsEditor();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
        }

        private void btnAxes_Click(object sender, EventArgs e)
        {
            AxisForm editor = new AxisForm();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
        }



        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (!vJoyManager.Config.Application.StartMinimized &&
                WindowState == FormWindowState.Minimized &&
                Program.TrayIcon!=null) {
                Program.TrayIcon.ShowBalloonTip(3000,
                        "BackForceFeeder by njz3",
                        "Running mode is " + vJoyManager.Config.Hardware.TranslatingModes.ToString(),
                        ToolTipIcon.Info);
            }
        }

        private void btnOutputs_Click(object sender, EventArgs e)
        {
            OutputsEditor editor = new OutputsEditor();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {

            }
        }

        private void btnTuneEffects_Click(object sender, EventArgs e)
        {
            EffectTuningEditor editor = new EffectTuningEditor();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {

            }
        }

        private void btnControlSets_Click(object sender, EventArgs e)
        {
            ControlSetEditor editor = new ControlSetEditor();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
                Program.Manager.SortControlSets();
                FillControlSet();
            }
        }

        private void FillControlSet()
        {
            cmbConfigSet.Items.Clear();
            for (int i = 0; i<vJoyManager.Config.AllControlSets.ControlSets.Count; i++) {
                var cs = vJoyManager.Config.AllControlSets.ControlSets[i];
                cmbConfigSet.Items.Add(cs.UniqueName);
            }
            cmbConfigSet.SelectedItem = vJoyManager.Config.CurrentControlSet.UniqueName;
            this.lblCurrentGame.Text = vJoyManager.Config.CurrentControlSet.GameName;
        }



        private void cmbConfigSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName == (string)cmbConfigSet.SelectedItem));
            if (cs!=null) {
                vJoyManager.Config.CurrentControlSet = cs;
                this.lblCurrentGame.Text = vJoyManager.Config.CurrentControlSet.GameName;
            }
        }

    }
}
