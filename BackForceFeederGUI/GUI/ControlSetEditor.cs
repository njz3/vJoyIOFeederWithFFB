using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.Managers;

namespace BackForceFeederGUI.GUI
{
    public partial class ControlSetEditor : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        public ControlSetEditor()
        {
            InitializeComponent();

            lsvControlSets.Columns.Add("Unique name", 230, HorizontalAlignment.Left);
            lsvControlSets.Columns.Add("Game", 135, HorizontalAlignment.Left);

            lsvControlSets.AllowColumnReorder = true;
            lsvControlSets.FullRowSelect = true;
            lsvControlSets.View = View.Details;
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            lsvControlSets.ListViewItemSorter = lvwColumnSorter;


            foreach (var exectype in Enum.GetValues(typeof(ExecTypes))) {
                cmbExecType.Items.Add(exectype.ToString());
            }
            cmbExecType.SelectedIndex = 0;

            foreach (var outputtype in Enum.GetValues(typeof(OutputTypes))) {
                cmbOutputType.Items.Add(outputtype.ToString());
            }
            cmbOutputType.SelectedIndex = 0;

            foreach (var outputtype in Enum.GetValues(typeof(PriorityLevels))) {
                cmbPriorityLevel.Items.Add(outputtype.ToString());
            }
            cmbPriorityLevel.SelectedIndex = 0;

            RefreshListFromConfig();
        }

