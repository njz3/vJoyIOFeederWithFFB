using BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Utils;
using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace BackForceFeederGUI.GUI
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

                if (BFFManager.Config.Hardware.TranslatingModes.ToString().Equals(mode, StringComparison.OrdinalIgnoreCase)) {
                    this.cmbSelectMode.SelectedIndex = this.cmbSelectMode.Items.Count - 1;
                }
            }

            this.txtWheelScale.Text = BFFManager.Config.Hardware.WheelScaleFactor_u_per_cts.ToString("G8", CultureInfo.InvariantCulture);
            this.txtWheelCenter.Text = BFFManager.Config.Hardware.WheelCenterOffset_u.ToString("G8", CultureInfo.InvariantCulture);

            this.cmbBaudrate.Items.Clear();
            UInt32[] speedlist = { 1000000, 500000, 115200, 57600 };
            foreach (var speed in speedlist) {
                this.cmbBaudrate.Items.Add(speed.ToString());

                if (BFFManager.Config.Hardware.SerialPortSpeed.ToString().Equals(speed.ToString(), StringComparison.OrdinalIgnoreCase)) {
                    this.cmbBaudrate.SelectedIndex = this.cmbBaudrate.Items.Count - 1;
                }
            }
            if (BFFManager.Config.Application.DebugModeGUI) {
                this.btnDebugMode.Visible = true;
            }
        }

        private void TargetHdwForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SharedData.Manager.SaveConfigurationFiles(SharedData.AppCfgFilename, SharedData.HwdCfgFilename);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            this.chkBoxStartMinimized.Checked = BFFManager.Config.Application.StartMinimized;
            this.chkBoxStartWithWindows.Checked = BFFManager.Config.Application.ShortcutStartWithWindowsCreated;
            this.chkDumpLogToFile.Checked = BFFManager.Config.Application.DumpLogToFile;
            this.chkOutputOnly.Checked = BFFManager.Config.Application.OutputOnly;
            this.chkAutodetectControlSet.Checked = BFFManager.Config.Application.AutodetectControlSetAtRuntime;
            this.chkResetFFBOnControlSetChange.Checked = BFFManager.Config.Application.ResetFFBOnControlSetChange;

            if (SharedData.Manager.IsRunning) {
                this.btnStartStopManager.BackColor = Color.Green;
                this.btnStartStopManager.Text = "Running (Stop)";

                this.cmbSelectMode.Enabled = false;
                this.cmbBaudrate.Enabled = false;
                this.chkDualModePWM.Enabled = false;
                this.chkDigitalPWM.Enabled = false;
                this.chkAlternativePinFFBController.Enabled = false;
            } else {
                this.btnStartStopManager.BackColor = Color.Red;
                this.btnStartStopManager.Text = "Stopped (Start)";

                this.cmbSelectMode.Enabled = true;
                this.cmbBaudrate.Enabled = true;
                this.chkDualModePWM.Enabled = true;
                this.chkDigitalPWM.Enabled = true;
                this.chkAlternativePinFFBController.Enabled = true;
            }

            if (SharedData.Manager.FFB!=null) {

                if (SharedData.Manager.FFB.IsDeviceReady) {
                    btnDeviceReady.BackColor = Color.Green;
                    btnDeviceReady.Text = "Ready";
                    btnCommit.Enabled = true;
                } else {
                    btnDeviceReady.BackColor = Color.Red;
                    btnDeviceReady.Text = "Not ready";
                    btnCommit.Enabled = false;
                }

                chkInvertWheel.Checked = BFFManager.Config.Hardware.InvertWheelDirection;
                chkInvertTorque.Checked = BFFManager.Config.Hardware.InvertTrqDirection;
                chkDualModePWM.Checked =  BFFManager.Config.Hardware.DualModePWM;
                chkDigitalPWM.Checked = BFFManager.Config.Hardware.DigitalPWM;
                chkAlternativePinFFBController.Checked = BFFManager.Config.Hardware.AlternativePinFFBController;
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
            if (!SharedData.Manager.IsRunning) {
                if (Enum.TryParse<FFBTranslatingModes>(this.cmbSelectMode.SelectedItem.ToString(), out var mode)) {
                    BFFManager.Config.Hardware.TranslatingModes = mode;
                }
                SharedData.Manager.Start();
            } else {
                SharedData.Manager.Stop();
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
                if (SharedData.Manager.IsRunning) {
                    SharedData.Manager.Stop();
                }
                BFFManager.Config.Hardware = new BackForceFeeder.Configuration.HardwareDB();
            }
        }
        private void cmbBaudrate_SelectedIndexChanged(object sender, EventArgs e)
        {
            var speed = this.cmbBaudrate.SelectedItem as string;
            if (speed!=null) {
                UInt32.TryParse(speed, out BFFManager.Config.Hardware.SerialPortSpeed);
            }
        }

        #region Application configuration
        private void chkBoxStartMinimized_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Application.StartMinimized = !BFFManager.Config.Application.StartMinimized;
        }

        private void chkBoxStartWithWindows_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Application.ShortcutStartWithWindowsCreated = !BFFManager.Config.Application.ShortcutStartWithWindowsCreated;
            if (BFFManager.Config.Application.ShortcutStartWithWindowsCreated) {
                // Create shortcut
                OSUtilities.CreateStartupShortcut("vJoyIOFeederGUI", "vJoyIOFeederGUI auto-startup");
            } else {
                OSUtilities.DeleteStartupShortcut("vJoyIOFeederGUI");
            }
        }

        private void chkDumpLogToFile_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Application.DumpLogToFile = !BFFManager.Config.Application.DumpLogToFile;
        }

        private void chkAutodetectControlSet_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Application.AutodetectControlSetAtRuntime = !BFFManager.Config.Application.AutodetectControlSetAtRuntime;
        }
        private void chkOutputOnly_Click(object sender, EventArgs e)
        {
            if (SharedData.Manager.IsRunning) {
                SharedData.Manager.Stop();
            }
            BFFManager.Config.Application.OutputOnly = !BFFManager.Config.Application.OutputOnly;
        }
        #endregion

        #region Hardware properties

        private void chkInvertWheel_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Hardware.InvertWheelDirection = !BFFManager.Config.Hardware.InvertWheelDirection;
        }
        private void chkInvertTorque_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Hardware.InvertTrqDirection = !BFFManager.Config.Hardware.InvertTrqDirection;
        }

        private void chkDualModePWM_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Hardware.DualModePWM = !BFFManager.Config.Hardware.DualModePWM;
        }
        private void chkDigitalPWM_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Hardware.DigitalPWM = !BFFManager.Config.Hardware.DigitalPWM;
        }
        private void chkUseAlternativeFFBPinout_Click(object sender, EventArgs e)
        {
            BFFManager.Config.Hardware.AlternativePinFFBController = !BFFManager.Config.Hardware.AlternativePinFFBController;
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            if (SharedData.Manager.IOboard!=null) {
                SharedData.Manager.IOboard.SendCommand("savecfg");
                Thread.Sleep(200);
                SharedData.Manager.IOboard.ResetBoard();
                Thread.Sleep(200);
                Application.DoEvents();
                if (!SharedData.Manager.IsRunning) {
                    SharedData.Manager.Start();
                } else {
                    SharedData.Manager.Stop();
                    Thread.Sleep(500);
                    Application.DoEvents();
                    SharedData.Manager.Start(); 
                    Thread.Sleep(500);
                    Application.DoEvents();

                }
            }
        }

        private void cmbSelectMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SharedData.Manager.IsRunning) {
                if (Enum.TryParse<FFBTranslatingModes>(this.cmbSelectMode.SelectedItem.ToString(), out var mode)) {
                    BFFManager.Config.Hardware.TranslatingModes = mode;
                    SharedData.Manager.SaveConfigurationFiles(SharedData.AppCfgFilename, SharedData.HwdCfgFilename);
                }
            }
        }

        private void btnMotorCalibrate_Click(object sender, EventArgs e)
        {
            CalibrateWheelForm calibwheel = new CalibrateWheelForm();
            calibwheel.SelectedRawAxis = 0;
            var res = calibwheel.ShowDialog(this);
            if (res == DialogResult.OK) {
                double range_cts = calibwheel.RawMostLeft_cts - calibwheel.RawMostRight_cts;
                double scale_u_per_cts = 2.0/range_cts;
                BFFManager.Config.Hardware.WheelScaleFactor_u_per_cts = scale_u_per_cts;
                txtWheelScale.Text = BFFManager.Config.Hardware.WheelScaleFactor_u_per_cts.ToString("G8", CultureInfo.InvariantCulture);

                double center_u = calibwheel.RawMostCenter_cts*scale_u_per_cts;
                BFFManager.Config.Hardware.WheelCenterOffset_u = center_u;
                txtWheelCenter.Text = BFFManager.Config.Hardware.WheelCenterOffset_u.ToString("G8", CultureInfo.InvariantCulture);
            }
        }

        private void txtWheelScale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;

            if (double.TryParse(txtWheelScale.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double scale_u_per_cts)) {
                BFFManager.Config.Hardware.WheelScaleFactor_u_per_cts = scale_u_per_cts;
                txtWheelScale.Text = BFFManager.Config.Hardware.WheelScaleFactor_u_per_cts.ToString("G8", CultureInfo.InvariantCulture);
            }
        }

        private void txtWheelCenter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            if (double.TryParse(txtWheelCenter.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double center_u)) {
                BFFManager.Config.Hardware.WheelCenterOffset_u = center_u;
                txtWheelCenter.Text = BFFManager.Config.Hardware.WheelCenterOffset_u.ToString("G8", CultureInfo.InvariantCulture);
            }
        }


        #endregion

        bool debugmode = false;
        private void btnDebugMode_Click(object sender, EventArgs e)
        {
            if (SharedData.Manager.IOboard!=null) {
                debugmode = !debugmode;
                SharedData.Manager.IOboard.DebugMode(debugmode);
            }

        }


    }
}
