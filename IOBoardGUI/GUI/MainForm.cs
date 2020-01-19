using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;
using vJoyIOFeeder.Utils;

namespace IOFeederGUI.GUI
{

    public partial class MainForm : Form
    {
        public vJoyManager Manager;

        public LogForm Log;

        public MainForm()
        {
            InitializeComponent();

            axesJoyGauge.FromValue = -135;
            axesJoyGauge.ToValue = 136;
            axesJoyGauge.LabelsStep = 67;
            axesJoyGauge.TickStep = 13;
            //axesGauge.DisableAnimations = true;
            axesJoyGauge.AnimationsSpeed = new TimeSpan(0, 0, 0, 0, 100);

            cmbSelectedAxis.SelectedIndex = 0;
            for (int i = 1; i <= 8; i++) {
                cmbBtnMapFrom.Items.Add(i.ToString());
                cmbBtnMapTo.Items.Add(i.ToString());
            }


            Log = new LogForm();
            // Must do this to create controls and allow for Log() to have 
            // right thread ID when calling InvokeReduired
            Log.Show();
            Log.Hide();

            Logger.Start();
            Logger.Loggers += Log.Log;

            Manager = new vJoyManager();
            Manager.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager.Stop();
            Logger.Stop();
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


            if (Manager.IsRunning) {
                this.btnStatusThread.BackColor = Color.Green;
                this.btnStatusThread.Text = "Running";
                this.btnStatusThread.Enabled = false;
            } else {
                this.btnStatusThread.BackColor = Color.Red;
                this.btnStatusThread.Text = "Stopped";
                this.btnStatusThread.Enabled = true;
            }

            int selectedAxis = cmbSelectedAxis.SelectedIndex;

            if ((Manager.vJoy != null) &&
                (Manager.vJoy.AxesInfo[selectedAxis].IsPresent) &&
                (Manager.vJoy.AxesInfo[selectedAxis].MaxValue > 0)) {

                txtRawAxisValue.Text = Manager.vJoy.AxesInfo[selectedAxis].RawValue.ToString();
                txtJoyAxisValue.Text = Manager.vJoy.AxesInfo[selectedAxis].CorrectedValue.ToString();

                slJoyAxis.Maximum = (int)Manager.vJoy.AxesInfo[selectedAxis].MaxValue;
                slJoyAxis.Value = (int)Manager.vJoy.AxesInfo[selectedAxis].CorrectedValue;
                slRawAxis.Maximum = slJoyAxis.Maximum;
                slRawAxis.Value = slJoyAxis.Value;

                axesJoyGauge.Value = (((double)slRawAxis.Value / (double)slRawAxis.Maximum) - 0.5) * 270;

                
                var listChk = new List<CheckBox>() {
                    chkBtn1, chkBtn2, chkBtn3, chkBtn4, chkBtn5, chkBtn6, chkBtn7, chkBtn8
                };

                for (int i = 0; i < 8; i++) {
                    var chk = listChk[i];
                    if ((Manager.vJoy.Report.Buttons & (1 << i)) != 0)
                        chk.Checked = true;
                    else
                        chk.Checked = false;
                }

            } else {
                txtRawAxisValue.Text = "NA";
                txtJoyAxisValue.Text = "NA";
                slRawAxis.Value = 0;
                slJoyAxis.Value = 0;
            }

        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                menuShow_Click(sender, e);
            }
        }

        private void slAxisRy_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }


        private void cmbSelectedAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
           
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

        private void btnShowLogWindow_Click(object sender, EventArgs e)
        {
            Log.Show();
        }

        private void btnStatusThread_Click(object sender, EventArgs e)
        {
            if (!Manager.IsRunning) {
                Manager.Start();
            }
        }

        private void btnAxisMappingEditor_Click(object sender, EventArgs e)
        {
            int selectedAxis = cmbSelectedAxis.SelectedIndex;

            if ((Manager.vJoy != null) &&
                (Manager.vJoy.AxesInfo[selectedAxis].IsPresent) &&
                (Manager.vJoy.AxesInfo[selectedAxis].MaxValue > 0)) {
                AxisMappingEditor editor = new AxisMappingEditor();
                editor.Input = Manager.vJoy.AxesInfo[selectedAxis];
                var res = editor.ShowDialog(this);
                if (res== DialogResult.OK) {
                    Manager.vJoy.AxesInfo[selectedAxis] = editor.Result;
                }
            }
        }
    }
}
