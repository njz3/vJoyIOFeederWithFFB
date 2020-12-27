using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using BackForceFeeder.vJoyIOFeederAPI;
using BackForceFeeder.Configuration;

namespace BackForceFeederGUI.GUI
{
    public partial class AxisMappingEditor : Form
    {

        protected double Epsilon = 1.0/Math.Pow(2, 16);
        protected ControlSetDB EditedControlSet;

        public RawAxisDB InputRawDB;
        public RawAxisDB ResultRawDB { get; protected set; }
        
        public int SelectedAxis;


        public AxisMappingEditor(ControlSetDB controlSet)
        {
            EditedControlSet = controlSet;
            InitializeComponent();
        }



        System.Windows.Controls.ContextMenu lineNodeMenu = new System.Windows.Controls.ContextMenu();

        private void AxisMappingEditor_Load(object sender, EventArgs e)
        {

            lineNodeMenu.Items.Add(new MenuItem { Name = "menuItem1 header" });
            lineNodeMenu.Items.Add(new MenuItem { Name = "menuItem2 header" });

            this.grpAxisMap.DisableAnimations = true;

            this.grpAxisMap.AxisX.Add(new Axis {
                Title = "Raw % input",
                LabelFormatter = value => value.ToString("F"),
                ContextMenu = lineNodeMenu,
                Separator = new Separator // force the separator step to 1, so it always display all labels
                {
                    Step = 0.1,
                    IsEnabled = false //disable it to make it invisible.
                },
                LabelsRotation = 15
            });


            this.grpAxisMap.AxisY.Add(new Axis {
                Title = "vJoy % output",
                LabelFormatter = value => value.ToString("F"),
                ContextMenu = lineNodeMenu,
                Separator = new Separator()
            });

            this.grpAxisMap.AxisX[0].MinValue = 0.0;
            this.grpAxisMap.AxisX[0].MaxValue = 1.0;
            this.grpAxisMap.AxisY[0].MinValue = 0.0;
            this.grpAxisMap.AxisY[0].MaxValue = 1.0;

            this.grpAxisMap.DataTooltip.Visibility = System.Windows.Visibility.Collapsed;
            this.grpAxisMap.DataClick += grpAxisMapOnDataClick;


            lbSelectedPoint.Text = "Click to select a point (" + this.InputRawDB.ControlPoints.Count + " total)";
            FillLine();

            this.chkNegPedal.Checked = InputRawDB.IsNegativeDirection;
            this.chkFullRangePedal.Checked = InputRawDB.IsFullRangeAxis;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.ResultRawDB = this.InputRawDB;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public ChartValues<ObservablePoint> LineValues { get; set; }

        void FillLine()
        {
            LineValues = new ChartValues<ObservablePoint>();
            var ctlpt = InputRawDB.ControlPoints;
            for (int i = 0; i < ctlpt.Count; i++) {
                double val_in = ctlpt[i].X;
                double val_out = ctlpt[i].Y;
                var pt = new ObservablePoint(val_in, val_out);
                LineValues.Add(pt);
            }

            if (this.grpAxisMap.Series.Count == 0) {
                var line = new LineSeries {
                    ContextMenu = lineNodeMenu,
                    Values = LineValues,
                    DataLabels = false,
                    LabelPoint = point => point.Y.ToString(),
                    LineSmoothness = 0
                };
                this.grpAxisMap.Series.Add(line);
            } else {
                this.grpAxisMap.Series[0].Values = LineValues;
            }
        }




        int SelectedPoint = -1;
        private void grpAxisMapOnDataClick(object sender, ChartPoint chartPoint)
        {
            var mouse = this.grpAxisMap.PointToClient(Cursor.Position);
            var ctlpt = this.InputRawDB.ControlPoints;
            //Console.WriteLine("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")  mouse=" + mouse.X + ", " + mouse.Y);
            // Find selected point within ControlPoints
            SelectedPoint = this.InputRawDB.FindClosestControlPoint(chartPoint.X);
            trValueX.Value = (int)(ctlpt[SelectedPoint].X * 100.0);
            trValueY.Value = (int)(ctlpt[SelectedPoint].Y * 100.0);
            lbSelectedPoint.Text = "Selected point: " + SelectedPoint + "/" + this.InputRawDB.ControlPoints.Count + " total";
        }


        private void txValueX_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedPoint >= 0) {

                var ctlpt = this.InputRawDB.ControlPoints;
                // Perform range checking with neighboors to not go outside limits
                double neglim, poslim;
                if (SelectedPoint>0)
                    neglim = ctlpt[SelectedPoint-1].X+Epsilon;
                else neglim = 0;

                if (SelectedPoint<ctlpt.Count-1)
                    poslim = ctlpt[SelectedPoint+1].X-Epsilon;
                else
                    poslim = 1.0;

                // Limit slider value
                var X = Math.Min(Math.Max((double)trValueX.Value * 0.01, neglim), poslim);

                // Change X Value of current point
                var newcp = new System.Windows.Point(X, ctlpt[SelectedPoint].Y);
                ctlpt[SelectedPoint] = newcp;
                LineValues[SelectedPoint].X = X;
            }
        }

