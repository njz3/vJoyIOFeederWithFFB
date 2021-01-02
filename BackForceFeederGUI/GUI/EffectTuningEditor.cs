using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BackForceFeeder;
using BackForceFeeder.BackForceFeeder;
using BackForceFeeder.Configuration;
using BackForceFeeder.FFBManagers;
using BackForceFeeder.Utils;

namespace BackForceFeederGUI.GUI
{

    public partial class EffectTuningEditor : Form
    {
        protected ControlSetDB EditedControlSet;

        /// <summary>
        /// Alias to current config
        /// </summary>
        public FFBParamsDB FFBParams { get { return EditedControlSet.FFBParams; } }

        public EffectTuningEditor(ControlSetDB controlSet)
        {
            EditedControlSet = controlSet;
            InitializeComponent();
        }


        private void EffectTuningEditor_Load(object sender, EventArgs e)
        {
            ToolTip tooltip = new ToolTip();
            RefreshValuesFromConfig();
        }

        private void EffectTuningEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            SharedData.Manager.SaveControlSetFiles();
        }

        private void SetTackbarValue(TrackBar tb, double value, double scale)
        {
            double safe = Math.Max(tb.Minimum, Math.Min(tb.Maximum, value/scale));
            int ival = (int)Math.Round(safe);
            tb.Value = ival;
        }
        private void SetTxtBoxValue(TextBox txt, double value)
        {
            txt.Text = value.ToString(CultureInfo.InvariantCulture);
        }
        private void SetTrackAndTxtBoxValues(TrackBar tb, TextBox txt, double value, double scale)
        {
            SetTackbarValue(tb, value, scale);
            SetTxtBoxValue(txt, value);
        }
        private void RefreshValuesFromConfig()
        {
            SetTrackAndTxtBoxValues(tbGlobalGain, txtGlobalGain, FFBParams.GlobalGain, 0.1);
            SetTrackAndTxtBoxValues(tbPowerLaw, txtPowerLaw, FFBParams.PowerLaw, 0.1);
            SetTrackAndTxtBoxValues(tbTrqDeadBand, txtTrqDeadBand, FFBParams.TrqDeadBand, 0.01);

            // Advanced tuning
            SetTrackAndTxtBoxValues(tbMinVelThreshold, txtMinVelThreshold, FFBParams.MinVelThreshold, 0.01);
            SetTrackAndTxtBoxValues(tbMinAccelThreshold, txtMinAccelThreshold, FFBParams.MinAccelThreshold, 0.01);

            SetTrackAndTxtBoxValues(tbSpring_Kp, txtSpring_Kp, FFBParams.Spring_Kp, 0.01);
            SetTrackAndTxtBoxValues(tbSpring_Bv, txtSpring_Bv, FFBParams.Spring_Bv, 0.01);
            SetTrackAndTxtBoxValues(tbSpring_TrqDeadband, txtSpring_TrqDeadband, FFBParams.Spring_TrqDeadband, 0.01);

            SetTrackAndTxtBoxValues(tbFriction_Bv, txtFriction_Bv, FFBParams.Friction_Bv, 0.01);
            SetTrackAndTxtBoxValues(tbMinDamperForActive, txtMinDamperForActive, FFBParams.MinDamperForActive, 0.01);
            SetTrackAndTxtBoxValues(tbPermanentSpring, txtPermanentSpring, FFBParams.PermanentSpring, 0.01);

            SetTrackAndTxtBoxValues(tbInertia_Bv, txtInertia_Bv, FFBParams.Inertia_Bv, 0.01);
            SetTrackAndTxtBoxValues(tbInertia_BvRaw, txtInertia_BvRaw, FFBParams.Inertia_BvRaw, 0.01);
            SetTrackAndTxtBoxValues(tbInertia_J, txtInertia_J, FFBParams.Inertia_J, 0.01);

            SetTrackAndTxtBoxValues(tbDamper_Bv, txtDamper_Bv, FFBParams.Damper_Bv, 0.01);
            SetTrackAndTxtBoxValues(tbDamper_J, txtDamper_J, FFBParams.Damper_J, 0.01);
        }

