using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.Utils;

namespace IOFeederGUI.GUI
{

    public partial class TargetHdwForm : Form
    {
        public TargetHdwForm()
        {
            InitializeComponent();
        }


        private void TargetHdwForm_Load(object sender, EventArgs e)
        {
            ToolTip tooltip = new ToolTip();

            tooltip.SetToolTip(this.cmbSelectMode, "Translation mode can only be changed while manager is Stopped");
            tooltip.SetToolTip(this.btnStartStopManager, "Translation mode can only be changed while manager is Stopped");
            this.cmbSelectMode.Items.Clear();
            foreach (string mode in Enum.GetNames(typeof(FFBTranslatingModes))) {
                this.cmbSelectMode.Items.Add(mode);

                if (vJoyManager.Config.TranslatingModes.ToString().Equals(mode, StringComparison.OrdinalIgnoreCase)) {
                    this.cmbSelectMode.SelectedIndex = this.cmbSelectMode.Items.Count - 1;
                }
            }
            tbGlobalGain.Value = (int)(vJoyManager.Config.GlobalGain*10.0);
            txtGlobalGain.Text = vJoyManager.Config.GlobalGain.ToString(CultureInfo.InvariantCulture);
            tbPowerLaw.Value = (int)(vJoyManager.Config.PowerLaw*10.0);
            txtPowerLaw.Text = vJoyManager.Config.PowerLaw.ToString(CultureInfo.InvariantCulture);
        }

        private void TargetHdwForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Manager.SaveConfigurationFiles(Program.ConfigFilename);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {

            if (Program.Manager.IsRunning) {
                this.btnStartStopManager.BackColor = Color.Green;
                this.btnStartStopManager.Text = "Running (Stop)";

                this.cmbSelectMode.Enabled = false;
            } else {
                this.btnStartStopManager.BackColor = Color.Red;
                this.btnStartStopManager.Text = "Stopped (Start)";

                this.cmbSelectMode.Enabled = true;
            }

            if (Program.Manager.FFB!=null) {

                if (Program.Manager.FFB.IsDeviceReady) {
                    btnDeviceReady.BackColor = Color.Green;
                    btnDeviceReady.Text = "Ready";
                } else {
                    btnDeviceReady.BackColor = Color.Red;
                    btnDeviceReady.Text = "Not ready";
                }

                chkInvertWheel.Checked = vJoyManager.Config.InvertWheelDirection;
                chkInvertTorque.Checked = vJoyManager.Config.InvertTrqDirection;

                chkSkipStopEffect.Checked = vJoyManager.Config.SkipStopEffect;
                chkEmulateMissing.Checked = vJoyManager.Config.UseTrqEmulationForMissing;
                chkPulsedTrq.Checked = vJoyManager.Config.UsePulseSeq;

                var ffbmodel3 = Program.Manager.FFB as FFBManagerModel3;
                if (ffbmodel3!=null) {
                    chkEmulateMissing.Enabled = true;
                    chkPulsedTrq.Enabled = true;
                } else {
                    chkEmulateMissing.Enabled = false;
                    chkPulsedTrq.Enabled = false;
                }
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
                    vJoyManager.Config.TranslatingModes = mode;
                }
                Program.Manager.Start();
            } else {
                Program.Manager.Stop();
            }
        }


        private void cmbSelectMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Program.Manager.IsRunning) {
                if (Enum.TryParse<FFBTranslatingModes>(this.cmbSelectMode.SelectedItem.ToString(), out var mode)) {
                    vJoyManager.Config.TranslatingModes = mode;
                    Program.Manager.SaveConfigurationFiles(Program.ConfigFilename);
                }
            }
        }

        private void chkPulsedTrq_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.UsePulseSeq = !vJoyManager.Config.UsePulseSeq;
        }

        private void chkEmulateMissing_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.UseTrqEmulationForMissing = !vJoyManager.Config.UseTrqEmulationForMissing;
        }
        private void chkSkipStopEffect_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.SkipStopEffect = !vJoyManager.Config.SkipStopEffect;
        }

        private void txtGlobalGain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                if (double.TryParse(txtGlobalGain.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double gain)) {
                    gain = Math.Max(tbGlobalGain.Minimum*0.1, Math.Min(tbGlobalGain.Maximum*0.1, gain));
                    vJoyManager.Config.GlobalGain = gain;
                    tbGlobalGain.Value = (int)(vJoyManager.Config.GlobalGain*10.0);
                    txtGlobalGain.Text = vJoyManager.Config.GlobalGain.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        private void txtPowerLaw_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                if (double.TryParse(txtPowerLaw.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pow)) {
                    pow = Math.Max(tbPowerLaw.Minimum*0.1, Math.Min(tbPowerLaw.Maximum*0.1, pow));
                    vJoyManager.Config.PowerLaw = pow;
                    tbPowerLaw.Value = (int)(vJoyManager.Config.PowerLaw*10.0);
                    txtPowerLaw.Text = vJoyManager.Config.PowerLaw.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void tbGlobalGain_Scroll(object sender, EventArgs e)
        {
            double gain = tbGlobalGain.Value*0.1;
            vJoyManager.Config.GlobalGain = gain;
            txtGlobalGain.Text = vJoyManager.Config.GlobalGain.ToString(CultureInfo.InvariantCulture);
        }

        private void tbPowerLaw_Scroll(object sender, EventArgs e)
        {
            double pow = tbPowerLaw.Value*0.1;
            vJoyManager.Config.PowerLaw = pow;
            txtPowerLaw.Text = vJoyManager.Config.PowerLaw.ToString(CultureInfo.InvariantCulture);
        }


    }
}
