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

namespace IOFeederGUI.GUI
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
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


        public void Log(string text)
        {
            if (this.InvokeRequired) {
                this.BeginInvoke(new Logger.LogMethod(Log), new object[] { text });
            } else {
                var now = DateTime.Now;
                this.txtLog.AppendText(now.ToLongTimeString() + " | " + text + Environment.NewLine);
            }
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            // set the current caret position to the end
            txtLog.SelectionStart = txtLog.Text.Length;
            // scroll it automatically
            txtLog.ScrollToCaret();
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
                using(StreamWriter sw = File.CreateText(savedialog.FileName)) {
                    sw.Write(this.txtLog.Text);
                }
            }
        }
    }
}

