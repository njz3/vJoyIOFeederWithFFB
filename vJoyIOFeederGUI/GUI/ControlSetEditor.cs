using System;
using System.Collections;
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

        /// <summary>
        /// This class is an implementation of the 'IComparer' interface.
        /// </summary>
        public class ListViewColumnSorter :
            IComparer
        {
            /// <summary>
            /// Specifies the column to be sorted
            /// </summary>
            private int ColumnToSort;
            /// <summary>
            /// Specifies the order in which to sort (i.e. 'Ascending').
            /// </summary>
            private SortOrder OrderOfSort;
            /// <summary>
            /// Case insensitive comparer object
            /// </summary>
            private CaseInsensitiveComparer ObjectCompare;

            /// <summary>
            /// Class constructor.  Initializes various elements
            /// </summary>
            public ListViewColumnSorter()
            {
                // Initialize the column to '0'
                ColumnToSort = 0;

                // Initialize the sort order to 'none'
                OrderOfSort = SortOrder.None;

                // Initialize the CaseInsensitiveComparer object
                ObjectCompare = new CaseInsensitiveComparer();
            }

            /// <summary>
            /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
            /// </summary>
            /// <param name="x">First object to be compared</param>
            /// <param name="y">Second object to be compared</param>
            /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
            public int Compare(object x, object y)
            {
                int compareResult;
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Compare the two items
                compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

                // Calculate correct return value based on object comparison
                if (OrderOfSort == SortOrder.Ascending) {
                    // Ascending sort is selected, return normal result of compare operation
                    return compareResult;
                } else if (OrderOfSort == SortOrder.Descending) {
                    // Descending sort is selected, return negative result of compare operation
                    return (-compareResult);
                } else {
                    // Return '0' to indicate they are equal
                    return 0;
                }
            }

            /// <summary>
            /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
            /// </summary>
            public int SortColumn {
                set {
                    ColumnToSort = value;
                }
                get {
                    return ColumnToSort;
                }
            }

            /// <summary>
            /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
            /// </summary>
            public SortOrder Order {
                set {
                    OrderOfSort = value;
                }
                get {
                    return OrderOfSort;
                }
            }

        }

        private ListViewColumnSorter lvwColumnSorter;

        public ControlSetEditor()
        {
            InitializeComponent();

            lsvControlSets.Columns.Add("Unique name", 150, HorizontalAlignment.Left);
            lsvControlSets.Columns.Add("Game", 95, HorizontalAlignment.Left);
            lsvControlSets.AllowColumnReorder = true;
            lsvControlSets.FullRowSelect = true;
            lsvControlSets.View = View.Details;
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            lsvControlSets.ListViewItemSorter = lvwColumnSorter;


            foreach (ExecTypes exectype in Enum.GetValues(typeof(ExecTypes))) {
                cmbExecType.Items.Add(exectype.ToString());
            }
            cmbExecType.SelectedIndex = 0;

            foreach (OutputTypes outputtype in Enum.GetValues(typeof(OutputTypes))) {
                cmbOutputType.Items.Add(outputtype.ToString());
            }
            cmbOutputType.SelectedIndex = 0;

            RefreshListFromConfig();
        }

        private void RefreshListFromConfig()
        {
            lsvControlSets.Items.Clear();
            for (int i = 0; i<vJoyManager.Config.AllControlSets.ControlSets.Count; i++) {
                var cs = vJoyManager.Config.AllControlSets.ControlSets[i];
                ListViewItem it = new ListViewItem(cs.UniqueName);
                it.Name = cs.UniqueName;
                it.SubItems.Add(cs.GameName);
                lsvControlSets.Items.Add(it);
            }
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
            cs.UniqueName = "Default-" + (vJoyManager.Config.AllControlSets.ControlSets.Count+1);
            vJoyManager.Config.AllControlSets.ControlSets.Add(cs);
            RefreshListFromConfig();
            lsvControlSets.SelectedItems.Clear();
            SelectGivenUniqueName(cs.UniqueName);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lsvControlSets.SelectedItems.Count==1) {
                var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
                var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==name));
                vJoyManager.Config.AllControlSets.ControlSets.Remove(cs);
                RefreshListFromConfig();
                lsvControlSets.SelectedItems.Clear();
                if (vJoyManager.Config.AllControlSets.ControlSets.Count>0) {
                    SelectGivenUniqueName(vJoyManager.Config.AllControlSets.ControlSets[vJoyManager.Config.AllControlSets.ControlSets.Count-1].UniqueName);
                }
            }
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (lsvControlSets.SelectedItems.Count==1) {
                var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
                var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==name));
                var ncs = (ControlSetDB)cs.Clone();
                ncs.UniqueName = cs.UniqueName + "-" + (vJoyManager.Config.AllControlSets.ControlSets.Count+1);

                vJoyManager.Config.AllControlSets.ControlSets.Add(ncs);
                RefreshListFromConfig();
                lsvControlSets.SelectedItems.Clear();
                SelectGivenUniqueName(ncs.UniqueName);
            }
        }

        private void lsvControlSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvControlSets.SelectedItems.Count==1) {
                var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
                var cs = vJoyManager.Config.AllControlSets.ControlSets.Find(x => (x.UniqueName==name));
                this.txtControlSetUniqueName.Text = cs.UniqueName;
                this.txtExecProcessName.Text = cs.ProcessDescriptor.ProcessName;
                this.txtGameName.Text = cs.GameName;
                this.txtMainWindowTitle.Text = cs.ProcessDescriptor.MainWindowTitle;
                this.cmbExecType.SelectedItem = cs.ProcessDescriptor.ExecType.ToString();
                this.cmbOutputType.SelectedItem = cs.ProcessDescriptor.OutputType.ToString();
            }
        }


        private void cmbExecTypeOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return;

            Enum.TryParse<ExecTypes>(this.cmbExecType.Text, out css[0].ProcessDescriptor.ExecType);
        }
        private void cmbOutputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return;

            Enum.TryParse<OutputTypes>(this.cmbOutputType.Text, out css[0].ProcessDescriptor.OutputType);
        }


        private void Update_txtControlSetUniqueName()
        {
            // Make sure text boxnot empty
            if (txtControlSetUniqueName.Text.Replace(" ", "").Length==0)
                return;
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return;
            // Make sure we are not saving our own unique name
            if (css[0].UniqueName == txtControlSetUniqueName.Text)
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

            RefreshListFromConfig();

            SelectGivenUniqueName(newname);
        }

        private void Update_txtGameName()
        {
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return;

            css[0].GameName= txtGameName.Text;
            RefreshListFromConfig();
            SelectGivenUniqueName(css[0].UniqueName);
        }

        private void Update_txtExecProcessName()
        {
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return;

            css[0].ProcessDescriptor.ProcessName = txtExecProcessName.Text;
        }
        private void Update_txtMainWindowTitle()
        {
            // Check an item is selected
            if (lsvControlSets.SelectedItems.Count!=1)
                return;
            // Retrieve item and check only one exist
            var name = lsvControlSets.SelectedItems[0].SubItems[0].Text;
            var css = vJoyManager.Config.AllControlSets.ControlSets.FindAll(x => (x.UniqueName==name));
            if (css.Count!=1)
                return;

            css[0].ProcessDescriptor.MainWindowTitle = txtMainWindowTitle.Text;
        }

        private void txtControlSetUniqueName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            Update_txtControlSetUniqueName();
        }

        private void txtGameName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            Update_txtGameName();
        }

        private void txtExecProcessName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            Update_txtExecProcessName();
        }
       

        private void txtMainWindowTitle_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter))
                return;
            Update_txtMainWindowTitle();
        }


        private void btnCurrent_Click(object sender, EventArgs e)
        {
            SelectGivenUniqueName(vJoyManager.Config.CurrentControlSet.UniqueName);
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

        private void txtControlSetUniqueName_Leave(object sender, EventArgs e)
        {
            Update_txtControlSetUniqueName();
        }

        private void txtGameName_Leave(object sender, EventArgs e)
        {
            Update_txtGameName();
        }

        private void txtExecProcessName_Leave(object sender, EventArgs e)
        {
            Update_txtExecProcessName();
        }

        private void txtMainWindowTitle_Leave(object sender, EventArgs e)
        {
            Update_txtMainWindowTitle();
        }

        private void ControlSetEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Manager.SaveControlSetFiles();
        }


    }
}
