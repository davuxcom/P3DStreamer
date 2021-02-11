using P3DStreamer.DataModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace P3DStreamer.ViewModel
{
    class BatteryInfoViewModel : CrewInfoDataViewModel
    {

        public BatteryInfoViewModel(string name) : base(name, "")
        {

        }
    }

    class FlightInfoViewModel : BaseComponentViewModel
    {
        public bool FuelIsLow { get => Get<bool>(); set => Set(value); }
        public bool IsOnGround { get => Get<bool>(); set => Set(value); }

        public CrewInfoDataViewModel OAT { get; }
        public CrewInfoDataViewModel TAS { get; }
        public CrewInfoDataViewModel GS { get; }
        public CrewInfoDataViewModel G { get; }
        public CrewInfoDataViewModel FuelQty { get; }
        public CrewInfoDataViewModel Endurance { get; }
        public CrewInfoDataViewModel Range { get; }
        public CrewInfoDataViewModel FuelBurn { get; }
        public CrewInfoDataViewModel Destination_ETE { get; }
        public CrewInfoDataViewModel FlightOrGroundTimer { get; }
        public CrewInfoDataViewModel WP_DTG { get; }
        public CrewInfoDataViewModel WP_ETE { get; }
        public CrewInfoDataViewModel ElevatorTrim { get; }
        public CrewInfoDataViewModel RA { get; }
        public CrewInfoDataViewModel GPS_XTK { get; }
        public IceTrackerViewModel IceInfo { get; }
        public CrewInfoDataViewModel Batt1 { get; }
        public CrewInfoDataViewModel Batt1Time { get; }
        public CrewInfoDataViewModel Batt2 { get; }
        public CrewInfoDataViewModel Batt2Time { get; }
        public CrewInfoDataViewModel BattStby { get; }
        public CrewInfoDataViewModel BattStbyTime { get; }
        public CrewInfoDataViewModel Eng1Hyd { get; }
        public CrewInfoDataViewModel Eng2Hyd { get; }
        public CrewInfoDataViewModel Throttle { get; }
        public CrewInfoDataViewModel IceTime { get; }



        private AppViewModel _appVm;
        private GENERIC_DATA newData;
        private Stopwatch m_lastGroundEvent = Stopwatch.StartNew();
        private double m_lastIsOnGround;

        CrewInfoDataViewModel Define(string title, Func<string> getValue, Func<bool> getWarning = null)
        {
            var ret = new CrewInfoDataViewModel(title, "");

            _appVm.DataArrived += (nd) =>
            {
                newData = nd;
                ret.Value = getValue();
                if (getWarning != null)
                {
                    ret.IsWarning = getWarning.Invoke();
                }
            };
            return ret;
        }

        public FlightInfoViewModel(AppViewModel appVm)
        {
            _appVm = appVm;
            IceInfo = new IceTrackerViewModel(appVm);

            OAT = Define("OAT",
                () => Math.Round(newData.outside_air_temp_c, 1).ToString(),
                () => newData.outside_air_temp_c < Reality.TEMP_LOW);
            OAT.WarningColor = System.Windows.Media.Colors.Cyan;

            TAS = Define("TAS",
                () => Math.Round(newData.airspeed_tas_kts, 0).ToString(),
                () => newData.sim_on_ground == 0 && newData.airspeed_tas_kts < Reality.SPEED_LOW);

            GS = Define("GS",
                () => Math.Round(newData.groundspeed_kts, 0).ToString(),
                () => newData.sim_on_ground == 0 && newData.groundspeed_kts < Reality.SPEED_LOW);

            G = Define("G",
               () => Math.Round(newData.gforce, 1).ToString());

            FuelQty = Define("Fuel Qty",
                () =>
                {
                    // TODO
                    // VL-3: 12lbs
                    // C172+G1k: 20lbs
                    var unusable_fuel_lbs = (appVm.FlightModelCfg?.Unsuable_Fuel_Gallons ?? 0) * newData.fuel_pounds_per_1gallon;

                    var fuel_cap_lbs = newData.fuel_total_capacity_gallons * newData.fuel_pounds_per_1gallon;
                    var fuel_reamining_pct = (newData.fuel_total_lbs - unusable_fuel_lbs) / (fuel_cap_lbs - unusable_fuel_lbs);
                    var qtyPercent = Math.Round(100 * fuel_reamining_pct, 0) + " %";
                    return qtyPercent;
                },
                () =>
                {
                    var unusable_fuel_lbs = (appVm.FlightModelCfg?.Unsuable_Fuel_Gallons ?? 0) * newData.fuel_pounds_per_1gallon;

                    var fuel_cap_lbs = newData.fuel_total_capacity_gallons * newData.fuel_pounds_per_1gallon;
                    var fuel_reamining_pct = (newData.fuel_total_lbs - unusable_fuel_lbs) / (fuel_cap_lbs - unusable_fuel_lbs);
                    return fuel_reamining_pct < 0.1;
                });

            RA = Define("RA",
                () =>
                {
                    var rad = (int)newData.radio_Height;
                    rad = rad > 300 ? (rad + 49) / 50 * 50 : rad; // Round to 50 for values higher than 300
                    RA.IsApplicable = rad <= 2500;
                    return rad.ToString();
                },
                () => appVm.Terrain.IsActive);

            Endurance = Define("Endurance",
                () => ETETextFromSeconds(newData.CalculateEndurance().TotalSeconds));

            Range = Define("Range",
                () => Math.Round(newData.groundspeed_kts * newData.CalculateEndurance().TotalHours, 0) + " nm");

            GPS_XTK = Define("GPS XTK",
                () => Math.Round(newData.gps_wp_xtrack_meters / 1000, 1).ToString());

            FuelBurn = Define("FuelBurn",
                () => Math.Round(newData.eng1_fuel_flow_pph + newData.eng2_fuel_flow_pph, 0) + " pph",
                () => newData.sim_on_ground == 0 && (newData.eng1_fuel_flow_pph < 15));

            ElevatorTrim = Define("TRIM",
                () => Math.Round(newData.elev_trim, 1).ToString(),
                () => newData.elev_trim > 0.3 || newData.elev_trim < -0.3);

            WP_DTG = Define("WP DTG",
                () => Math.Round(newData.gps_wp_distance / 1000 * Reality.KM_TO_NM).ToString());

            WP_ETE = Define("WP",
                () => ETETextFromSeconds(newData.gps_WP_ETE_seconds));

            Destination_ETE = Define("Destination",
                () => ETETextFromSeconds(newData.gps_ETE_seconds));

            FlightOrGroundTimer = Define("FLT",
                 () =>
                 {
                     if (newData.sim_on_ground != m_lastIsOnGround)
                     {
                         m_lastIsOnGround = newData.sim_on_ground;
                         m_lastGroundEvent = Stopwatch.StartNew();
                     }

                     FlightOrGroundTimer.Name = newData.sim_on_ground == 1 ? "GND" : "FLT";
                     return m_lastGroundEvent.Elapsed.ToString(@"hh\:mm\:ss");
                 });

            Batt1 = Define("Batt 1",
                 () => Math.Round(ReadBatteryCurve(newData.elec_batt1_volts),1).ToString(),
                 () => newData.elec_batt_load_1 > 0);
            Batt2 = Define("Batt 2",
                 () => Math.Round(ReadBatteryCurve(newData.elec_batt2_volts),1).ToString(),
                 () => newData.elec_batt_load_2 > 0);
            BattStby = Define("Batt STBY",
                 () => Math.Round(ReadBatteryCurve(newData.elec_batt3_volts),1).ToString(),
                 () => newData.elec_batt_load_3 > 0);

            Eng1Hyd = Define("Hyd Eng 1 Psi", () => Math.Round(newData.eng1_hydraulic_press).ToString());
            Eng2Hyd = Define("Hyd Eng 2 Psi", () => Math.Round(newData.eng2_hydraulic_press).ToString());
           
            Throttle = Define("TH", () => Math.Round(newData.eng1_throttle_pct, 0).ToString());

            //Pushback = Define("Pushback", () => Math.Round(newData.pushback_state).ToString());


            Batt1Time = new CrewInfoDataViewModel("Time", "");
            Batt2Time = new CrewInfoDataViewModel("Time", "");
            BattStbyTime = new CrewInfoDataViewModel("Time", "");
            IceTime = new CrewInfoDataViewModel("Ice", "");



            var batt1T = new ValueTracker();
            var batt2T = new ValueTracker();
            var batt3T = new ValueTracker();
            var iceT = new ValueTracker();

            int ctr = 0;
            appVm.DataArrived += (newData) =>
            {
                if (++ctr % 4 == 0)
                {
                    Batt1Time.Value = ETETextFromSeconds(batt1T.Update(ReadBatteryCurve(newData.elec_batt1_volts)));
                    Batt2Time.Value = ETETextFromSeconds(batt2T.Update(ReadBatteryCurve(newData.elec_batt2_volts)));
                    BattStbyTime.Value = ETETextFromSeconds(batt3T.Update(ReadBatteryCurve(newData.elec_batt3_volts)));
                    IceTime.Value = ETETextFromSeconds(iceT.Update(newData.structural_ice_pct * 100));
                }

                IsOnGround = newData.sim_on_ground == 1;

                BattStby.IsApplicable = HasStandbyBattery();
                BattStbyTime.IsApplicable = HasStandbyBattery();

                GS.IsApplicable = IsOnGround && 
                    (_appVm.CurrentAircraft == AircraftKind.CitationLongitude || _appVm.CurrentAircraft == AircraftKind.F22);
            };
        }

        bool HasStandbyBattery()
        {
            return _appVm.SystemsCfg?.HasBattery3 ?? false;
        }

        private string ETETextFromSeconds(object eTE)
        {
            throw new NotImplementedException();
        }

        class CurveInfo
        {
            public double Value { get; }
            public double PercentValue { get; }
            public CurveInfo(string text)
            {
                var parts = text.Split(':');
                Value = double.Parse(parts[1]);
                PercentValue = double.Parse(parts[0]);
            }
        }

        private double ReadBatteryCurve(double batt)
        {
            // %, volts
            // 0   :21, 
            // 0.1 :22.5, 
            // 0.5 :24, 
            // 0.9 :25, 
            // 1   :25.4
            if (_appVm.SystemsCfg == null) return 0;

            var battInfo = _appVm.SystemsCfg.Values["ELECTRICAL"]["battery.1"];
            var battParts = battInfo.Split('#');
            var values = new Dictionary<string, string>();
            foreach(var p in battParts)
            {
                var kv = p.Split(':');
                values.Add(kv[0], kv[1]);
            }

            var curveId = values["Voltage"];
            var curve = _appVm.SystemsCfg.Values["ELECTRICAL"][curveId];
            var curveParts = curve.Split(',').Select(c => new CurveInfo(c.Trim())).Reverse().ToArray();

            int idx = 0;
            double ret = -1;
            while (ret == -1 && idx < curveParts.Length)
            {
                var v = curveParts[idx].Value;
                var p = curveParts[idx].PercentValue;
                if (batt >= v)
                {
                    if (idx == 0)
                    {
                        ret = 1;
                    }
                    else
                    {
                        var prev = curveParts[idx - 1];
                        ret = ScaleValue(batt, v, prev.Value, p, prev.PercentValue);
                    }
                }
                idx++;
            }

          /*
            if (batt >= 25.4)
            {
                ret = 1;
            }
            else if (batt >= 25)
            {
                ret = ScaleValue(batt, 25, 25.4, 0.9, 1.0);
            }
            else if (batt >= 24)
            {
                ret = ScaleValue(batt, 24, 25, 0.5, 0.9);
            }
            else if (batt >= 22.5)
            {
                ret = ScaleValue(batt, 22.5, 24, 0.1, 0.5);
            }
            else //if (batt >= 21)
            {
                ret = ScaleValue(batt, 21, 22.5, 0, 0.1);
            }
          */
            return ret * 100;
        }

        private static double ScaleValue(double value, double valueMin, double valueMax, double scaleMin, double scaleMax)
        {
            double vPerc = (value - valueMin) / (valueMax - valueMin);
            double bigSpan = vPerc * (scaleMax - scaleMin);
            return scaleMin + bigSpan;
        }

        string ETETextFromSeconds(double seconds)
        {
            var dETE = TimeSpan.FromSeconds(seconds);
            return dETE.Hours + "+" + dETE.Minutes;
        }
    }
}
