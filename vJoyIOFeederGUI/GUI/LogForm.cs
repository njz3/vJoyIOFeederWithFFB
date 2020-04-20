using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;
using vJoyIOFeeder.Utils;

namespace vJoyIOFeederGUI.GUI
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();

            Logger.Loggers += Log;
            txtLog.HideSelection = false;
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            this.cmbLogLevel.Items.Clear();
            foreach (string level in Enum.GetNames(typeof(LogLevels))) {
                this.cmbLogLevel.Items.Add(level);
                if (Logger.LogLevel.ToString().Equals(level, StringComparison.OrdinalIgnoreCase)) {
                    this.cmbLogLevel.SelectedIndex = this.cmbLogLevel.Items.Count - 1;
                }
            }
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        StringBuilder Newlog = new StringBuilder();
        public void Log(string text)
        {
            if (!this.IsHandleCreated)
                return;
            Newlog.Clear();
            Newlog.Append(DateTime.Now.ToLongTimeString());
            Newlog.Append(" | ");
            Newlog.AppendLine(text);
            string line = Newlog.ToString();
            txtLog.BeginInvoke((Action)(()=>{ txtLog.AppendText(line); } ));
        }


        private void cmbLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse<LogLevels>(this.cmbLogLevel.SelectedItem.ToString(), true, out var level)) {
                Logger.LogLevel = level;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtLog.Clear();
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            var savedialog = new SaveFileDialog();
            savedialog.Title = "Save log file";
            savedialog.DefaultExt = ".txt";
            savedialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            var now = DateTime.Now;
            savedialog.FileName = "log-" + now.ToString("yyyy-MM-ddTHH-mm-ss") + ".txt";
            var result = savedialog.ShowDialog();
            if (result == DialogResult.OK) {
                using (StreamWriter sw = File.CreateText(savedialog.FileName)) {
                    sw.Write(this.txtLog.Text);
                }
            }
        }

        const int MAX_LOG_BUF = 1 << 17; // 128kB
        const int MIN_LOG_BUF = 1 << 11; // 2kB
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            // Sanity cleanup
            if (txtLog.TextLength > MAX_LOG_BUF) {
                txtLog.Text = txtLog.Text.Remove(0, txtLog.TextLength - MIN_LOG_BUF);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

