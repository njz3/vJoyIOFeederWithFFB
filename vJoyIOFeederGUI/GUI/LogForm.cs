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

        const int MAX_LOG_BUF = 1 << 17; // 128kB
        const int MIN_LOG_BUF = 1 << 11; // 2kB
        StringBuilder savedLog = new StringBuilder(MAX_LOG_BUF);
        bool newText = false;
        public void Log(string text)
        {
            lock (savedLog) {
                // Sanity cleanup if only 2k left in buffer
                if (savedLog.Length > (MAX_LOG_BUF - MIN_LOG_BUF)) {
                    savedLog.Remove(0, savedLog.Length - MIN_LOG_BUF);
                }
                savedLog.Append(DateTime.Now.ToLongTimeString());
                savedLog.Append(" | ");
                savedLog.AppendLine(text);
            }
            newText = true;
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
            this.savedLog.Clear();
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

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (!newText)
                return;
            newText = false;
            lock (savedLog) {
                this.txtLog.Text = savedLog.ToString();
            }
        }
    }
}

