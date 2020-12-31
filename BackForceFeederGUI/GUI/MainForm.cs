using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using BackForceFeeder.vJoyIOFeederAPI;

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

        List<CheckBox> AllRawInputs = new List<CheckBox>();
        List<CheckBox> AllvJoyBtns = new List<CheckBox>();
        List<CheckBox> AllOutputs = new List<CheckBox>();
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = "BackForceFeeder v" +typeof(BFFManager).Assembly.GetName().Version.ToString() + " Made for Gamoover by njz3";

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

            // Only display first 16 buttons/io
            for (int i = 1; i <= 16; i++) {
                // Checkboxes for Raw inputs
                var chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(320 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
                chkBox.Name = "RawBtn" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllRawInputs.Add(chkBox);
                this.splitContainerMain.Panel2.Controls.Add(chkBox);

                // Checkboxes for vJoy Buttons
                chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(440 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
                chkBox.Name = "vJoyBtn" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllvJoyBtns.Add(chkBox);
                this.splitContainerMain.Panel2.Controls.Add(chkBox);

                // Checkboxes for Raw outputs

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


        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (!BFFManager.Config.Application.StartMinimized &&
                WindowState == FormWindowState.Minimized &&
                Program.TrayIcon!=null) {
                Program.TrayIcon.ShowBalloonTip(3000,
                        "BackForceFeeder by njz3",
                        "Running mode is " + BFFManager.Config.Hardware.TranslatingModes.ToString(),
                        ToolTipIcon.Info);
            }
        }

        private static int WM_QUERYENDSESSION = 0x11;
        /// <summary>
        /// Catch specific OS events like ENDSESSION which closes any application
        /// </summary>
        /// <param name="msg"></param>
        protected override void WndProc(ref Message msg)
        {
            var message = msg.Msg;
            // For debugging only
            //Console.WriteLine(message);
            // WM_ENDSESSION: invoke Close in the pump
            if (message == WM_QUERYENDSESSION) {
                BeginInvoke(new EventHandler(delegate { Close(); }));
            }
            // everything else is default
            base.WndProc(ref msg);
        }

        protected vJoyFeeder vJoy { get { return SharedData.Manager.vJoy; } }
        protected ControlSetDB CurrentControlSet { get { return BFFManager.CurrentControlSet; } }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            // Scan vJoy used axis to refresh list of axes
            int comboidx = 0;
            var vjoy = vJoy;
            var cs = CurrentControlSet;
            if (vjoy!=null) {
                for (int i = 0; i<vjoy.NbAxes; i++) {
                    var axisinfo = vjoy.SafeGetvJoyAxisInfo(i);
                    if (axisinfo==null)
                        return;
                    var name = axisinfo.Name.ToString().Replace("HID_USAGE_", "");
                    if (comboidx>=cmbSelectedAxis.Items.Count) {
                        cmbSelectedAxis.Items.Add(name);
                    } else {
                        cmbSelectedAxis.Items[comboidx] = name;
                    }
                    comboidx++;
                }
            }
            int selectedAxis = cmbSelectedAxis.SelectedIndex;

            if (!cmbConfigSet.DroppedDown) {
                cmbConfigSet.SelectedItem = cs.UniqueName;
                this.lblCurrentGame.Text = cs.GameName;
            }

            // Raw Axes
            if (SharedData.Manager.Inputs != null) {
                var rawaxis = SharedData.Manager.Inputs.SafeGetRawAxis(selectedAxis);
                if (rawaxis!=null) {
                    txtRawAxisValue.Text = (rawaxis.RawValue_pct*100).ToString("F2");
                    slRawAxis.Maximum = 100;
                    slRawAxis.Value = (int)(rawaxis.RawValue_pct*100);
                }
            } else {
                txtRawAxisValue.Text = "NA";
                slRawAxis.Value = 0;
            }

            // vJoy Axes
            if (vjoy != null) {
                var vjoyaxis = vjoy.SafeGetvJoyAxisInfo(selectedAxis);
                if (vjoyaxis!=null) {
                    txtJoyAxisValue.Text = (vjoyaxis.AxisValue_pct*100).ToString("F2");
                    slJoyAxis.Maximum = 100;
                    slJoyAxis.Value = (int)(vjoyaxis.AxisValue_pct*100);
                    axesJoyGauge.Value = (((double)slJoyAxis.Value / (double)slJoyAxis.Maximum) - 0.5) * 270;
                } else {
                    if (vjoy.NbAxes>0)
                        cmbSelectedAxis.SelectedIndex = 0;
                }
            } else {
                txtJoyAxisValue.Text = "NA";
                slJoyAxis.Value = 0;
                axesJoyGauge.Value = 0;
            }

            // Buttons
            if ((vjoy != null)) {
                UInt64 buttons0_63 = 0;
                UInt64 buttons64_127 = 0;
                vjoy.GetButtonsStates(ref buttons0_63, ref buttons64_127);
                for (int i = 0; i < AllvJoyBtns.Count; i++) {
                    var chk = AllvJoyBtns[i];
                    if ((buttons0_63 & (UInt64)(1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }
            // Raw inputs
            if (SharedData.Manager.IOboard != null) {
                var inputs = SharedData.Manager.RawInputsFromIOBoard;
                for (int i = 0; i < AllRawInputs.Count; i++) {
                    var chk = AllRawInputs[i];
                    if ((inputs & (UInt64)(1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }
            // Outputs
            if (SharedData.Manager.Outputs != null) {
                var outputs = SharedData.Manager.RawOutputsToIOBoard;
                for (int i = 0; i < 16; i++) {
                    var chk = AllOutputs[i];
                    if ((outputs & ((UInt64)1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }

            // IOBoard status
            if (SharedData.Manager.IOboard ==null) {
                // No IO BOard
                this.labelStatus.ForeColor = Color.Red;
                this.labelStatus.Text = "IOBoard scanning, not found yet (check cables or baudrate)";
            } else {
                // Outputs mode only?
                if (BFFManager.Config.Application.OutputOnly) {
                    // Check manager state only
                    this.labelStatus.ForeColor = Color.Black;
                    if (SharedData.Manager.IsRunning)
                        this.labelStatus.Text = "Running (outputs only)";
                    else
                        this.labelStatus.Text = "Stopped (outputs only)";
                } else {
                    // vJoy ?
                    if (vJoy!=null && !vJoy.vJoyVersionMatch) {
                        // Wrong vJoy
                        this.labelStatus.ForeColor = Color.Red;
                        this.labelStatus.Text = "vJoy error, driver version=" + String.Format("{0:X}", vJoy.vJoyVersionDriver)
                            + ", dll version=" + String.Format("{0:X}", vJoy.vJoyVersionDll);
                    } else {
                        // All good
                        /*
                        this.labelStatus.ForeColor = Color.Red;
                        this.labelStatus.Text = "vJoy error, wrong Driver version=" + String.Format("{0:X}", Program.Manager.vJoy.vJoyVersionDriver)
                            + " expecting dll version=" + String.Format("{0:X}", Program.Manager.vJoy.vJoyVersionDll);
                        */
                        this.labelStatus.ForeColor = Color.Black;
                        if (SharedData.Manager.IsRunning)
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

            // Make sure vJoy is enabled
            if (vJoy == null) return;
            var cs = CurrentControlSet;
            // Ensure we have a valid axis
            var axis = cs.RawAxisDBs[selectedvJoyIndexAxis];
            if (axis==null) return;

            AxisMappingEditor editor = new AxisMappingEditor(cs);
            editor.SelectedAxis = selectedvJoyIndexAxis;
            // Clone configuration before modifying it
            editor.InputRawDB = (RawAxisDB)cs.RawAxisDBs[selectedvJoyIndexAxis].Clone();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
                // Save new object
                cs.RawAxisDBs[selectedvJoyIndexAxis] = editor.ResultRawDB;
                SharedData.Manager.SaveControlSetFiles();
            }
            editor.Dispose();
        }

        private void btnButtons_Click(object sender, EventArgs e)
        {
            ButtonsEditor editor = new ButtonsEditor(CurrentControlSet);
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
            editor.Dispose();
        }


        private void btnOutputs_Click(object sender, EventArgs e)
        {
            OutputsEditor editor = new OutputsEditor(CurrentControlSet);
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
            editor.Dispose();
        }

        private void btnKeyStrokes_Click(object sender, EventArgs e)
        {
            KeyEmulationEditor editor = new KeyEmulationEditor(CurrentControlSet);
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
            editor.Dispose();
        }

        private void btnTuneEffects_Click(object sender, EventArgs e)
        {
            EffectTuningEditor editor = new EffectTuningEditor(CurrentControlSet);
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
            }
            editor.Dispose();
        }

        private void btnControlSets_Click(object sender, EventArgs e)
        {
            ControlSetEditor editor = new ControlSetEditor();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
                SharedData.Manager.SortControlSets();
                FillControlSet();
            }
        }

        private void FillControlSet()
        {
            cmbConfigSet.Items.Clear();
            for (int i = 0; i<BFFManager.Config.AllControlSets.ControlSets.Count; i++) {
                var cs = BFFManager.Config.AllControlSets.ControlSets[i];
                cmbConfigSet.Items.Add(cs.UniqueName);
            }
            cmbConfigSet.SelectedItem = CurrentControlSet.UniqueName;
            this.lblCurrentGame.Text = CurrentControlSet.GameName;
        }

        private void cmbConfigSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cs = BFFManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName == (string)cmbConfigSet.SelectedItem));
            if (cs!=null) {
                SharedData.Manager.ChangeCurrentControlSet(cs);
                this.lblCurrentGame.Text = CurrentControlSet.GameName;
            }
        }

    }
}
