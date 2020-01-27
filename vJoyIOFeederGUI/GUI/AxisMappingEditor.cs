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
using vJoyIOFeeder.vJoyIOFeederAPI;

namespace IOFeederGUI.GUI
{
    public partial class AxisMappingEditor : Form
    {

        public vJoyFeeder.vJoyAxisInfos Input;
        public vJoyFeeder.vJoyAxisInfos Result { get; protected set; }

        public AxisMappingEditor()
        {
            InitializeComponent();
        }

       

        System.Windows.Controls.ContextMenu lineNodeMenu = new System.Windows.Controls.ContextMenu();
       
        private void AxisMappingEditor_Load(object sender, EventArgs e)
        {

            lineNodeMenu.Items.Add(new MenuItem { Name = "menuItem1 header" });
            lineNodeMenu.Items.Add(new MenuItem { Name = "menuItem2 header" });


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



            UpdateLine();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Result = this.Input;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        void UpdateLine()
        {
            var values = new ChartValues<ObservablePoint>();
            for (int i = 0; i < Input.AxisCorrection.ControlPoints.Count; i++) {
                double val_in = Input.AxisCorrection.ControlPoints[i].X;
                double val_out = Input.AxisCorrection.ControlPoints[i].Y;
                var pt = new ObservablePoint(val_in, val_out);
                values.Add(pt);
            }

            if (this.grpAxisMap.Series.Count == 0) {
                var line = new LineSeries {
                    ContextMenu = lineNodeMenu,
                    Values = values,
                    DataLabels = false,
                    LabelPoint = point => point.Y.ToString(),
                    LineSmoothness = 0
                };
                this.grpAxisMap.Series.Add(line);
            } else {
                this.grpAxisMap.Series[0].Values = values;
            }
        }



        int selectedPoint = -1;
        private void grpAxisMapOnDataClick(object sender, ChartPoint chartPoint)
        {
            var mouse = this.grpAxisMap.PointToClient(Cursor.Position);
            Console.WriteLine("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")  mouse=" + mouse.X + ", " + mouse.Y);
            // Find selected point within ControlPoints
            selectedPoint = Input.FindIndexControlPoint(chartPoint.X);
            Console.WriteLine("Deduced point = " + selectedPoint);
            trValueX.Value = (int)(Input.AxisCorrection.ControlPoints[selectedPoint].X * 100.0);
            trValueY.Value = (int)(Input.AxisCorrection.ControlPoints[selectedPoint].Y * 100.0);
            lbSelectedPoint.Text = "Selected point: " + selectedPoint;
        }


        private void txValueX_ValueChanged(object sender, EventArgs e)
        {
            if (selectedPoint >= 0) {
                // Change X Value and reorder control point (sort ascending X)
                var newcp = new System.Windows.Point((double)trValueX.Value * 0.01, Input.AxisCorrection.ControlPoints[selectedPoint].Y);
                Input.AxisCorrection.ControlPoints[selectedPoint] = newcp;
                Input.AxisCorrection.ControlPoints.OrderBy(p => p.X).ThenBy(p => p.Y);

            }

            UpdateLine();
        }

        private void trValueY_ValueChanged(object sender, EventArgs e)
        {
            if (selectedPoint >= 0) {
                // Change Y Value
                var newcp = new System.Windows.Point(Input.AxisCorrection.ControlPoints[selectedPoint].X, (double)trValueY.Value * 0.01);
                Input.AxisCorrection.ControlPoints[selectedPoint] = newcp;
            }

            UpdateLine();
        }

        private void btnAddCP_Click(object sender, EventArgs e)
        {
            int idx = selectedPoint;
            if (idx< 0) {
                idx = 0;
            }
            if (idx >= Input.AxisCorrection.ControlPoints.Count-1) {
                idx = Input.AxisCorrection.ControlPoints.Count-2;
            }

            // use linear approximation between index and next point
            double X = (Input.AxisCorrection.ControlPoints[idx + 1].X + Input.AxisCorrection.ControlPoints[idx].X)*0.5;
            double Y = (Input.AxisCorrection.ControlPoints[idx + 1].Y + Input.AxisCorrection.ControlPoints[idx].Y)*0.5;

            Input.AxisCorrection.ControlPoints.Add(new System.Windows.Point(X, Y));
            Input.AxisCorrection.ControlPoints.OrderBy(p => p.X).ThenBy(p => p.Y);
            selectedPoint = Input.FindIndexControlPoint(X);
            lbSelectedPoint.Text = "Selected point: " + selectedPoint;
            UpdateLine();
        }

        private void btnDeleteCP_Click(object sender, EventArgs e)
        {
            if (Input.AxisCorrection.ControlPoints.Count <= 2) {
                MessageBox.Show("Not enough points - need to keep at least 2");
                return;
            }
            if (selectedPoint >= 0) {
                Input.AxisCorrection.ControlPoints.RemoveAt(selectedPoint);
                Input.AxisCorrection.ControlPoints.OrderBy(p => p.X).ThenBy(p => p.Y);
            }
            if (selectedPoint >= Input.AxisCorrection.ControlPoints.Count)
                selectedPoint = Input.AxisCorrection.ControlPoints.Count - 1;

            lbSelectedPoint.Text = "Selected point: " + selectedPoint;
            UpdateLine();
        }
    }
}
