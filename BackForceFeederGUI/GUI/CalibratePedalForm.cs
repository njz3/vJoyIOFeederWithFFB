using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackForceFeeder.vJoyIOFeederAPI;

namespace BackForceFeederGUI.GUI
{
    public partial class CalibratePedalForm : Form
    {

        public int SelectedvJoyAxis;
        public long RawMostPressed;
        public long RawMostReleased;


        public CalibratePedalForm()
        {
            InitializeComponent();
        }

        enum CalibrateSteps : int
        {
            PressMax = 0,
            Release,
            Done,
        }

        CalibrateSteps CalibrateStep;
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Raw value
            if (Program.Manager.vJoy != null) {
                var axis = Program.Manager.vJoy.SafeGetUsedAxis(SelectedvJoyAxis);
                if (axis!=null) {
                    this.lbRawValue.Text = "Raw value (0..4095)=" + axis.vJoyAxisInfo.RawValue.ToString();
                }
            }


            // Steps
            switch (CalibrateStep) {
                case CalibrateSteps.PressMax:
                    this.lbInstructions.Text = "Press pedal to maximum";
                    this.btnNext.Text = "Next";
                    this.lbResult.Text = "Result:" + Environment.NewLine;
                    break;
                case CalibrateSteps.Release:
                    this.lbInstructions.Text = "Release pedal completely";
                    this.btnNext.Text = "Next";
                    this.lbResult.Text = "Result:" + Environment.NewLine
                        + " Pressed=" + this.RawMostPressed + Environment.NewLine;
                    break;
                case CalibrateSteps.Done:
                    this.lbInstructions.Text = "Done";
                    this.btnNext.Text = "Done";
                    this.lbResult.Text = "Result:" + Environment.NewLine
                        + " Pressed=" + this.RawMostPressed + Environment.NewLine
                        + " Released=" + this.RawMostReleased + Environment.NewLine;
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            var axis = Program.Manager.vJoy.SafeGetUsedAxis(SelectedvJoyAxis);
            if (axis==null) {
                MessageBox.Show("Error axis is not present anymore", "Error", MessageBoxButtons.OK);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }
            // Take value
            switch (CalibrateStep) {
                case CalibrateSteps.PressMax:
                    RawMostPressed = axis.vJoyAxisInfo.RawValue;
                    break;
                case CalibrateSteps.Release:
                    RawMostReleased = axis.vJoyAxisInfo.RawValue;
                    break;
                case CalibrateSteps.Done:
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
            }

            CalibrateStep++;
        }
    }
}
