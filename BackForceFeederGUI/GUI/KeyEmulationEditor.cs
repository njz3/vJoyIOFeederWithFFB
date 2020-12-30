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
using BackForceFeeder.BackForceFeeder;

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
            lsvKeyRulesSets.Columns.Add("Source", 125, HorizontalAlignment.Left);

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
                cmbKeyStroke1.Items.Add(item.ToString());
                cmbKeyStroke2.Items.Add(item.ToString());
            }
            cmbKeyStroke1.SelectedIndex = 0;
            cmbKeyStroke2.SelectedIndex = 0;

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

            RefreshListFromKeyStrokeDBs();
        }
        private void ControlSetEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            SharedData.Manager.SaveControlSetFiles();
        }

        private void RefreshListFromKeyStrokeDBs()
        {
            lsvKeyRulesSets.Items.Clear();
            for (int i = 0; i<EditedControlSet.KeyStrokeDBs.Count; i++) {
                var rule = EditedControlSet.KeyStrokeDBs[i];
                ListViewItem it = new ListViewItem(rule.UniqueName);
                it.Name = rule.UniqueName;
                string keys = "";
                for (int j = 0; j<rule.CombinedKeyStrokes.Count; j++) {
                    // Stop if "none" is following
                    if (rule.CombinedKeyStrokes[j] == KeyCodes.None) {
                        break;
                    }
                    keys += rule.CombinedKeyStrokes[j].ToString();
                    if ((j+1)<rule.CombinedKeyStrokes.Count) {
                        if (rule.CombinedKeyStrokes[j+1] != KeyCodes.None) {
                            keys += " + ";
                        }
                    }
                }
                it.SubItems.Add(keys);
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

        bool NoSelectedIndexDueToOngoingRefresh = false;
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
            rule.UniqueName = "Default-" + (EditedControlSet.KeyStrokeDBs.Count+1);
            EditedControlSet.KeyStrokeDBs.Add(rule);
            RefreshListFromKeyStrokeDBs();
            lsvKeyRulesSets.SelectedItems.Clear();
            SelectFromUniqueName(rule.UniqueName);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lsvKeyRulesSets.SelectedItems.Count==1) {
                var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
                var rule = EditedControlSet.KeyStrokeDBs.Find(x => (x.UniqueName==name));
                EditedControlSet.KeyStrokeDBs.Remove(rule);
                RefreshListFromKeyStrokeDBs();
                lsvKeyRulesSets.SelectedItems.Clear();
                if (EditedControlSet.KeyStrokeDBs.Count>0) {
                    SelectFromUniqueName(EditedControlSet.KeyStrokeDBs[EditedControlSet.KeyStrokeDBs.Count-1].UniqueName);
                }
            }
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (lsvKeyRulesSets.SelectedItems.Count==1) {
                var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
                var rule = EditedControlSet.KeyStrokeDBs.Find(x => (x.UniqueName==name));
                var newrule = (KeyStrokeDB)rule.Clone();
                newrule.UniqueName = rule.UniqueName + "-" + (EditedControlSet.KeyStrokeDBs.Count+1);

                EditedControlSet.KeyStrokeDBs.Add(newrule);
                RefreshListFromKeyStrokeDBs();
                lsvKeyRulesSets.SelectedItems.Clear();
                SelectFromUniqueName(newrule.UniqueName);
            }
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
        }

        private void _updateAllControlsFromRule(KeyStrokeDB rule)
        {
            // Cancel selectindex events because we are refreshing
            NoSelectedIndexDueToOngoingRefresh = true;

            // Clean some things that could lead to hazardous editing error.
            // Limit number of elements.
            rule.KeySources.RemoveRange(3, rule.KeySources.Count-3);
            rule.KeySourcesOperators.RemoveRange(2, rule.KeySourcesOperators.Count-2);

            // Now update
            this.txtKeyRuleName.Text = rule.UniqueName;

            this.cmbSourceType1.SelectedItem = rule.KeySources[0].Type.ToString();
            this.txtSourceIndex1.Text = rule.KeySources[0].Index.ToString();
            this.txtThreshold1.Text = rule.KeySources[0].Threshold.ToString("N3");
            this.chkSign1.Checked = rule.KeySources[0].InvSign;

            this.cmbCombine1.SelectedItem = rule.KeySourcesOperators[0].ToString();

            this.cmbSourceType2.SelectedItem = rule.KeySources[1].Type.ToString();
            this.txtSourceIndex2.Text = rule.KeySources[1].Index.ToString();
            this.txtThreshold2.Text = rule.KeySources[1].Threshold.ToString("N3");
            this.chkSign2.Checked = rule.KeySources[1].InvSign;

            this.cmbCombine2.SelectedItem = rule.KeySourcesOperators[1].ToString();

            this.cmbSourceType3.SelectedItem = rule.KeySources[2].Type.ToString();
            this.txtSourceIndex3.Text = rule.KeySources[2].Index.ToString();
            this.txtThreshold3.Text = rule.KeySources[2].Threshold.ToString("N3");
            this.chkSign2.Checked = rule.KeySources[2].InvSign;

            this.txtAxisTolerance_pct.Text = rule.AxisTolerance_pct.ToString("N3");
            this.txtHoldTimes_ms.Text = rule.HoldTime_ms.ToString();
            this.chkIsInversed.Checked = rule.IsInvertedLogic;
            this.chkTestValue.Checked = false;

            this.cmbKeyAPI.SelectedItem = rule.KeyAPI.ToString();
            if (rule.CombinedKeyStrokes.Count<2) {
                for (int i = rule.CombinedKeyStrokes.Count; i<2; i++) {
                    rule.CombinedKeyStrokes.Add(new KeyCodes());
                }
            }
            this.cmbKeyStroke1.SelectedItem = rule.CombinedKeyStrokes[0].ToString();
            this.cmbKeyStroke2.SelectedItem = rule.CombinedKeyStrokes[1].ToString();

            this.txtExpr.Text = rule.GetExpression();

            NoSelectedIndexDueToOngoingRefresh = false;
        }

        private void lsvControlSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Avoid recursive calls
            if (NoSelectedIndexDueToOngoingRefresh)
                return;

            if (lsvKeyRulesSets.SelectedItems.Count==1) {
                var name = lsvKeyRulesSets.SelectedItems[0].SubItems[0].Text;
                var rule = EditedControlSet.KeyStrokeDBs.Find(x => (x.UniqueName==name));
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
            var rule = EditedControlSet.KeyStrokeDBs.FindAll(x => (x.UniqueName==name));
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
            var nn = EditedControlSet.KeyStrokeDBs.FindAll(x => (x.UniqueName==newname));
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
            RefreshListFromKeyStrokeDBs();
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
            // Avoid recursive calls
            if (NoSelectedIndexDueToOngoingRefresh)
                return;
            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeySourceTypes>(sourcetypetxt, out rule.KeySources[source_index].Type);
            this.txtExpr.Text = rule.GetExpression();
            RefreshListFromKeyStrokeDBs();
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
            RefreshListFromKeyStrokeDBs();
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
            rule.KeySources[0].InvSign = chkSign1.Checked;
        }
        private void chkSign2_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            rule.KeySources[1].InvSign = chkSign2.Checked;
        }
        private void chkSign3_Click(object sender, EventArgs e)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            rule.KeySources[2].InvSign = chkSign3.Checked;
        }
        #endregion

        #region Combine operators
        private void updateCombine(int comb_index, string combinetxt)
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeysOperators>(combinetxt, out var op);
            rule.KeySourcesOperators[comb_index] = op;
            this.txtExpr.Text = rule.GetExpression();
            RefreshListFromKeyStrokeDBs();
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
        private void txtHoldTimes_ms_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Filter Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtHoldTime_ms();
        }
        private void txtHoldTimes_ms_Leave(object sender, EventArgs e)
        { Update_txtHoldTime_ms(); }

        private void Update_txtAxisTolerance_pct()
        {
            var rule = _GetSelectedRule();
            if (rule==null) return;
            double.TryParse(txtAxisTolerance_pct.Text, out rule.AxisTolerance_pct);
            if (rule.AxisTolerance_pct<0.02)
                rule.AxisTolerance_pct = 0.02;
            if (rule.AxisTolerance_pct>0.5)
                rule.AxisTolerance_pct = 0.5;
        }
        private void txtAxisTolerance_pct_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Filter Enter
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            Update_txtAxisTolerance_pct();
        }

        private void txtAxisTolerance_pct_Leave(object sender, EventArgs e)
        { Update_txtAxisTolerance_pct(); }


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

        private void cmbKeyStroke1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Avoid recursive calls
            if (NoSelectedIndexDueToOngoingRefresh)
                return;

            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeyCodes>(cmbKeyStroke1.SelectedItem.ToString(), out var keyCode1);
            Enum.TryParse<KeyCodes>(cmbKeyStroke2.SelectedItem.ToString(), out var keyCode2);
            rule.CombinedKeyStrokes.Clear();
            rule.CombinedKeyStrokes.Add(keyCode1);
            rule.CombinedKeyStrokes.Add(keyCode2);
            RefreshListFromKeyStrokeDBs();
            SelectFromUniqueName(rule.UniqueName);
        }
        private void cmbKeyStroke2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Avoid recursive calls
            if (NoSelectedIndexDueToOngoingRefresh)
                return;

            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeyCodes>(cmbKeyStroke1.SelectedItem.ToString(), out var keyCode1);
            Enum.TryParse<KeyCodes>(cmbKeyStroke2.SelectedItem.ToString(), out var keyCode2);
            rule.CombinedKeyStrokes.Clear();
            rule.CombinedKeyStrokes.Add(keyCode1);
            rule.CombinedKeyStrokes.Add(keyCode2);
            RefreshListFromKeyStrokeDBs();
            SelectFromUniqueName(rule.UniqueName);
        }

        private void cmbKeyAPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Avoid recursive calls
            if (NoSelectedIndexDueToOngoingRefresh)
                return;

            var rule = _GetSelectedRule();
            if (rule==null) return;
            Enum.TryParse<KeyEmulationAPI>(cmbKeyAPI.SelectedItem.ToString(), out rule.KeyAPI);
            RefreshListFromKeyStrokeDBs();
            SelectFromUniqueName(rule.UniqueName);
        }


        #endregion


    }
}
