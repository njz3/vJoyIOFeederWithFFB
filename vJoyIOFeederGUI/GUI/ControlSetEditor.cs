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
using vJoyIOFeeder.Configuration;

namespace vJoyIOFeederGUI.GUI
{
    public partial class ControlSetEditor : Form
    {
        public ControlSetEditor()
        {
            InitializeComponent();

            // Sort all items added previously.
            lstControlSets.Sorted = true;

            // Set the SelectionMode to select multiple items.
            lstControlSets.SelectionMode = SelectionMode.One;

            foreach (ExecTypes exectype in Enum.GetValues(typeof(ExecTypes))) {
                cmbExecTypeOutput.Items.Add(exectype.ToString());
            }
            cmbExecTypeOutput.SelectedIndex = 0;

            RefreshListFromConfig();
        }

        private void RefreshListFromConfig()
        {
            lstControlSets.Items.Clear();
            for (int i = 0; i<vJoyManager.Config.AllControlSets.ControlSets.Count; i++) {
                var cs = vJoyManager.Config.AllControlSets.ControlSets[i];
                lstControlSets.Items.Add(cs.UniqueName);
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ControlSetDB cs = new ControlSetDB();
            cs.UniqueName = "Default-" + (vJoyManager.Config.AllControlSets.ControlSets.Count+1);
            vJoyManager.Config.AllControlSets.ControlSets.Add(cs);
            RefreshListFromConfig();
            lstControlSets.SelectedItem = cs.UniqueName;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var item = lstControlSets.SelectedItem as string;
            if (item!= null) {
                var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==item));
                vJoyManager.Config.AllControlSets.ControlSets.Remove(cs);
                RefreshListFromConfig();
                lstControlSets.SelectedIndex = -1;
            }
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            var item = lstControlSets.SelectedItem as string;
            if (item!= null) {
                var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==item));
                ControlSetDB ncs = new ControlSetDB();
                ncs.UniqueName = cs.UniqueName + "-" + (vJoyManager.Config.AllControlSets.ControlSets.Count+1);

                vJoyManager.Config.AllControlSets.ControlSets.Add(ncs);
                RefreshListFromConfig();
                lstControlSets.SelectedItem = ncs.UniqueName;
            }
        }

        private void lstControlSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = lstControlSets.SelectedItem as string;
            if (item!= null) {
                var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==item));
                this.txtControlSetUniqueName.Text = cs.UniqueName;
                this.txtExecProcessName.Text = cs.ProcessDescriptor.ProcessName;
                this.txtGameName.Text = cs.GameName;
                this.txtMainWindowTitle.Text = cs.ProcessDescriptor.MainWindowTitle;
                this.cmbExecTypeOutput.SelectedItem = cs.ProcessDescriptor.ExecType;
            }
        }

        private void txtControlSetUniqueName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            // Make sure text boxnot empty
            if (txtControlSetUniqueName.Text.Replace(" ", "").Length==0)
                return;
            // Check an item is selected
            var item = lstControlSets.SelectedItem as string;
            if (item==null)
                return;
            // Retrieve item and check only one exist
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==item));
            if (css.Count!=1)
                return;
            // Look whether new name already exists
            var newname = txtControlSetUniqueName.Text;
            var nn = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==newname));
            if (nn.Count>0) {
                MessageBox.Show("Unique name" + newname, "Error in new name", MessageBoxButtons.OK);
                return;
            }
            // Now rename controlset
            css[0].UniqueName = newname;
        }

        private void txtGameName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            // Check an item is selected
            var item = lstControlSets.SelectedItem as string;
            if (item==null)
                return;
            // Retrieve item and check only one exist
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==item));
            if (css.Count!=1)
                return;

            css[0].GameName= txtGameName.Text;
        }

        private void cmbExecTypeOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check an item is selected
            var item = lstControlSets.SelectedItem as string;
            if (item==null)
                return;
            // Retrieve item and check only one exist
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==item));
            if (css.Count!=1)
                return;

            css[0].GameName= txtGameName.Text;
        }
    }
}
