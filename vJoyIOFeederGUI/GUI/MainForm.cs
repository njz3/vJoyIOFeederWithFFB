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

            cmbSelectedAxis.SelectedIndex = 0;

            // Only display first 16 buttons/io
            for (int i = 1; i <= 16; i++) {
                var chkBox = new CheckBox();
                chkBox.AutoSize = true;
                chkBox.Enabled = false;
                chkBox.Location = new System.Drawing.Point(341 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
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
                chkBox.Location = new System.Drawing.Point(451 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
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
                chkBox.Location = new System.Drawing.Point(557 + 48*((i-1)>>3), 23 + 20*((i-1)&0b111));
                chkBox.Name = "Output" + i;
                chkBox.Size = new System.Drawing.Size(32, 17);
                chkBox.TabIndex = i;
                chkBox.Text = i.ToString();
                chkBox.Tag = i;
                chkBox.UseVisualStyleBackColor = true;
                AllOutputs.Add(chkBox);
                this.splitContainerMain.Panel2.Controls.Add(chkBox);


            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Manager.SaveConfigurationFiles(Program.ConfigPath);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized) {
                this.Hide();

                notifyIcon.Visible = true;
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuShow_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            menuShow_Click(sender, e);
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            int selectedAxis = cmbSelectedAxis.SelectedIndex;

            if ((Program.Manager.vJoy != null) && (selectedAxis>=0) &&
                (Program.Manager.vJoy.AxesInfo[selectedAxis].IsPresent) &&
                (Program.Manager.vJoy.AxesInfo[selectedAxis].MaxValue > 0)) {

                txtRawAxisValue.Text = Program.Manager.vJoy.AxesInfo[selectedAxis].RawValue.ToString();
                slRawAxis.Maximum = 4095;
                slRawAxis.Value = (int)Program.Manager.vJoy.AxesInfo[selectedAxis].RawValue;

                txtJoyAxisValue.Text = Program.Manager.vJoy.AxesInfo[selectedAxis].CorrectedValue.ToString();
                slJoyAxis.Maximum = (int)Program.Manager.vJoy.AxesInfo[selectedAxis].MaxValue;
                slJoyAxis.Value = (int)Program.Manager.vJoy.AxesInfo[selectedAxis].CorrectedValue;

                axesJoyGauge.Value = (((double)slJoyAxis.Value / (double)slJoyAxis.Maximum) - 0.5) * 270;

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
                    if ((inputs & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }

            if (Program.Manager.Outputs != null) {
                var outputs = Program.Manager.RawOutputsStates;
                for (int i = 0; i < AllvJoyBtn.Count; i++) {
                    var chk = AllOutputs[i];
                    if ((outputs & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                menuShow_Click(sender, e);
            }
        }


        private void btnConfigureHardware_Click(object sender, EventArgs e)
        {
            TargetHdwForm editor = new TargetHdwForm();
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
            int selectedAxis = cmbSelectedAxis.SelectedIndex;

            if ((Program.Manager.vJoy != null) &&
                (Program.Manager.vJoy.AxesInfo[selectedAxis].IsPresent) &&
                (Program.Manager.vJoy.AxesInfo[selectedAxis].MaxValue > 0)) {
                AxisMappingEditor editor = new AxisMappingEditor();
                editor.Input = Program.Manager.vJoy.AxesInfo[selectedAxis];
                var res = editor.ShowDialog(this);
                if (res == DialogResult.OK) {
                    Program.Manager.vJoy.AxesInfo[selectedAxis] = editor.Result;
                    Program.Manager.SaveConfigurationFiles(Program.ConfigPath);
                }
            }
        }

        private void btnButtons_Click(object sender, EventArgs e)
        {
            ButtonsForm editor = new ButtonsForm();
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
    }
}
