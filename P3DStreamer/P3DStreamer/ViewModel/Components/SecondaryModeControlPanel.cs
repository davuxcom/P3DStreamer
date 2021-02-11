using P3DStreamer.DataModel;
using System.Windows.Media;

namespace P3DStreamer.ViewModel
{
    class SecondaryModeControlPanel : BaseModeControlPanelViewModel
    {
        public SecondaryModeControlPanel(AppViewModel appVm) : base(appVm)
        {
            DefineMode("LDG", SimEvents.LANDING_LIGHTS_TOGGLE, () => newData.light_landing_on == 1, null, 0);
            DefineMode("TAXI", SimEvents.TOGGLE_TAXI_LIGHTS, () => newData.light_taxi_on == 1);
            DefineMode("BEACON", SimEvents.TOGGLE_BEACON_LIGHTS, () => newData.light_beacon_on == 1);
            DefineMode("STROBE", SimEvents.STROBES_TOGGLE, () => newData.light_strobe_on == 1);

            var ice = DefineMode("ANTI-ICE", SimEvents.ANTI_ICE_TOGGLE, () => newData.eng1_antiice_on == 1, () => _appVm.Info.IceInfo.HasIce);
            ice.WarningColor = Colors.Red;

            var SPLR = DefineMode("SPOILER", SimEvents.SPOILERS_ARM_TOGGLE, () => newData.spoilers_is_armed == 1);

            var pitot = DefineMode("PITOT HT", SimEvents.PITOT_HEAT_TOGGLE,
                () => newData.pitot_heat_enabled == 1,
                () => newData.pitot_heat_enabled == 0 && newData.outside_air_temp_c < Reality.TEMP_LOW);
            pitot.WarningColor = Colors.Yellow;

            DefineMode("NAV", SimEvents.TOGGLE_NAV_LIGHTS, () => newData.light_nav_on == 1);

            _appVm.DataArrived += (nd) =>
            {
                SPLR.IsApplicable = _appVm.FlightModelCfg?.HasAutoSpoilers ?? false;
            };
        }
    }
}
