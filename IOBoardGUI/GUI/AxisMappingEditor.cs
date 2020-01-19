using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
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

        private void AxisMappingEditor_Load(object sender, EventArgs e)
        {
            var values = new ChartValues<ObservablePoint>();
            for (int i = 0; i <= 10; i++) {
                var pt = new ObservablePoint(i * 0.1, i * 0.1);
                values.Add(pt);
            }
            this.grpAxisMap.Series.Add(
                new LineSeries {
                    Values = values,
                    DataLabels = true,
                    LabelPoint = point => point.Y + "%",
                    LineSmoothness = 0
                });


            this.grpAxisMap.AxisX.Add(new Axis {
                Title = "Raw % input",
                LabelFormatter = value => value.ToString("F"),

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
                Separator = new Separator()
            });

            this.grpAxisMap.AxisX[0].MinValue = 0.0;
            this.grpAxisMap.AxisX[0].MaxValue = 1.0;
            this.grpAxisMap.AxisY[0].MinValue = 0.0;
            this.grpAxisMap.AxisY[0].MaxValue = 1.0;

            this.grpAxisMap.DataTooltip.Visibility = System.Windows.Visibility.Collapsed;
            this.grpAxisMap.DataClick += grpAxisMapOnDataClick;
            this.grpAxisMap.DataHover += grpAxisMapOnDataHoverClick;
            this.MouseClick += OnClick;
        }

        void OnClick(object sender, MouseEventArgs e)
        {
            var pt = this.grpAxisMap.PointToClient(Cursor.Position);
            Console.WriteLine("You clicked mouse=" + pt.X + ", " + pt.Y);
        }

        int selectedPoint = -1;
        ChartPoint SelectedChartPoint = null;
        private void grpAxisMapOnDataClick(object sender, ChartPoint chartPoint)
        {
            var pt = this.grpAxisMap.PointToClient(Cursor.Position);
            Console.WriteLine("You clicked (" + chartPoint.X + "," + chartPoint.Y + ")  mouse=" + pt.X + ", " + pt.Y);
            SelectedChartPoint = chartPoint;
            selectedPoint = (int)(chartPoint.X * 10);
            trValue.Value = (int)(chartPoint.Y*100.0);
        }
        private void grpAxisMapOnDataHoverClick(object sender, ChartPoint chartPoint)
        {
            var pt = this.grpAxisMap.PointToClient(Cursor.Position);
            Console.WriteLine("You hover on (" + chartPoint.X + "," + chartPoint.Y + ")  mouse=" + pt.X + ", " + pt.Y);
        }

        private void trValue_ValueChanged(object sender, EventArgs e)
        {
            if (selectedPoint >= 0) {
                
                var pt = new ObservablePoint(selectedPoint * 0.1, (double)trValue.Value * 0.01);
                this.grpAxisMap.Series[0].Values[selectedPoint] = pt;
               
            }
        }
    }
}
