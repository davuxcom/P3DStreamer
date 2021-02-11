using P3DStreamer.DataModel;

namespace P3DStreamer.ViewModel
{

    class APModeControlPanelViewModel : BaseModeControlPanelViewModel
    {
        // "PLANE IN PARKING STATE"
        public MCPKnobViewModel VS_Sel { get; }
        public MCPKnobViewModel SPD_Sel { get; }
        public MCPKnobViewModel ALT_Sel { get; }
        public MCPKnobViewModel Baro_Sel { get; }

        public APModeControlPanelViewModel(AppViewModel appVm) : base(appVm)
        {

            VS_Sel = DefineKnob(SimEvents.AP_VS_VAR_INC, SimEvents.AP_VS_VAR_DEC, 1, 1);
            SPD_Sel = DefineKnob(SimEvents.AP_SPD_VAR_INC, SimEvents.AP_SPD_VAR_DEC, 1, 5);
            ALT_Sel = DefineKnob(SimEvents.AP_ALT_VAR_INC, SimEvents.AP_ALT_VAR_DEC, 100, 10);
            Baro_Sel = DefineKnob(SimEvents.KOHLSMAN_INC, SimEvents.KOHLSMAN_DEC, 1, 1);

            DefineMode("AP", SimEvents.AP_MASTER,
                () => newData.ap_master == 1,
                () => newData.ap_master == 0 && newData.sim_on_ground == 0);
            var AT_native = DefineMode("AT", SimEvents.AUTO_THROTTLE_ARM,
                () => newData.ap_at_is_active == 1,
                () => newData.ap_at_is_active == 0 && newData.sim_on_ground == 0);

            
            var AT_supplemental = new MCPModeViewModel { Text = "AT s", Invoke = new RelayCommand(() => appVm.AT.IsActive = !appVm.AT.IsActive) };
            AT_supplemental.WarningColor = System.Windows.Media.Colors.Yellow;
            appVm.DataArrived += (newData) =>
            {
                AT_supplemental.IsActive = appVm.AT.IsActive;
                AT_supplemental.IsWarning = appVm.AT.IsWarning;
                if (AT_supplemental.IsActive)
                {
                    AT_supplemental.Text = appVm.AT.DesiredSpeed.ToString();
                }
            };
            Modes.Add(AT_supplemental);
            
            DefineMode("FD", SimEvents.TOGGLE_FLIGHT_DIRECTOR,
                () => newData.ap_fd_active == 1);
            DefineMode("HDG", SimEvents.AP_HDG_HOLD_ON,
                () => newData.ap_mode_HDG == 1);
            DefineMode("LVL", SimEvents.AP_WING_LEVELER_ON,
                () => newData.ap_mode_lvl == 1);
            DefineMode("NAV", SimEvents.AP_NAV1_HOLD_ON,
                () => newData.ap_mode_NAV == 1);
            DefineMode("APPR", SimEvents.AP_APR_HOLD_ON,
                () => newData.ap_mode_APPR == 1);
            DefineMode("FLC", SimEvents.FLIGHT_LEVEL_CHANGE,
                () => newData.ap_mode_flc == 1);
            DefineMode("ALT", SimEvents.AP_ALT_HOLD_ON,
                () => newData.ap_mode_ALT == 1);
            DefineMode("VS", SimEvents.AP_VS_HOLD,
                () => newData.ap_mode_VS == 1);

            appVm.DataArrived += (nd) =>
            {
                AT_native.IsApplicable = appVm.SystemsCfg?.HasAutoThrottle ?? false;
                AT_supplemental.IsApplicable = !appVm.SystemsCfg?.HasAutoThrottle ?? false;
            };
        }
    }
}
