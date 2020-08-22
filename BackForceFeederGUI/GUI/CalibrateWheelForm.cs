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
    public partial class CalibrateWheelForm : Form
    {

        public int SelectedvJoyAxis;
        public long RawMostLeft;
        public long RawMostRight;
        public long RawMostCenter;


        public CalibrateWheelForm()
        {
            InitializeComponent();
        }

        enum CalibrateSteps : int
        {
            TurnLeft = 0,
            TurnRight,
            Center,
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
                case CalibrateSteps.TurnLeft:
                    this.lbInstructions.Text = "Turn wheel maximum left";
                    this.btnNext.Text = "Next";
                    this.lbResult.Text = "Result:" + Environment.NewLine;
                    break;
                case CalibrateSteps.TurnRight:
                    this.lbInstructions.Text = "Turn wheel maximum right";
                    this.btnNext.Text = "Next";
                    this.lbResult.Text = "Result:" + Environment.NewLine 
                        + " Mostleft=" + this.RawMostLeft + Environment.NewLine;
                    break;
                case CalibrateSteps.Center:
                    this.lbInstructions.Text = "Center wheel";
                    this.btnNext.Text = "Next";
                    this.lbResult.Text = "Result:" + Environment.NewLine
                        + " Most left=" + this.RawMostLeft + Environment.NewLine
                        + " Most right=" + this.RawMostRight + Environment.NewLine;
                    break;
                case CalibrateSteps.Done:
                    this.lbInstructions.Text = "Done";
                    this.btnNext.Text = "Done";
                    this.lbResult.Text = "Result:" + Environment.NewLine
                        + " Most left=" + this.RawMostLeft + Environment.NewLine
                        + " Most right=" + this.RawMostRight + Environment.NewLine
                        + " Center=" + this.RawMostCenter + Environment.NewLine;
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
                case CalibrateSteps.TurnLeft:
                    RawMostLeft = axis.vJoyAxisInfo.RawValue;
                    break;
                case CalibrateSteps.TurnRight:
                    RawMostRight = axis.vJoyAxisInfo.RawValue;
                    break;
                case CalibrateSteps.Center:
                    RawMostCenter = axis.vJoyAxisInfo.RawValue;
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