        private void RefreshEnabledEffectTunning()
        {
            bool enabled = false;
            if (chkAllowEffectTuning.Checked) {
                enabled = true;
            }
            txtMinVelThreshold.Enabled = enabled;
            txtMinAccelThreshold.Enabled = enabled;
            txtSpring_Kp.Enabled = enabled;
            txtSpring_Bv.Enabled = enabled;
            txtSpring_TrqDeadband.Enabled = enabled;
            txtFriction_Bv.Enabled = enabled;
            txtMinDamperForActive.Enabled = enabled;
            txtPermanentSpring.Enabled = enabled;

            txtInertia_Bv.Enabled = enabled;
            txtInertia_BvRaw.Enabled = enabled;
            txtInertia_J.Enabled = enabled;
            txtDamper_Bv.Enabled = enabled;
            txtDamper_J.Enabled = enabled;

            tbMinVelThreshold.Enabled = enabled;
            tbMinAccelThreshold.Enabled = enabled;
            tbSpring_Kp.Enabled = enabled;
            tbSpring_Bv.Enabled = enabled;
            tbSpring_TrqDeadband.Enabled = enabled;
            tbFriction_Bv.Enabled = enabled;
            tbMinDamperForActive.Enabled = enabled;
            tbPermanentSpring.Enabled = enabled;

            tbInertia_Bv.Enabled = enabled;
            tbInertia_BvRaw.Enabled = enabled;
            tbInertia_J.Enabled = enabled;
            tbDamper_Bv.Enabled = enabled;
            tbDamper_J.Enabled = enabled;
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            lbCurrentControlSet.Text = "Control set: " + EditedControlSet.UniqueName + " (" + EditedControlSet.GameName + ")";
            if (SharedData.Manager.FFB!=null) {
                chkSkipStopEffect.Checked = FFBParams.SkipStopEffect;
                chkEmulateMissing.Checked = FFBParams.UseTrqEmulationForMissing;
                chkPulsedTrq.Checked = FFBParams.UsePulseSeq;
                chkForceTorque.Checked = FFBParams.ForceTrqForAllCommands;
                chkAllowEffectTuning.Checked = FFBParams.AllowEffectsTuning;

                var ffbmodel3 = SharedData.Manager.FFB as FFBManagerModel3;
                if (ffbmodel3!=null) {
                    chkEmulateMissing.Enabled = true;
                    chkPulsedTrq.Enabled = true;
                    chkForceTorque.Enabled = true;
                } else {
                    chkEmulateMissing.Enabled = false;
                    chkPulsedTrq.Enabled = false;
                    chkForceTorque.Enabled = false;
                }

                RefreshEnabledEffectTunning();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Reset configuration\nAre you sure ?", "Reset configuration", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (res == DialogResult.OK) {
                /*
                if (Program.Manager.IsRunning) {
                    Program.Manager.Stop();
                }
                */
                try {
                    var oldffb = FFBParams;
                    var newffb = new FFBParamsDB();
                    EditedControlSet.FFBParams = newffb;
                } catch (Exception ex) {
                    Console.WriteLine("Uncatch exception " + ex.Message);
                }
            }
            RefreshValuesFromConfig();
        }


        #region Common force effect properties
        private void chkAllowEffectTuning_Click(object sender, EventArgs e)
        {
            FFBParams.AllowEffectsTuning = !FFBParams.AllowEffectsTuning;
        }
        private void chkSkipStopEffect_Click(object sender, EventArgs e)
        {
            FFBParams.SkipStopEffect = !FFBParams.SkipStopEffect;
        }

        private bool HandleTxtBoxKeyPress(TextBox txt, TrackBar tb, KeyPressEventArgs e, ref double value, double scale)
        {
            if (txt==null || tb==null) return false;
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return false;
            if (!double.TryParse(txt.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double content))
                return false;
            content = Math.Max(tb.Minimum*scale, Math.Min(tb.Maximum*scale, content));
            value = content;
            SetTrackAndTxtBoxValues(tb, txt, value, scale);
            return true;
        }
        private bool HandleTrackbarScroll(TextBox txt, TrackBar tb, ref double value, double scale)
        {
            if (txt==null || tb==null) return false;
            value = tb.Value*scale;
            SetTxtBoxValue(txt, value);
            return true;
        }

