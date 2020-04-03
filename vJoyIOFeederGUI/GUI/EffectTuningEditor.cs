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
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;
using vJoyIOFeeder.FFBAgents;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeederGUI.GUI
{

    public partial class EffectTuningEditor : Form
    {
        public EffectTuningEditor()
        {
            InitializeComponent();
        }


        private void TargetHdwForm_Load(object sender, EventArgs e)
        {
            ToolTip tooltip = new ToolTip();

            tbGlobalGain.Value = (int)(vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain*10.0);
            txtGlobalGain.Text = vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain.ToString(CultureInfo.InvariantCulture);
            tbPowerLaw.Value = (int)(vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw*10.0);
            txtPowerLaw.Text = vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw.ToString(CultureInfo.InvariantCulture);
            tbTrqDeadBand.Value = (int)(vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand*100.0);
            txtTrqDeadBand.Text = vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand.ToString(CultureInfo.InvariantCulture);

        }

        private void TargetHdwForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Manager.SaveConfigurationFiles(Program.AppCfgFilename, Program.HwdCfgFilename, Program.CtlSetsCfgFilename);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            lbCurrentControlSet.Text = "Control set: " + vJoyManager.Config.CurrentControlSet.UniqueName + " (" + vJoyManager.Config.CurrentControlSet.GameName + ")";
            if (Program.Manager.FFB!=null) {
                chkSkipStopEffect.Checked = vJoyManager.Config.CurrentControlSet.FFBParams.SkipStopEffect;
                chkEmulateMissing.Checked = vJoyManager.Config.CurrentControlSet.FFBParams.UseTrqEmulationForMissing;
                chkPulsedTrq.Checked = vJoyManager.Config.CurrentControlSet.FFBParams.UsePulseSeq;
                chkForceTorque.Checked = vJoyManager.Config.CurrentControlSet.FFBParams.ForceTrqForAllCommands;

                var ffbmodel3 = Program.Manager.FFB as FFBManagerModel3;
                if (ffbmodel3!=null) {
                    chkEmulateMissing.Enabled = true;
                    chkPulsedTrq.Enabled = true;
                    chkForceTorque.Enabled = true;
                } else {
                    chkEmulateMissing.Enabled = false;
                    chkPulsedTrq.Enabled = false;
                    chkForceTorque.Enabled = false;
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

        #region Common force effect properties
        private void txtGlobalGain_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                if (double.TryParse(txtGlobalGain.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double gain)) {
                    gain = Math.Max(tbGlobalGain.Minimum*0.1, Math.Min(tbGlobalGain.Maximum*0.1, gain));
                    vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain = gain;
                    tbGlobalGain.Value = (int)(vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain*10.0);
                    txtGlobalGain.Text = vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        private void txtPowerLaw_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                if (double.TryParse(txtPowerLaw.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pow)) {
                    pow = Math.Max(tbPowerLaw.Minimum*0.1, Math.Min(tbPowerLaw.Maximum*0.1, pow));
                    vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw = pow;
                    tbPowerLaw.Value = (int)(vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw*10.0);
                    txtPowerLaw.Text = vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        private void txtTrqDeadBand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) {
                if (double.TryParse(txtTrqDeadBand.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double deadband)) {
                    deadband = Math.Max(tbTrqDeadBand.Minimum*0.01, Math.Min(tbTrqDeadBand.Maximum*0.01, deadband));
                    vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand = deadband;
                    tbTrqDeadBand.Value = (int)(vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand*10.0);
                    txtTrqDeadBand.Text = vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void tbGlobalGain_Scroll(object sender, EventArgs e)
        {
            double gain = tbGlobalGain.Value*0.1;
            vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain = gain;
            txtGlobalGain.Text = vJoyManager.Config.CurrentControlSet.FFBParams.GlobalGain.ToString(CultureInfo.InvariantCulture);
        }

        private void tbPowerLaw_Scroll(object sender, EventArgs e)
        {
            double pow = tbPowerLaw.Value*0.1;
            vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw = pow;
            txtPowerLaw.Text = vJoyManager.Config.CurrentControlSet.FFBParams.PowerLaw.ToString(CultureInfo.InvariantCulture);
        }

        private void tbTrqDeadBand_Scroll(object sender, EventArgs e)
        {
            double deadband = tbTrqDeadBand.Value*0.01;
            vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand = deadband;
            txtTrqDeadBand.Text = vJoyManager.Config.CurrentControlSet.FFBParams.TrqDeadBand.ToString(CultureInfo.InvariantCulture);
        }

        private void chkSkipStopEffect_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.CurrentControlSet.FFBParams.SkipStopEffect = !vJoyManager.Config.CurrentControlSet.FFBParams.SkipStopEffect;
        }

        #endregion

        #region Specific mode properties
        private void chkPulsedTrq_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.CurrentControlSet.FFBParams.UsePulseSeq = !vJoyManager.Config.CurrentControlSet.FFBParams.UsePulseSeq;
        }

        private void chkEmulateMissing_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.CurrentControlSet.FFBParams.UseTrqEmulationForMissing = !vJoyManager.Config.CurrentControlSet.FFBParams.UseTrqEmulationForMissing;
        }

        private void chkForceTorque_Click(object sender, EventArgs e)
        {
            vJoyManager.Config.CurrentControlSet.FFBParams.ForceTrqForAllCommands = !vJoyManager.Config.CurrentControlSet.FFBParams.ForceTrqForAllCommands;
        }

        #endregion

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
                try {
                    var index = vJoyManager.Config.AllControlSets.ControlSets.FindIndex(x => (x.UniqueName == vJoyManager.Config.CurrentControlSet.UniqueName));
                    vJoyManager.Config.AllControlSets.ControlSets[index] = new vJoyIOFeeder.Configuration.ControlSetDB();
                } catch (Exception ex) {
                }
            }
        }
    }
}