        private void trValueY_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedPoint >= 0) {
                var ctlpt = this.InputRawDB.ControlPoints;
                // Change Y Value
                var newcp = new System.Windows.Point(ctlpt[SelectedPoint].X, (double)trValueY.Value * 0.01);
                ctlpt[SelectedPoint] = newcp;
                LineValues[SelectedPoint].Y = ctlpt[SelectedPoint].Y;
            }
        }

        private void btnAddCP_Click(object sender, EventArgs e)
        {
            var ctlpt = this.InputRawDB.ControlPoints;
            int idx = SelectedPoint;
            if (idx< 0) {
                idx = 0;
            }
            if (idx >= ctlpt.Count-1) {
                idx = ctlpt.Count-2;
            }

            // use linear approximation between index and next point
            double X = (ctlpt[idx + 1].X + ctlpt[idx].X)*0.5;
            double Y = (ctlpt[idx + 1].Y + ctlpt[idx].Y)*0.5;

            ctlpt.Add(new System.Windows.Point(X, Y));
            this.InputRawDB.ControlPoints = ctlpt.OrderBy(p => p.X).ThenBy(p => p.Y).ToList<System.Windows.Point>();
            SelectedPoint = InputRawDB.FindClosestControlPoint(X);

            lbSelectedPoint.Text = "Selected point: " + SelectedPoint + "/" + this.InputRawDB.ControlPoints.Count + " total";
            FillLine();
        }

        private void btnDeleteCP_Click(object sender, EventArgs e)
        {
            var ctlpt = this.InputRawDB.ControlPoints;
            if (ctlpt.Count <= 2) {
                MessageBox.Show("Not enough points - need to keep at least 2");
                return;
            }
            if (SelectedPoint >= 0) {
                ctlpt.RemoveAt(SelectedPoint);
                this.InputRawDB.ControlPoints = ctlpt.OrderBy(p => p.X).ThenBy(p => p.Y).ToList<System.Windows.Point>();
            }
            if (SelectedPoint >= ctlpt.Count)
                SelectedPoint = ctlpt.Count - 1;

            lbSelectedPoint.Text = "Selected point: " + SelectedPoint + "/" + this.InputRawDB.ControlPoints.Count + " total";
            FillLine();
        }

        private void btCalibrateWheel_Click(object sender, EventArgs e)
        {
            var ctlpt = this.InputRawDB.ControlPoints;
            CalibrateWheelForm calibwheel = new CalibrateWheelForm();
            calibwheel.SelectedvJoyAxis = SelectedAxis;
            var res = calibwheel.ShowDialog(this);
            if (res == DialogResult.OK) {
                ctlpt.Clear();

                double X = calibwheel.RawMostLeft/4095.0;
                double Y = 0.0; // 0%
                ctlpt.Add(new System.Windows.Point(X, Y));
                X = calibwheel.RawMostCenter/4095.0;
                Y = 0.5; // 50%
                ctlpt.Add(new System.Windows.Point(X, Y));
                X = calibwheel.RawMostRight/4095.0;
                Y = 1.0; // 100%
                ctlpt.Add(new System.Windows.Point(X, Y));

                this.InputRawDB.ControlPoints = ctlpt.OrderBy(p => p.X).ThenBy(p => p.Y).ToList<System.Windows.Point>();
                SelectedPoint = InputRawDB.FindClosestControlPoint(X);
                lbSelectedPoint.Text = "Selected point: " + SelectedPoint + "/" + this.InputRawDB.ControlPoints.Count + " total";
                FillLine();
            }
        }

        private void btnCalibratePedal_Click(object sender, EventArgs e)
        {
            var ctlpt = this.InputRawDB.ControlPoints;
            CalibratePedalForm calibpedal = new CalibratePedalForm();
            calibpedal.SelectedvJoyAxis = SelectedAxis;
            var res = calibpedal.ShowDialog(this);
            if (res == DialogResult.OK) {
                ctlpt.Clear();

                double Xreleased = calibpedal.RawMostReleased/4095.0; // Released %
                double Xpressed = calibpedal.RawMostPressed/4095.0; // Pressed %

                double Ystart = 0.0; // Start point = 0%
                double Yend = 1.0; // End point = 100%

                if (chkFullRangePedal.Checked) {
                    // Full range of axis
                    if (chkNegPedal.Checked) {
                        // Negative axis
                        Ystart = 1.0; // released = 100%
                        Yend = 0.0; // pressed = 0%
                    } else {
                        Ystart = 0.0; // released = 0%
                        Yend = 1.0; // pressed = 100%
                    }
                } else {
                    // Half range, neutral point is 50%
                    if (chkNegPedal.Checked) {
                        Ystart = 0.5; // released = 50%
                        Yend = 0.0; // pressed = 0%
                    } else {
                        Ystart = 0.5; // released = 50%
                        Yend = 1.0; // pressed = 100%
                    }
                }

                ctlpt.Add(new System.Windows.Point(Xreleased, Ystart));
                ctlpt.Add(new System.Windows.Point(Xpressed, Yend));

                this.InputRawDB.ControlPoints = ctlpt.OrderBy(p => p.X).ThenBy(p => p.Y).ToList<System.Windows.Point>();
                SelectedPoint = InputRawDB.FindClosestControlPoint(Xpressed);
                lbSelectedPoint.Text = "Selected point: " + SelectedPoint + "/" + this.InputRawDB.ControlPoints.Count + " total";
                FillLine();
            }
        }


    }
}
