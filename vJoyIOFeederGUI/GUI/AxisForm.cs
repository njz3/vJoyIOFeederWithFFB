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

    public partial class AxisForm : Form
    {

        public AxisForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            axesJoyGauge.FromValue = -135;
            axesJoyGauge.ToValue = 135;
            axesJoyGauge.Wedge = 270;
            axesJoyGauge.LabelsStep = 270.0 / 4.0;
            axesJoyGauge.TickStep = 135 / 10.0;
            //axesGauge.DisableAnimations = true;
            axesJoyGauge.AnimationsSpeed = new TimeSpan(0, 0, 0, 0, 100);
            //axesJoyGauge.RightToLeft = RightToLeft.Yes;

            foreach (HID_USAGES toBeTested in Enum.GetValues(typeof(HID_USAGES))) {
                // Skip POV
                if (toBeTested == HID_USAGES.HID_USAGE_POV)
                    continue;
                var name = toBeTested.ToString().Replace("HID_USAGE_", "");
                cmbSelectedAxis.Items.Add(name);
            }
            cmbSelectedAxis.SelectedIndex = 0;
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }


        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            int selectedAxis = cmbSelectedAxis.SelectedIndex;
            var name = "HID_USAGE_" + cmbSelectedAxis.SelectedItem.ToString();
            Enum.TryParse<HID_USAGES>(name, out var usage);
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
            }

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
                    Program.Manager.SaveConfigurationFiles(Program.ConfigFilename);
                }
            }
        }

    }
}
