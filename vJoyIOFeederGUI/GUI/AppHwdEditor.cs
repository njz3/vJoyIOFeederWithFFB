using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeederGUI.GUI
{

    public partial class AppHwdEditor : Form
    {
        public AppHwdEditor()
        {
            InitializeComponent();
        }

        private ToolTip tooltip = new ToolTip();

        private void TargetHdwForm_Load(object sender, EventArgs e)
        {
            tooltip.SetToolTip(this.cmbSelectMode, "Translation mode can only be changed while manager is Stopped");
            tooltip.SetToolTip(this.btnStartStopManager, "Press here to stop manager");
            tooltip.SetToolTip(this.chkDualModePWM, "PWM mode can only be changed when manager is stopped");
            this.cmbSelectMode.Items.Clear();
            foreach (string mode in Enum.GetNames(typeof(FFBTranslatingModes))) {
                this.cmbSelectMode.Items.Add(mode);

                if (vJoyManager.Config.Hardware.TranslatingModes.ToString().Equals(mode, StringComparison.OrdinalIgnoreCase)) {
                    this.cmbSelectMode.SelectedIndex = this.cmbSelectMode.Items.Count - 1;
                }
            }

            this.txtWheelScale.Text = vJoyManager.Config.Hardware.WheelScaleFactor_u_per_cts.ToString("G8", CultureInfo.InvariantCulture);
            this.txtWheelCenter.Text = vJoyManager.Config.Hardware.WheelCenterOffset_u.ToString("G8", CultureInfo.InvariantCulture);

            this.cmbBaudrate.Items.Clear();
            UInt32[] speedlist = { 1000000, 500000, 115200, 57600 };
            foreach (var speed in speedlist) {
                this.cmbBaudrate.Items.Add(speed.ToString());

                if (vJoyManager.Config.Hardware.SerialPortSpeed.ToString().Equals(speed.ToString(), StringComparison.OrdinalIgnoreCase)) {
                    this.cmbBaudrate.SelectedIndex = this.cmbBaudrate.Items.Count - 1;
                }
            }
        }

        private void TargetHdwForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Manager.SaveConfigurationFiles(Program.AppCfgFilename, Program.HwdCfgFilename);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            this.chkBoxStartMinimized.Checked = vJoyManager.Config.Application.StartMinimized;
            this.chkBoxStartWithWindows.Checked = vJoyManager.Config.Application.ShortcutStartWithWindowsCreated;
            this.chkDumpLogToFile.Checked = vJoyManager.Config.Application.DumpLogToFile;
            this.chkAutodetectControlSet.Checked = vJoyManager.Config.Application.AutodetectControlSetAtRuntime;

            if (Program.Manager.IsRunning) {
                this.btnStartStopManager.BackColor = Color.Green;
                this.btnStartStopManager.Text = "Running (Stop)";

                this.cmbSelectMode.Enabled = false;
                this.cmbBaudrate.Enabled = false;
                this.chkDualModePWM.Enabled = false;
                this.chkDigitalPWM.Enabled = false;
            } else {
                this.btnStartStopManager.BackColor = Color.Red;
                this.btnStartStopManager.Text = "Stopped (Start)";

                this.cmbSelectMode.Enabled = true;
                this.cmbBaudrate.Enabled = true;
                this.chkDualModePWM.Enabled = true;
                this.chkDigitalPWM.Enabled = true;
            }

            if (Program.Manager.FFB!=null) {

                if (Program.Manager.FFB.IsDeviceReady) {
                    btnDeviceReady.BackColor = Color.Green;
                    btnDeviceReady.Text = "Ready";
                    btnCommit.Enabled = true;
                } else {
                    btnDeviceReady.BackColor = Color.Red;
                    btnDeviceReady.Text = "Not ready";
                    btnCommit.Enabled = false;
                }

                chkInvertWheel.Checked = vJoyManager.Config.Hardware.InvertWheelDirection;
                chkInvertTorque.Checked = vJoyManager.Config.Hardware.InvertTrqDirection;
                chkDualModePWM.Checked =  vJoyManager.Config.Hardware.DualModePWM;
                chkDigitalPWM.Checked = vJoyManager.Config.Hardware.DigitalPWM;
            }
        }

        private void btnOpenJoyCPL_Click(object sender, EventArgs e)
        {
            ProcessAnalyzer.StartProcess(@"joy.cpl");
        }

        private void btnOpenvJoyMonitor_Click(object sender, EventArgs e)
        {
            ProcessAnalyzer.StartProcess(@"C:\Program Files\vJoy\x64\JoyMonitor.exe");
        }

        private void btnOpenvJoyConfig_Click(object sender, EventArgs e)
        {
            ProcessAnalyzer.StartProcess(@"C:\Program Files\vJoy\x64\vJoyConf.exe");
        }


        private void btnStartStopManager_Click(object sender, EventArgs e)
        {
            if (!Program.Manager.IsRunning) {
                if (Enum.TryParse<FFBTranslatingModes>(this.cmbSelectMode.SelectedItem.ToString(), out var mode)) {
                    vJoyManager.Config.Hardware.TranslatingModes = mode;
                }
                Program.Manager.Start();
            } else {
                Program.Manager.Stop();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Reset configuration\nAre you sure ?", "Reset configuration", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK) {
                if (Program.Manager.IsRunning) {
                    Program.Manager.Stop();
                }
                vJoyManager.Config.Hardware = new vJoyIOFeeder.Configuration.HardwareDB();
            }
        }
        private void cmbBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var speed = this.cmbBaudrate.SelectedItem as string;
            if (speed!=null) {
                UInt32.TryParse(speed, out vJoyManager.Config.Hardware.SerialPortSpeed);
            }
        }

        #region Application configuration
        private void chkBoxStartMinimized_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Application.StartMinimized = !vJoyManager.Config.Application.StartMinimized;
        }

        private void chkBoxStartWithWindows_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Application.ShortcutStartWithWindowsCreated = !vJoyManager.Config.Application.ShortcutStartWithWindowsCreated;
            if (vJoyManager.Config.Application.ShortcutStartWithWindowsCreated) {
                // Create shortcut
                OSUtilities.CreateStartupShortcut("vJoyIOFeederGUI", "vJoyIOFeederGUI auto-startup");
            } else {
                OSUtilities.DeleteStartupShortcut("vJoyIOFeederGUI");
            }
        }

        private void chkDumpLogToFile_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Application.DumpLogToFile = !vJoyManager.Config.Application.DumpLogToFile;
        }

        private void chkAutodetectControlSet_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Application.AutodetectControlSetAtRuntime = !vJoyManager.Config.Application.AutodetectControlSetAtRuntime;
        }

        #endregion

        #region Hardware properties

        private void chkInvertWheel_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Hardware.InvertWheelDirection = !vJoyManager.Config.Hardware.InvertWheelDirection;
        }
        private void chkInvertTorque_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Hardware.InvertTrqDirection = !vJoyManager.Config.Hardware.InvertTrqDirection;
        }

        private void chkDualModePWM_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Hardware.DualModePWM = !vJoyManager.Config.Hardware.DualModePWM;
        }
        private void chkDigitalPWM_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.Hardware.DigitalPWM = !vJoyManager.Config.Hardware.DigitalPWM;
        }
        private void btnCommit_Click(object sender, EventArgs e)
        {
            if (Program.Manager.IOboard!=null) {
                Program.Manager.IOboard.SendCommand("savecfg");
                Thread.Sleep(200);
                Program.Manager.IOboard.SendCommand("~");
            }
        }

        private void cmbSelectMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Program.Manager.IsRunning) {
                if (Enum.TryParse<FFBTranslatingModes>(this.cmbSelectMode.SelectedItem.ToString(), out var mode)) {
                    vJoyManager.Config.Hardware.TranslatingModes = mode;
                    Program.Manager.SaveConfigurationFiles(Program.AppCfgFilename, Program.HwdCfgFilename);
                }
            }
        }

        private void btnWheelCalibrate_Click(object sender, EventArgs e)
        {
            CalibrateWheelForm calibwheel = new CalibrateWheelForm();
            calibwheel.SelectedAxis = 0;
            var res = calibwheel.ShowDialog(this);
            if (res == DialogResult.OK) {
                double range_cts = calibwheel.RawMostLeft - calibwheel.RawMostRight;
                double scale_u_per_cts = 2.0/range_cts;
                vJoyManager.Config.Hardware.WheelScaleFactor_u_per_cts = scale_u_per_cts;
                txtWheelScale.Text = vJoyManager.Config.Hardware.WheelScaleFactor_u_per_cts.ToString("G8", CultureInfo.InvariantCulture);

                double center_u = calibwheel.RawMostCenter*scale_u_per_cts;
                vJoyManager.Config.Hardware.WheelCenterOffset_u = center_u;
                txtWheelCenter.Text = vJoyManager.Config.Hardware.WheelCenterOffset_u.ToString("G8", CultureInfo.InvariantCulture);
            }
        }

        private void txtWheelScale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;

            if (double.TryParse(txtWheelScale.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double scale_u_per_cts)) {
                vJoyManager.Config.Hardware.WheelScaleFactor_u_per_cts = scale_u_per_cts;
                txtWheelScale.Text = vJoyManager.Config.Hardware.WheelScaleFactor_u_per_cts.ToString("G8", CultureInfo.InvariantCulture);
            }
        }

        private void txtWheelCenter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            if (double.TryParse(txtWheelCenter.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double center_u)) {
                vJoyManager.Config.Hardware.WheelCenterOffset_u = center_u;
                txtWheelCenter.Text = vJoyManager.Config.Hardware.WheelCenterOffset_u.ToString("G8", CultureInfo.InvariantCulture);
            }
        }

        #endregion


    }
}
