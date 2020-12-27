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
    public partial class KeyEmulationEditor : Form
    {
        protected ControlSetDB EditedControlSet;

        private ListViewColumnSorter lvwColumnSorter;

        public KeyEmulationEditor(ControlSetDB controlSet)
        {
            EditedControlSet = controlSet;

            InitializeComponent();

            lsvKeyRulesSets.Columns.Add("Unique name", 100, HorizontalAlignment.Left);
            lsvKeyRulesSets.Columns.Add("Key", 80, HorizontalAlignment.Left);
            lsvKeyRulesSets.Columns.Add("Source", 96, HorizontalAlignment.Left);

            lsvKeyRulesSets.AllowColumnReorder = true;
            lsvKeyRulesSets.FullRowSelect = true;
            lsvKeyRulesSets.View = View.Details;
            // Create an instance of a ListView column sorter and assign it 
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            lsvKeyRulesSets.ListViewItemSorter = lvwColumnSorter;


            foreach (var exectype in Enum.GetValues(typeof(KeySourceTypes))) {
                cmbSourceType1.Items.Add(exectype.ToString());
                cmbSourceType2.Items.Add(exectype.ToString());
                cmbSourceType3.Items.Add(exectype.ToString());
            }
            cmbSourceType1.SelectedIndex = 0;
            cmbSourceType2.SelectedIndex = 0;
            cmbSourceType3.SelectedIndex = 0;


            foreach (var item in Enum.GetValues(typeof(KeyCodes))) {
                cmbKeyStroke.Items.Add(item.ToString());
            }
            cmbKeyStroke.SelectedIndex = 0;

            foreach (var item in Enum.GetValues(typeof(KeyEmulationAPI))) {
                cmbKeyAPI.Items.Add(item.ToString());
            }
            cmbKeyAPI.SelectedIndex = 0;

            foreach (var oper in Enum.GetValues(typeof(KeysOperators))) {
                cmbCombine1.Items.Add(oper.ToString());
                cmbCombine2.Items.Add(oper.ToString());
            }
            cmbCombine1.SelectedIndex = 0;
            cmbCombine2.SelectedIndex = 0;

            RefreshListFromRules();
        }
        private void ControlSetEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Manager.SaveControlSetFiles();
        }

        private void RefreshListFromRules()
        {
            lsvKeyRulesSets.Items.Clear();
            for (int i = 0; i<EditedControlSet.KeyRules.Count; i++) {
                var rule = EditedControlSet.KeyRules[i];
                ListViewItem it = new ListViewItem(rule.UniqueName);
                it.Name = rule.UniqueName;
                it.SubItems.Add(rule.KeyCode.ToString());
                it.SubItems.Add(rule.GetExpression());
                lsvKeyRulesSets.Items.Add(it);
            }
        }

        private void lsvKeyRules_ColumnClick(object sender, ColumnClickEventArgs e)
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
            this.lsvKeyRulesSets.Sort();
        }


        private void SelectFromUniqueName(string uniquename)
        {
            lsvKeyRulesSets.SelectedItems.Clear();
            for (int i = 0; i<lsvKeyRulesSets.Items.Count; i++) {
                var it = lsvKeyRulesSets.Items[i];
                if (it.Name == uniquename) {
                    it.Selected = true;
                    it.Focused = true;
                    lsvKeyRulesSets.Select();
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
            KeyStrokeDB rule = new KeyStrokeDB();
            rule.Initialize();
            rule.UniqueName = "Default-" + (EditedControlSet.KeyRules.Count+1);
            EditedControlSet.KeyRules.Add(rule);
            RefreshListFromRules();
            lsvKeyRulesSets.SelectedItems.Clear();
            SelectFromUniqueName(rule.UniqueName);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lsvKeyRulesSets.SelectedItems.Count==1) {
                var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
                var rule = EditedControlSet.KeyRules.Find(x => (x.UniqueName==name));
                EditedControlSet.KeyRules.Remove(rule);
                RefreshListFromRules();
                lsvKeyRulesSets.SelectedItems.Clear();
                if (EditedControlSet.KeyRules.Count>0) {
                    SelectFromUniqueName(EditedControlSet.KeyRules[EditedControlSet.KeyRules.Count-1].UniqueName);
                }
            }
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (lsvKeyRulesSets.SelectedItems.Count==1) {
                var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
                var rule = EditedControlSet.KeyRules.Find(x => (x.UniqueName==name));
                var newrule = (KeyStrokeDB)rule.Clone();
                newrule.UniqueName = rule.UniqueName + "-" + (EditedControlSet.KeyRules.Count+1);

                EditedControlSet.KeyRules.Add(newrule);
                RefreshListFromRules();
                lsvKeyRulesSets.SelectedItems.Clear();
                SelectFromUniqueName(newrule.UniqueName);
            }
        }
        private void btnCurrent_Click(object sender, EventArgs e)
        { SelectFromUniqueName(BFFManager.Config.CurrentControlSet.UniqueName); }

        private void btnValidate_Click(object sender, EventArgs e)
        {
        }

        private void _updateAllControlsFromRule(KeyStrokeDB rule)
        {
            // Clean some things that could lead to hazardous editing error.
            // Limit number of elements.
            rule.KeySources.RemoveRange(3, rule.KeySources.Count-3);
            rule.KeyCombineOperators.RemoveRange(2, rule.KeyCombineOperators.Count-2);

            // Now update
            this.txtKeyRuleName.Text = rule.UniqueName;

            this.cmbSourceType1.SelectedItem = rule.KeySources[0].Type.ToString();
            this.txtSourceIndex1.Text = rule.KeySources[0].Index.ToString();
            this.txtThreshold1.Text = rule.KeySources[0].Threshold.ToString("N3");
            this.chkSign1.Checked = rule.KeySources[0].Sign;

            this.cmbCombine1.SelectedItem = rule.KeyCombineOperators[0].ToString();

            this.cmbSourceType2.SelectedItem = rule.KeySources[1].Type.ToString();
            this.txtSourceIndex2.Text = rule.KeySources[1].Index.ToString();
            this.txtThreshold2.Text = rule.KeySources[1].Threshold.ToString("N3");
            this.chkSign2.Checked = rule.KeySources[1].Sign;

            this.cmbCombine2.SelectedItem = rule.KeyCombineOperators[1].ToString();

            this.cmbSourceType3.SelectedItem = rule.KeySources[2].Type.ToString();
            this.txtSourceIndex3.Text = rule.KeySources[2].Index.ToString();
            this.txtThreshold3.Text = rule.KeySources[2].Threshold.ToString("N3");
            this.chkSign2.Checked = rule.KeySources[2].Sign;

            this.txtHoldTimes_ms.Text = rule.HoldTime_ms.ToString();
            this.chkIsInversed.Checked = rule.IsInvertedLogic;
            this.chkTestValue.Checked = false;

            this.cmbKeyAPI.SelectedItem = rule.KeyAPI.ToString();
            this.cmbKeyStroke.SelectedItem = rule.KeyCode.ToString();

            this.txtExpr.Text = rule.GetExpression();
        }

        private void lsvControlSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsvKeyRulesSets.SelectedItems.Count==1) {
                var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
                var rule = EditedControlSet.KeyRules.Find(x => (x.UniqueName==name));
                if (this.txtKeyRuleName.Text != name) {
                    _updateAllControlsFromRule(rule);
                }
            }
        }

        private KeyStrokeDB _GetSelectedRule()
        {
            // Check an item is selected
            if (lsvKeyRulesSets.SelectedItems.Count!=1)
                return null;
            // Retrieve item and check only one exist
            var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
            var rule = EditedControlSet.KeyRules.FindAll(x => (x.UniqueName==name));
            if (rule.Count!=1)
                return null;
            return rule[0];
        }

        #region Unique name
        private void Update_txtRuleUniqueName()
        {
            // Check an item is selected
            var rule = _GetSelectedRule();
            if (rule==null) return;

            // Make sure text boxnot empty
            if (txtKeyRuleName.Text.Replace(" ", "").Length==0) {
                MessageBox.Show("Empty name no accepted", "Error in new name", MessageBoxButtons.OK);
                this.txtKeyRuleName.Focus();
                return;
            }
            // Make sure we are not saving our own unique name
            if (rule.UniqueName == txtKeyRuleName.Text)
                return;

            // Look whether new name already exists
            var newname = txtKeyRuleName.Text;
            var nn = EditedControlSet.KeyRules.FindAll(x => (x.UniqueName==newname));
            if (nn.Count>0) {
                MessageBox.Show("The unique name " + newname + " is already used, please use another name", "Error in new name", MessageBoxButtons.OK);
                this.txtKeyRuleName.Focus();
                return;
            }
            // Filter invalid char
            var notValid = string.IsNullOrEmpty(newname) || (newname.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0);
            if (notValid) {
                MessageBox.Show("Name cannot contain special characters", "Error in new name", MessageBoxButtons.OK);
                this.txtKeyRuleName.Focus();
                return;
            }

            // Now rename controlset
            rule.UniqueName = newname;
            RefreshListFromRules();
            SelectFromUniqueName(newname);
        }
        private void txtRuleUniqueName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Catch only Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtRuleUniqueName();
        }
        private void txtRuleUniqueName_Leave(object sender, EventArgs e)
        { Update_txtRuleUniqueName(); }

        #endregion

        #region All Controls

        #region Source type
        private void _UpdateSourceType(string sourcetypetxt, int source_index)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeySourceTypes>(sourcetypetxt, out rule.KeySources[source_index].Type);
            this.txtExpr.Text = rule.GetExpression();
            RefreshListFromRules();
            SelectFromUniqueName(rule.UniqueName);
        }

        private void cmbSourceType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _UpdateSourceType(this.cmbSourceType1.Text, 0);
        }
        private void cmbSourceType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            _UpdateSourceType(this.cmbSourceType2.Text, 1);
        }
        private void cmbSourceType3_SelectedIndexChanged(object sender, EventArgs e)
        {
            _UpdateSourceType(this.cmbSourceType3.Text, 2);
        }
        #endregion

        #region Source index
        private void UpdateSourceIndex(int source, string index)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            int.TryParse(index, out rule.KeySources[source].Index);
            this.txtExpr.Text = rule.GetExpression();
            RefreshListFromRules();
            SelectFromUniqueName(rule.UniqueName);
        }
        private void txtSourceIndex1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            UpdateSourceIndex(0, txtSourceIndex1.Text);
        }
        private void txtSourceIndex1_Leave(object sender, EventArgs e)
        { UpdateSourceIndex(0, txtSourceIndex1.Text); }
        private void txtSourceIndex2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            UpdateSourceIndex(1, txtSourceIndex2.Text);
        }
        private void txtSourceIndex2_Leave(object sender, EventArgs e)
        { UpdateSourceIndex(1, txtSourceIndex2.Text); }
        private void txtSourceIndex3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            UpdateSourceIndex(2, txtSourceIndex3.Text);
        }
        private void txtSourceIndex3_Leave(object sender, EventArgs e)
        { UpdateSourceIndex(2, txtSourceIndex3.Text); }
        #endregion

        #region Thresholds and sign
        private void UpdateThreshold(int source, string text)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            double.TryParse(text, out rule.KeySources[source].Threshold);
        }
        private void txtThreshold1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            UpdateThreshold(0, txtThreshold1.Text);
        }
        private void txtThreshold1_Leave(object sender, EventArgs e)
        { UpdateThreshold(0, txtThreshold1.Text); }
        private void txtThreshold2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            UpdateThreshold(1, txtThreshold2.Text);
        }
        private void txtThreshold2_Leave(object sender, EventArgs e)
        { UpdateThreshold(1, txtThreshold2.Text); }
        private void txtThreshold3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            UpdateThreshold(2, txtThreshold1.Text);
        }
        private void txtThreshold3_Leave(object sender, EventArgs e)
        { UpdateThreshold(2, txtThreshold1.Text); }

        private void chkSign1_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            rule.KeySources[0].Sign = chkSign1.Checked;
        }
        private void chkSign2_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            rule.KeySources[1].Sign = chkSign2.Checked;
        }
        private void chkSign3_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            rule.KeySources[2].Sign = chkSign3.Checked;
        }
        #endregion

        #region Combine operators
        private void updateCombine(int comb_index, string combinetxt)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeysOperators>(combinetxt, out var op);
            rule.KeyCombineOperators[comb_index] = op;
            this.txtExpr.Text = rule.GetExpression();
            RefreshListFromRules();
            SelectFromUniqueName(rule.UniqueName);
        }

        private void cmbCombine1_SelectedIndexChanged(object sender, EventArgs e)
        { updateCombine(0, this.cmbCombine1.Text); }
        private void cmbCombine2_SelectedIndexChanged(object sender, EventArgs e)
        { updateCombine(1, this.cmbCombine2.Text); }
        #endregion

        #region Hold, inverse and test
        private void Update_txtHoldTime_ms()
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            long.TryParse(txtHoldTimes_ms.Text, out rule.HoldTime_ms);
        }
        private void txtHoldTime_ms_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Filter Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtHoldTime_ms();
        }
        private void txtHoldTime_ms_Leave(object sender, EventArgs e)
        { Update_txtHoldTime_ms(); }

        private void chkIsInversed_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            rule.IsInvertedLogic = this.chkIsInversed.Checked;
        }

        private void chkTestValue_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
        }
        #endregion

        private void cmbKeyStroke_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeyCodes>(cmbKeyStroke.SelectedItem.ToString(), out rule.KeyCode);
            rule.MappedKeyStrokes.Clear();
            rule.MappedKeyStrokes.Add(rule.KeyCode);
            RefreshListFromRules();
            SelectFromUniqueName(rule.UniqueName);
        }

        private void cmbKeyAPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeyEmulationAPI>(cmbKeyAPI.SelectedItem.ToString(), out rule.KeyAPI);
            RefreshListFromRules();
            SelectFromUniqueName(rule.UniqueName);
        }

        #endregion

    }
}
