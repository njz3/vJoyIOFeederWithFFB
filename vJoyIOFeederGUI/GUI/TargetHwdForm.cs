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

                if (Program.Manager.Config.TranslatingModes.ToString().Equals(mode, StringComparison.OrdinalIgnoreCase)) {
                    this.cmbSelectMode.SelectedIndex = this.cmbSelectMode.Items.Count - 1;
                }
            }
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

                if (Program.Manager.FFB.WheelSign<0.0) {
                    chkInvertWheel.Checked = true;
                } else {
                    chkInvertWheel.Checked = false;
                }
                if (Program.Manager.FFB.TrqSign<0.0) {
                    chkInvertTorque.Checked = true;
                } else {
                    chkInvertTorque.Checked = false;
                }

                var ffbmodel3 = Program.Manager.FFB as FFBManagerModel3;
                if (ffbmodel3!=null) {
                    chkEmulateMissing.Enabled = true;
                    chkPulsedTrq.Enabled = true;
                    chkEmulateMissing.Checked = ffbmodel3.UseTrqEmulation;
                    chkPulsedTrq.Checked = ffbmodel3.UsePulseSeq;
                } else {
                    chkEmulateMissing.Enabled = false;
                    chkPulsedTrq.Enabled = false;
                    chkEmulateMissing.Checked = false;
                    chkPulsedTrq.Checked = false;
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
                    Program.Manager.Config.TranslatingModes = mode;
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
                    Program.Manager.Config.TranslatingModes = mode;
                    Program.Manager.SaveConfigurationFiles(Program.ConfigPath);
                }
            }
        }

        private void chkPulsedTrq_Click(object sender, EventArgs e)
        {
            var ffbmodel3 = Program.Manager.FFB as FFBManagerModel3;
            if (ffbmodel3!=null) {
                ffbmodel3.UsePulseSeq = !ffbmodel3.UsePulseSeq;
            }
        }

        private void chkEmulateMissing_Click(object sender, EventArgs e)
        {
            var ffbmodel3 = Program.Manager.FFB as FFBManagerModel3;
            if (ffbmodel3!=null) {
                ffbmodel3.UseTrqEmulation = !ffbmodel3.UseTrqEmulation;
            }
        }
    }
}
