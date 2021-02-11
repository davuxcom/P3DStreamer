using P3DStreamer.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace P3DStreamer.ViewModel
{
    class SystemModeControlPanelViewModel : BaseModeControlPanelViewModel
    {
        public SystemModeControlPanelViewModel(AppViewModel appVm) : base(appVm)
        {
            var B1 = DefineMode("BATT 1", SimEvents.TOGGLE_MASTER_BATTERY,
                () => newData.elec_batt1_on == 1,
                () => newData.elec_batt_load_1 > 0, 1);

            var B2 = DefineMode("STBY", SimEvents.TOGGLE_MASTER_BATTERY,
                () => newData.elec_batt3_on == 1,
                () => newData.elec_batt_load_3 > 0, 3);

            var B3 = DefineMode("BATT 2", SimEvents.TOGGLE_MASTER_BATTERY,
                () => newData.elec_batt2_on == 1,
                () => newData.elec_batt_load_2 > 0, 2);

            B1.WarningColor = B2.WarningColor = B3.WarningColor = Colors.Yellow;

            AddPlaceholder();

            var ENG_MIN_RPM = 5;

            DefineMode("GEN", SimEvents.TOGGLE_ALTERNATOR1,
                () => newData.eng1_gen_on == 1,
                () => newData.eng1_gen_on == 0 && newData.eng1_rpm > ENG_MIN_RPM);

            DefineMode("GEN", SimEvents.APU_GENERATOR_SWITCH_TOGGLE,
                () => newData.apu_gen_on == 1,
                () => newData.apu_gen_on == 0 && newData.apu_pct_rpm == 1);

            DefineMode("GEN", SimEvents.TOGGLE_ALTERNATOR2,
                () => newData.eng2_gen_on == 1,
                () => newData.eng2_gen_on == 0 && newData.eng2_rpm > ENG_MIN_RPM);
            DefineMode("GR WRN", SimEvents.ELECTRICAL_CIRCUIT_TOGGLE,
          () => newData.elec_circuit_31_on == 1, null, 31);

            // AddPlaceholder();

            DefineMode("EXT PWR", SimEvents.TOGGLE_EXTERNAL_POWER,
                 () => newData.elec_ext_pwr_available == 1 && newData.elec_ext_pwr_on == 1,
                 () => newData.elec_ext_pwr_available == 1 && newData.elec_ext_pwr_on == 0);

            AddPlaceholder();
            AddPlaceholder();

            DefineMode("1", () =>
            {
                var start = newData.eng1_on == 0;
                if (start != (newData.eng1_fuel_valve_on == 1))
                {
                    _appVm.DoCommand(SimEvents.TOGGLE_FUEL_VALVE_ENG1);
                }
                _appVm.DoCommand(start ? SimEvents.MIXTURE1_RICH : SimEvents.MIXTURE1_LEAN);
                _appVm.DoCommand(SimEvents.TURBINE_IGNITION_SWITCH_SET1, (uint)(start ? 1 : 0));
                if (start && (newData.eng1_starter_on == 0))
                {
                    _appVm.DoCommand(SimEvents.TOGGLE_STARTER1);
                }
            }, () => newData.eng1_on == 1,
            () => newData.eng1_starter_on == 1);

            DefineMode("APU", () =>
            {
                _appVm.DoCommand(newData.apu_pct_rpm > 0 ? SimEvents.APU_OFF_SWITCH : SimEvents.APU_STARTER);
            }, () => newData.apu_pct_rpm == 1, () => newData.apu_pct_rpm > 0 && newData.apu_pct_rpm < 1);

            DefineMode("2", () =>
            {
                var start = newData.eng2_on == 0;
                if (start != (newData.eng2_fuel_valve_on == 1))
                {
                    _appVm.DoCommand(SimEvents.TOGGLE_FUEL_VALVE_ENG2);
                }
                _appVm.DoCommand(start ? SimEvents.MIXTURE2_RICH : SimEvents.MIXTURE2_LEAN);
                _appVm.DoCommand(SimEvents.TURBINE_IGNITION_SWITCH_SET2, (uint)(start ? 1 : 0));
                if (start && (newData.eng2_starter_on == 0))
                {
                    _appVm.DoCommand(SimEvents.TOGGLE_STARTER2);
                }
            }, () => newData.eng2_on == 1,
            () => newData.eng2_starter_on == 1);

            // DefineMode("WNG DEICE", SimEvents.TOGGLE_STRUCTURAL_DEICE, () => newData.deice_structural_on == 1);
            // DefineMode("WING", SimEvents.TOGGLE_WING_LIGHTS, () => newData.light_wing_on == 1);
            // DefineMode("PANEL", SimEvents.PANEL_LIGHTS_TOGGLE, () => newData.light_panel_on == 1);
            // DefineMode("CABIN", SimEvents.TOGGLE_CABIN_LIGHTS, () => newData.light_cabin_on == 1);
            // DefineMode("RECOG", SimEvents.TOGGLE_RECOGNITION_LIGHTS, () => newData.light_recog_on == 1);
        }
    }
}
