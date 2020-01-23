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
            for (int i = 0; i < Input.ControlPoints.Count; i++) {
                double val_in = Input.ControlPoints[i].Item1;
                double val_out = Input.ControlPoints[i].Item2;
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
            trValueX.Value = (int)(Input.ControlPoints[selectedPoint].Item1 * 100.0);
            trValueY.Value = (int)(Input.ControlPoints[selectedPoint].Item2 * 100.0);
            lbSelectedPoint.Text = "Selected point: " + selectedPoint;
        }


        private void txValueX_ValueChanged(object sender, EventArgs e)
        {
            if (selectedPoint >= 0) {
                // Change X Value and reorder control point (sort ascending X)
                var newcp = new Tuple<double, double>((double)trValueX.Value * 0.01, Input.ControlPoints[selectedPoint].Item2);
                Input.ControlPoints[selectedPoint] = newcp;
                Input.ControlPoints.Sort();
            }

            UpdateLine();
        }

        private void trValueY_ValueChanged(object sender, EventArgs e)
        {
            if (selectedPoint >= 0) {
                // Change Y Value
                var newcp = new Tuple<double, double>(Input.ControlPoints[selectedPoint].Item1, (double)trValueY.Value * 0.01);
                Input.ControlPoints[selectedPoint] = newcp;
            }

            UpdateLine();
        }

        private void btnAddCP_Click(object sender, EventArgs e)
        {
            int idx = selectedPoint;
            if (idx< 0) {
                idx = 0;
            }
            if (idx >= Input.ControlPoints.Count-1) {
                idx = Input.ControlPoints.Count-2;
            }

            // use linear approximation between index and next point
            double X = (Input.ControlPoints[idx + 1].Item1 + Input.ControlPoints[idx].Item1)*0.5;
            double Y = (Input.ControlPoints[idx + 1].Item2 + Input.ControlPoints[idx].Item2)*0.5;

            Input.ControlPoints.Add(new Tuple<double, double>(X, Y));
            Input.ControlPoints.Sort();
            selectedPoint = Input.FindIndexControlPoint(X);
            lbSelectedPoint.Text = "Selected point: " + selectedPoint;
            UpdateLine();
        }

        private void btnDeleteCP_Click(object sender, EventArgs e)
        {
            if (Input.ControlPoints.Count <= 2) {
                MessageBox.Show("Not enough points - need to keep at least 2");
                return;
            }
            if (selectedPoint >= 0) {
                Input.ControlPoints.RemoveAt(selectedPoint);
                Input.ControlPoints.Sort();
            }
            if (selectedPoint >= Input.ControlPoints.Count)
                selectedPoint = Input.ControlPoints.Count - 1;

            lbSelectedPoint.Text = "Selected point: " + selectedPoint;
            UpdateLine();
        }
    }
}