        private void ControlSetEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Manager.SaveControlSetFiles();
        }

        private void RefreshListFromConfig()
        {
            lsvControlSets.Items.Clear();
            for (int i = 0; i<BFFManager.Config.AllControlSets.ControlSets.Count; i++) {
                var cs = BFFManager.Config.AllControlSets.ControlSets[i];
                ListViewItem it = new ListViewItem(cs.UniqueName);
                it.Name = cs.UniqueName;
                it.SubItems.Add(cs.GameName);
                lsvControlSets.Items.Add(it);
            }
        }
        private void lsvControlSets_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn) {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending) {
                    lvwColumnSorter.Order = SortOrder.Descending;
                } else {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            } else {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.lsvControlSets.Sort();
        }

        private void SelectGivenUniqueName(string uniquename)
        {
            lsvControlSets.SelectedItems.Clear();
            for (int i = 0; i<lsvControlSets.Items.Count; i++) {
                var it = lsvControlSets.Items[i];
                if (it.Name == uniquename) {
                    it.Selected = true;
                    it.Focused = true;
                    lsvControlSets.Select();
                    it.EnsureVisible();
                    break;
                }
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
            cs.UniqueName = "Default-" + (BFFManager.Config.AllControlSets.ControlSets.Count+1);
            BFFManager.Config.AllControlSets.ControlSets.Add(cs);
            RefreshListFromConfig();
            lsvControlSets.SelectedItems.Clear();
            SelectGivenUniqueName(cs.UniqueName);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lsvControlSets.SelectedItems.Count==1) {
                var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
                var cs = BFFManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==name));
                BFFManager.Config.AllControlSets.ControlSets.Remove(cs);
                RefreshListFromConfig();
                lsvControlSets.SelectedItems.Clear();
                if (BFFManager.Config.AllControlSets.ControlSets.Count>0) {
                    SelectGivenUniqueName(BFFManager.Config.AllControlSets.ControlSets[BFFManager.Config.AllControlSets.ControlSets.Count-1].UniqueName);
                }
            }
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (lsvControlSets.SelectedItems.Count==1) {
                var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
                var cs = BFFManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==name));
                var ncs = (ControlSetDB)cs.Clone();
                ncs.UniqueName = cs.UniqueName + "-" + (BFFManager.Config.AllControlSets.ControlSets.Count+1);

                BFFManager.Config.AllControlSets.ControlSets.Add(ncs);
                RefreshListFromConfig();
                lsvControlSets.SelectedItems.Clear();
                SelectGivenUniqueName(ncs.UniqueName);
            }
        }
        private void btnCurrent_Click(object sender, EventArgs e)
        { SelectGivenUniqueName(BFFManager.Config.CurrentControlSet.UniqueName); }
        private void _updateAllControlsFromControlSet(ControlSetDB cs)
        {
            this.txtControlSetUniqueName.Text = cs.UniqueName;

            this.txtExecProcessName.Text = cs.ProcessDescriptor.ProcessName;
            this.txtGameName.Text = cs.GameName;
            this.cmbPriorityLevel.SelectedItem = cs.PriorityLevel.ToString();
            this.txtMainWindowTitle.Text = cs.ProcessDescriptor.MainWindowTitle;
            this.cmbExecType.SelectedItem = cs.ProcessDescriptor.ExecType.ToString();
            this.cmbOutputType.SelectedItem = cs.ProcessDescriptor.OutputType.ToString();
        }

        private void lsvControlSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvControlSets.SelectedItems.Count==1) {
                var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
                var cs = BFFManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==name));
                _updateAllControlsFromControlSet(cs);
            }
        }

        private ControlSetDB _GetSelectedControlSet()
        {
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return null;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = BFFManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return null;
            return css[0];
        }

        #region Unique name
        private void Update_txtControlSetUniqueName()
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;

            // Make sure text boxnot empty
            if (txtControlSetUniqueName.Text.Replace(" ", "").Length==0) {
                MessageBox.Show("Empty name no accepted", "Error in new name", MessageBoxButtons.OK);
                this.txtControlSetUniqueName.Focus();
                return;
            }
            // Make sure we are not saving our own unique name
            if (css.UniqueName == txtControlSetUniqueName.Text)
                return;
            // Look whether new name already exists
            var newname = txtControlSetUniqueName.Text;
            var nn = BFFManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==newname));
            if (nn.Count>0) {
                MessageBox.Show("The unique name " + newname + " is already used, please use another name", "Error in new name", MessageBoxButtons.OK);
                this.txtControlSetUniqueName.Focus();
                return;
            }
            // Filter invalid char
            var notValid = string.IsNullOrEmpty(newname) || (newname.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
            if (notValid) {
                MessageBox.Show("Name cannot contain special characters", "Error in new name", MessageBoxButtons.OK);
                this.txtControlSetUniqueName.Focus();
                return;
            }

            // Now rename controlset
            css.UniqueName = newname;
            RefreshListFromConfig();
            SelectGivenUniqueName(newname);
        }

        private void txtControlSetUniqueName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtControlSetUniqueName();
        }
        private void txtControlSetUniqueName_Leave(object sender, EventArgs e)
        { Update_txtControlSetUniqueName(); }

        #endregion

        #region All Controls
        private void cmbExecTypeOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;
            Enum.TryParse<ExecTypes>(this.cmbExecType.Text, out css.ProcessDescriptor.ExecType);
        }
        private void cmbOutputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;
            Enum.TryParse<OutputTypes>(this.cmbOutputType.Text, out css.ProcessDescriptor.OutputType);
        }
        private void cmbPriorityLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;
            Enum.TryParse<PriorityLevels>(this.cmbPriorityLevel.Text, out css.PriorityLevel);
        }

        private void Update_txtGameName()
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;

            css.GameName= txtGameName.Text;
            RefreshListFromConfig();
            SelectGivenUniqueName(css.UniqueName);
        }

        private void Update_txtExecProcessName()
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;
            css.ProcessDescriptor.ProcessName = txtExecProcessName.Text;
        }
        private void Update_txtMainWindowTitle()
        {
            var css = _GetSelectedControlSet();
            if (css==null) return;
            css.ProcessDescriptor.MainWindowTitle = txtMainWindowTitle.Text;
        }



        private void txtGameName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtGameName();
        }
        private void txtGameName_Leave(object sender, EventArgs e)
        { Update_txtGameName(); }


        private void txtExecProcessName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtExecProcessName();
        }
        private void txtExecProcessName_Leave(object sender, EventArgs e)
        { Update_txtExecProcessName(); }


        private void txtMainWindowTitle_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtMainWindowTitle();
        }
        private void txtMainWindowTitle_Leave(object sender, EventArgs e)
        { Update_txtMainWindowTitle(); }

        #endregion

    }
}