        private void txtGlobalGain_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtGlobalGain, tbGlobalGain, e, ref FFBParams.GlobalGain, 0.1);
        }
        private void tbGlobalGain_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtGlobalGain, tbGlobalGain, ref FFBParams.GlobalGain, 0.1);
        }

        private void txtPowerLaw_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtPowerLaw, tbPowerLaw, e, ref FFBParams.PowerLaw, 0.1);
        }
        private void tbPowerLaw_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtPowerLaw, tbPowerLaw, ref FFBParams.PowerLaw, 0.1);
        }

        private void txtTrqDeadBand_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtTrqDeadBand, tbTrqDeadBand, e, ref FFBParams.TrqDeadBand, 0.01);
        }
        private void tbTrqDeadBand_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtTrqDeadBand, tbTrqDeadBand, ref FFBParams.TrqDeadBand, 0.01);
        }

        #endregion

        #region Specific mode properties
        private void chkPulsedTrq_Click(object sender, EventArgs e)
        {
            FFBParams.UsePulseSeq = !FFBParams.UsePulseSeq;
        }

        private void chkEmulateMissing_Click(object sender, EventArgs e)
        {
            FFBParams.UseTrqEmulationForMissing = !FFBParams.UseTrqEmulationForMissing;
        }

        private void chkForceTorque_Click(object sender, EventArgs e)
        {
            FFBParams.ForceTrqForAllCommands = !FFBParams.ForceTrqForAllCommands;
        }

        #endregion

        #region Advanced tuning properties
        private void txtMinVelThreshold_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtMinVelThreshold, tbMinVelThreshold, e, ref FFBParams.MinVelThreshold, 0.01);
        }
        private void tbMinVelThreshold_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtMinVelThreshold, tbMinVelThreshold, ref FFBParams.MinVelThreshold, 0.01);
        }
        private void txtMinAccelThreshold_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtMinAccelThreshold, tbMinAccelThreshold, e, ref FFBParams.MinAccelThreshold, 0.01);
        }
        private void tbMinAccelThreshold_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtMinAccelThreshold, tbMinAccelThreshold, ref FFBParams.MinAccelThreshold, 0.01);
        }
        private void txtSpring_Kp_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtSpring_Kp, tbSpring_Kp, e, ref FFBParams.Spring_Kp, 0.01);
        }
        private void tbSpring_Kp_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtSpring_Kp, tbSpring_Kp, ref FFBParams.Spring_Kp, 0.01);
        }
        private void txtSpring_Bv_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtSpring_Bv, tbSpring_Bv, e, ref FFBParams.Spring_Bv, 0.01);
        }
        private void tbSpring_Bv_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtSpring_Bv, tbSpring_Bv, ref FFBParams.Spring_Bv, 0.01);
        }
        private void txtSpring_TrqDeadband_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtSpring_TrqDeadband, tbSpring_TrqDeadband, e, ref FFBParams.Spring_TrqDeadband, 0.01);
        }
        private void tbSpring_TrqDeadband_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtSpring_TrqDeadband, tbSpring_TrqDeadband, ref FFBParams.Spring_TrqDeadband, 0.01);
        }

        private void txtFriction_Bv_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtFriction_Bv, tbFriction_Bv, e, ref FFBParams.Friction_Bv, 0.01);
        }
        private void tbFriction_Bv_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtFriction_Bv, tbFriction_Bv, ref FFBParams.Friction_Bv, 0.01);
        }
        private void txtMinDamperForActive_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtMinDamperForActive, tbMinDamperForActive, e, ref FFBParams.MinDamperForActive, 0.01);
        }
        private void tbMinDamperForActive_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtMinDamperForActive, tbMinDamperForActive, ref FFBParams.MinDamperForActive, 0.01);
        }
        private void txtPermanentSpring_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtPermanentSpring, tbPermanentSpring, e, ref FFBParams.PermanentSpring, 0.01);
        }
        private void tbPermanentSpring_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtPermanentSpring, tbPermanentSpring, ref FFBParams.PermanentSpring, 0.01);
        }


        private void txtInertia_Bv_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtInertia_Bv, tbInertia_Bv, e, ref FFBParams.Inertia_Bv, 0.01);
        }
        private void tbInertia_Bv_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtInertia_Bv, tbInertia_Bv, ref FFBParams.Inertia_Bv, 0.01);
        }

        private void txtInertia_BvRaw_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtInertia_BvRaw, tbInertia_BvRaw, e, ref FFBParams.Inertia_BvRaw, 0.01);
        }
        private void tbInertia_BvRaw_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtInertia_BvRaw, tbInertia_BvRaw, ref FFBParams.Inertia_BvRaw, 0.01);
        }

        private void txtInertia_J_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtInertia_J, tbInertia_J, e, ref FFBParams.Inertia_J, 0.01);
        }
        private void tbInertia_J_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtInertia_J, tbInertia_J, ref FFBParams.Inertia_J, 0.01);
        }

        private void txtDamper_Bv_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtDamper_Bv, tbDamper_Bv, e, ref FFBParams.Damper_Bv, 0.01);
        }
        private void tbDamper_Bv_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtDamper_Bv, tbDamper_Bv, ref FFBParams.Damper_Bv, 0.01);
        }

        private void txtDamper_J_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleTxtBoxKeyPress(txtDamper_J, tbDamper_J, e, ref FFBParams.Damper_J, 0.01);
        }
        private void tbDamper_J_Scroll(object sender, EventArgs e)
        {
            HandleTrackbarScroll(txtDamper_J, tbDamper_J, ref FFBParams.Damper_J, 0.01);
        }
        #endregion
    }
}
