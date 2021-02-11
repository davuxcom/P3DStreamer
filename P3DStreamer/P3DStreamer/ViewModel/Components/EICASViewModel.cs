using P3DStreamer.DataModel;
using System;
using System.Diagnostics;

namespace P3DStreamer.ViewModel
{
    class EICASViewModel : BaseEICASViewModel 
    {

        // TODO:
        // var msgFuelImbal = new CautionMessageViewModel("FUEL IMBAL");
        //   xTrackError.IsApplicable = newData.sim_on_ground == 0 && Math.Abs(newData.gps_xtrack_meters) > appVm.XTRACK_DIST;
        //   xTrackError.Text = "XTRK ERR: " + Math.Abs(Math.Round(newData.gps_xtrack_meters / 1000, 1)) + " km";

        public EICASViewModel(AppViewModel appVm) : base(appVm)
        {
            


            appVm.DataArrived += (nd) => newData = nd;

            var noSimMsg = new WarningMessageViewModel("NO SIMCONNECT");
            noSimMsg.IsApplicable = true;
            appVm.Connected += (isConnected) => noSimMsg.IsApplicable = !isConnected;
            Messages.Add(noSimMsg);

            WithResolution(Define(EICASMessageKind.Warning, "ICING",
                () => newData.structural_ice_pct > 0,
                () => "ICING " + Math.Round(newData.structural_ice_pct, 1) + "%"),
                "ENABLE ANTI-ICE", SimEvents.ANTI_ICE_TOGGLE);

            Define(EICASMessageKind.Warning, "ENG 1 FUEL F", () => newData.eng1_on == 1 &&  newData.eng1_fuel_flow_pph == 0);
            Define(EICASMessageKind.Warning, "ENG 2 FUEL F", () => newData.eng2_on == 1 && newData.eng2_fuel_flow_pph == 0);
            var singleSourcePower = new WarningMessageViewModel("1 PWR SOURCE");
            Messages.Add(singleSourcePower);
            var onBatteryPower = new WarningMessageViewModel("ON BATTERY");
            Messages.Add(onBatteryPower);
            Define(EICASMessageKind.Warning, "PARKING BRAKE", () => newData.parking_brake == 1);
            Define(EICASMessageKind.Caution, "LOW FUEL", () => appVm.Info.FuelIsLow);

            Define(EICASMessageKind.Caution, "BATT 1 DSCHG", () => newData.elec_batt_load_1 > 0);
            Define(EICASMessageKind.Caution, "BATT 2 DSCHG", () => newData.elec_batt_load_2 > 0);
            Define(EICASMessageKind.Caution, "STBY BT DSCHG", () => newData.elec_batt_load_3 > 0);

            Define(EICASMessageKind.Caution, "ENG 1 GEN OFF", () => newData.eng1_on == 1 && newData.eng1_gen_on == 0);
            Define(EICASMessageKind.Caution, "ENG 2 GEN OFF", () => newData.eng2_on == 1 && newData.eng2_gen_on == 0);

            int LOW_VOLTS = 22; // 50%


            Define(EICASMessageKind.Caution, "GEN 1 LOW", () => newData.eng1_on == 1 && newData.eng1_gen_on == 1 && newData.eng1_gen_volts < LOW_VOLTS);
            Define(EICASMessageKind.Caution, "GEN 2 LOW", () => newData.eng2_on == 1 && newData.eng2_gen_on == 1 && newData.eng2_gen_volts < LOW_VOLTS);


            Define(EICASMessageKind.Caution, "APU GEN OFF", () => newData.apu_pct_rpm == 1 && newData.apu_gen_on == 0);
            Define(EICASMessageKind.Caution, "APU GEN LOW", 
                () => newData.apu_pct_rpm > 0 && newData.apu_gen_on == 1 && newData.apu_gen_volts < LOW_VOLTS,
                () => $"APU GEN {newData.apu_gen_volts}v");

            Define(EICASMessageKind.Caution, "BATT 1 OFF", () => newData.elec_batt1_on == 0);
            Define(EICASMessageKind.Caution, "BATT 2 OFF", () => Battery2ShouldBeOn() && newData.elec_batt2_on == 0);
            Define(EICASMessageKind.Caution, "STBY BATT OFF", () => HasStandbyBattery() && newData.elec_batt3_on == 0);

            Define(EICASMessageKind.Caution, "BATT 1 LOW", () => newData.elec_batt1_volts < LOW_VOLTS);
            Define(EICASMessageKind.Caution, "BATT 2 LOW", () => newData.elec_batt2_volts < LOW_VOLTS);
            Define(EICASMessageKind.Caution, "STBY BATT LOW", () => HasStandbyBattery() && newData.elec_batt3_volts < LOW_VOLTS);

            var ENG_RPM_LOW = _appVm.CurrentAircraft == AircraftKind.CitationLongitude ? 14000 : 7400;
            Define(EICASMessageKind.Caution, "ENG 1 LOW RPM", () => newData.eng1_on == 1 && newData.eng1_rpm < ENG_RPM_LOW);
            Define(EICASMessageKind.Caution, "ENG 2 LOW RPM", () => newData.eng2_on == 1 && newData.eng2_rpm < ENG_RPM_LOW);

            var HYD_PRESS_LOW = _appVm.CurrentAircraft == AircraftKind.CitationLongitude ? 2500 : 2400;
            Define(EICASMessageKind.Caution, "ENG 1 HYD PRES", () => newData.eng1_on == 1 && newData.eng1_hydraulic_press < HYD_PRESS_LOW);
            Define(EICASMessageKind.Caution, "ENG 2 HYD PRES", () => newData.eng2_on == 1 && newData.eng2_hydraulic_press < HYD_PRESS_LOW);

            var ENG_OIL_PRESS_LOW = 65;
            Define(EICASMessageKind.Caution, "ENG 1 OIL PSI", () => newData.eng1_on == 1 && newData.eng1_oil_press < ENG_OIL_PRESS_LOW);
            Define(EICASMessageKind.Caution, "ENG 2 OIL PSI", () => newData.eng2_on == 1 && newData.eng2_oil_press < ENG_OIL_PRESS_LOW);


            var msgFuelSelector = new CautionMessageViewModel("FUEL SEL");
            WithResolution(msgFuelSelector,
                "SET FUEL ALL", SimEvents.FUEL_SELECTOR_ALL);
            Messages.Add(msgFuelSelector);

            WithResolution(Define(EICASMessageKind.Caution, "PITOT HEAT OFF", () => newData.pitot_heat_enabled == 0),
                "TURN PITOT ON", SimEvents.PITOT_HEAT_ON);

            Define(EICASMessageKind.Caution, "NAV LTS OFF", () => newData.light_nav_on == 0);
            Define(EICASMessageKind.Caution, "BEACON OFF", () => (newData.eng1_on == 1 || newData.eng2_on == 1) && newData.light_beacon_on == 0);
            Define(EICASMessageKind.Caution, "STROBE OFF", () => newData.sim_on_ground == 0 && newData.light_strobe_on == 0);

            Define(EICASMessageKind.Caution, "LDG LTS <10K", () => newData.sim_on_ground == 0 && newData.indicated_alt < 10000 && newData.light_landing_on == 0);
            Define(EICASMessageKind.Caution, "LDG LTS >10K", () => newData.sim_on_ground == 0 && newData.indicated_alt > 10000 && newData.light_landing_on == 1);

            var spoilersExt5Min = Define(EICASMessageKind.Caution, "SPOILERS EXT");

            Define(EICASMessageKind.Advisory, "GEAR IN TRANSIT", () => newData.sim_on_ground == 0 && newData.gear_is_retractable == 1 && newData.gear_ext_pct > 0 && newData.gear_ext_pct < 1);
            Define(EICASMessageKind.Advisory, "GEAR EXT", () => newData.sim_on_ground == 0 && newData.gear_is_retractable == 1 && newData.gear_ext_pct == 1);
            Define(EICASMessageKind.Advisory, "CLOUD / ICE", () => newData.is_in_cloud == 1 && newData.outside_air_temp_c < Reality.TEMP_LOW);
            Define(EICASMessageKind.Advisory, "FLAPS", () => newData.flaps_position_idx > 0, () => "FLAP EXT: " + newData.flaps_position_idx);

            Define(EICASMessageKind.Advisory, "ICE ...", () => _appVm.Info.IceInfo.HasIce, () =>
            {
                switch (_appVm.Info.IceInfo.Status)
                {
                    case IceStatusKind.Accumulating: return "ICE ACCUM";
                    case IceStatusKind.Holding: return "ICE HOLDING";
                    case IceStatusKind.Shedding: return "DE-ICING " + _appVm.Info.IceTime.Value;
                    default: return "ICE ...";
                }
            });



            Define(EICASMessageKind.Advisory, "SPOILERS EXT", () => newData.is_spoilers_available == 1 && newData.spoilers_position > 50);
            Define(EICASMessageKind.Advisory, "APU RUNNING", () => newData.apu_pct_rpm == 1);

            Define(EICASMessageKind.Advisory, "EXT PWR AVAIL", () => newData.elec_ext_pwr_available ==1 && newData.elec_ext_pwr_on == 0);
            Define(EICASMessageKind.Advisory, "EXTERNAL PWR", () => newData.elec_ext_pwr_available ==1 && newData.elec_ext_pwr_on == 1);

            var spoilersExtStopwatch = Stopwatch.StartNew();
            bool spoilers_are_ext = false;
            appVm.DataArrived += (newData) =>
            {
                if (newData.is_spoilers_available == 1)
                {
                    if (newData.spoilers_position > 50)
                    {
                        if (!spoilers_are_ext)
                        {
                            spoilersExtStopwatch = Stopwatch.StartNew();
                        }

                        spoilers_are_ext = true;
                    }
                    else
                    {
                        spoilers_are_ext = false;
                    }
                }

                spoilersExt5Min.IsApplicable = (spoilers_are_ext && spoilersExtStopwatch.Elapsed.TotalMinutes > 5);
 
                int sourceCount = 0;
                int nonBatterySourceCount = 0;

                if (newData.elec_batt1_on == 1 && newData.elec_batt1_volts > LOW_VOLTS)
                {
                    sourceCount++;
                }
                if (newData.elec_batt2_on == 1 && newData.elec_batt2_volts > LOW_VOLTS)
                {
                    sourceCount++;
                }
                if (newData.elec_batt3_on == 1 && newData.elec_batt3_volts > LOW_VOLTS)
                {
                    sourceCount++;
                }

                if (newData.eng1_gen_on == 1 && newData.eng1_gen_volts > LOW_VOLTS)
                {
                    sourceCount++;
                    nonBatterySourceCount++;
                }
                if (newData.eng2_gen_on == 1 && newData.eng2_gen_volts > LOW_VOLTS)
                {
                    sourceCount++;
                    nonBatterySourceCount++;
                }
                if (newData.apu_gen_on == 1 && newData.apu_gen_volts > LOW_VOLTS)
                {
                    sourceCount++;
                    nonBatterySourceCount++;
                }

                if (newData.elec_ext_pwr_available == 1 && newData.elec_ext_pwr_on == 1)
                {
                    nonBatterySourceCount++;
                }

                singleSourcePower.IsApplicable = (sourceCount < 2);
                onBatteryPower.IsApplicable = (nonBatterySourceCount == 0);

                msgFuelSelector.IsApplicable = newData.fuel_tank_selector_1 != 1;
                var tank = "OFF";
                if (newData.fuel_tank_selector_1 > 0)
                {
                    tank = newData.fuel_tank_selector_1 == 2 ? "LEFT" : "RIGHT";
                }
                msgFuelSelector.Text = "FUEL SEL " + tank;

                if (appVm.CurrentAircraft == AircraftKind.Baron ||
                appVm.CurrentAircraft == AircraftKind.CitationLongitude) // any twin needs fuel sel 2 checked
                {
                    msgFuelSelector.IsApplicable = false;
                }

            };
        }

        bool HasStandbyBattery()
        {
            return _appVm.SystemsCfg?.HasBattery3 ?? false;
        }

        bool Battery2ShouldBeOn()
        {
            if (_appVm.CurrentAircraft == AircraftKind.DA40NG)
            {
                return false;
            }
            return true;
        }
    }
}
