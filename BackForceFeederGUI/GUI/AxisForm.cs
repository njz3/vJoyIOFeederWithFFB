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

    public partial class AxisForm : Form
    {
        protected ControlSetDB EditedControlSet;

        public AxisForm(ControlSetDB controlSet)
        {
            EditedControlSet = controlSet;
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

            cmbSelectedAxis.Items.Clear();
            cmbSelectedAxis.SelectedIndex = -1;
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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

            int selectedAxis = cmbSelectedAxis.SelectedIndex;

            if (Program.Manager.vJoy != null) {
                var axis = Program.Manager.vJoy.SafeGetUsedAxis(selectedAxis);
                if (axis!=null) {

                    txtRawAxisValue.Text = axis.vJoyAxisInfo.RawValue.ToString();
                    slRawAxis.Maximum = 4095;
                    slRawAxis.Value = (int)axis.vJoyAxisInfo.RawValue;

                    txtJoyAxisValue.Text = axis.vJoyAxisInfo.CorrectedValue.ToString();
                    slJoyAxis.Maximum = (int)axis.vJoyAxisInfo.MaxValue;
                    slJoyAxis.Value = (int)axis.vJoyAxisInfo.CorrectedValue;

                    axesJoyGauge.Value = (((double)slJoyAxis.Value / (double)slJoyAxis.Maximum) - 0.5) * 270;
                }
            } else {
                txtRawAxisValue.Text = "NA";
                txtJoyAxisValue.Text = "NA";
                slRawAxis.Value = 0;
                slJoyAxis.Value = 0;
            }

        }

        private void btnAxisMappingEditor_Click(object sender, EventArgs e)
        {
            int selectedvJoyIndexAxis = cmbSelectedAxis.SelectedIndex;

            if (Program.Manager.vJoy == null) return;

            var axis = Program.Manager.vJoy.SafeGetUsedAxis(selectedvJoyIndexAxis);
            if (axis==null) return;

            AxisMappingEditor editor = new AxisMappingEditor(this.EditedControlSet);
            editor.SelectedAxis = selectedvJoyIndexAxis;
            editor.InputRawDB = (RawAxisDB)axis.RawAxisDB.Clone();
            var res = editor.ShowDialog(this);
            if (res == DialogResult.OK) {
                axis.RawAxisDB = editor.ResultRawDB;
                Program.Manager.SaveControlSetFiles();
            }
            editor.Dispose();
        }

    }
}
