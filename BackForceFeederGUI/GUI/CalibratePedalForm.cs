using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.vJoyIOFeederAPI;

namespace BackForceFeederGUI.GUI
{
    public partial class CalibratePedalForm : Form
    {

        public int SelectedRawAxis;
        public double RawMostPressed_cts;
        public double RawMostReleased_cts;


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
            if (SharedData.Manager.vJoy != null) {
                var axis = SharedData.Manager.Inputs.SafeGetRawAxis(SelectedRawAxis);
                if (axis!=null) {
                    this.lbRawValue.Text = "Raw value=" + axis.RawValue_cts;
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
                        + " Pressed=" + this.RawMostPressed_cts + Environment.NewLine;
                    break;
                case CalibrateSteps.Done:
                    this.lbInstructions.Text = "Done";
                    this.btnNext.Text = "Done";
                    this.lbResult.Text = "Result:" + Environment.NewLine
                        + " Pressed=" + this.RawMostPressed_cts + Environment.NewLine
                        + " Released=" + this.RawMostReleased_cts + Environment.NewLine;
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
            var axis = SharedData.Manager.Inputs.SafeGetRawAxis(SelectedRawAxis);
            if (axis==null) {
                MessageBox.Show("Error axis is not present anymore", "Error", MessageBoxButtons.OK);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return;
            }
            // Take value
            switch (CalibrateStep) {
                case CalibrateSteps.PressMax:
                    RawMostPressed_cts = axis.RawValue_cts;
                    break;
                case CalibrateSteps.Release:
                    RawMostReleased_cts = axis.RawValue_cts;
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
