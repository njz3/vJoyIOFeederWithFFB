using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    }
}
