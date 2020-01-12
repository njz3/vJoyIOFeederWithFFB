using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyIOFeeder;

namespace IOBoardGUI
{
    public partial class Form1 : Form
    {
        ManagingThread Manager;
        public Form1()
        {
            InitializeComponent();

            Manager = new ManagingThread();
            Manager.StartManagingThread();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Manager.StopManagingThread();
        }
    }
}
