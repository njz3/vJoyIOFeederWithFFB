using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder.vJoyIOFeederAPI;

namespace IOFeederGUI.GUI
{
    public partial class CalibrateWheelForm : Form
    {

        public int SelectedAxis;
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
            if ((Program.Manager.vJoy != null) && (SelectedAxis>=0) &&
                 (Program.Manager.vJoy.AxesInfo[SelectedAxis].IsPresent) &&
                 (Program.Manager.vJoy.AxesInfo[SelectedAxis].MaxValue > 0)) {

                this.lbRawValue.Text = "Raw value (0..4095)=" + Program.Manager.vJoy.AxesInfo[SelectedAxis].RawValue.ToString();
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
            // Take value
            switch (CalibrateStep) {
                case CalibrateSteps.TurnLeft:
                    RawMostLeft = Program.Manager.vJoy.AxesInfo[SelectedAxis].RawValue;
                    break;
                case CalibrateSteps.TurnRight:
                    RawMostRight = Program.Manager.vJoy.AxesInfo[SelectedAxis].RawValue;
                    break;
                case CalibrateSteps.Center:
                    RawMostCenter = Program.Manager.vJoy.AxesInfo[SelectedAxis].RawValue;
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
