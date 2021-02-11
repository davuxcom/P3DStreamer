using P3DStreamer.DataModel;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class MixViewModel : BaseComponentViewModel
    {
        public string Value { get => Get<string>(); set => Set(value); }

        public ICommand Up { get; }
        public ICommand Down { get; }
        public ICommand UpSmall { get; }
        public ICommand DownSmall { get; }

        GENERIC_DATA lastData = default(GENERIC_DATA);

        private int setpoint = 120;

        public MixViewModel(AppViewModel appVm)
        {
            Up = new RelayCommand(() =>
            {
                if (IsActive)
                {
                    setpoint += 5;
                }
                else
                {
                    var ov = 16383 * ((lastData.eng1_mix_percent / 100) + 0.05);
                    appVm.DoCommand(SimEvents.MIXTURE_SET, (uint)ov);
                }
            });
            Down = new RelayCommand(() =>
            {
                if (IsActive)
                {
                    setpoint -= 5;
                }
                else
                {

                    var ov = 16383 * ((lastData.eng1_mix_percent / 100) - 0.05);
                    appVm.DoCommand(SimEvents.MIXTURE_SET, (uint)ov);
                }
            });
            UpSmall = new RelayCommand(() =>
            {
                var ov = 16383 * ((lastData.eng1_mix_percent / 100) + 0.005);
                appVm.DoCommand(SimEvents.MIXTURE_SET, (uint)ov);
            });
            DownSmall = new RelayCommand(() =>
            {
                var ov = 16383 * ((lastData.eng1_mix_percent / 100) - 0.005);
                appVm.DoCommand(SimEvents.MIXTURE_SET, (uint)ov);
            });
            /*
             * this was used for 2020 cannonball run to optimize gph against only 2700rpm limit
            var mix_pid = new PID();
            mix_pid.PV = new PID.Range { Min = 0, Max = 1.2 };
            mix_pid.OV = new PID.Range { Max = 0, Min = 16383 };
            mix_pid.Gains = new PID.Gain { P = 0.1, I = -2, D = 0 };
            var SETPOINT = 0.9985; // Ballmark of just under 2700rpm in C172+G1k
            */

            var mix_pid = new PID();
            mix_pid.PV = new PID.Range { Min = 0, Max = 250 };
            mix_pid.OV = new PID.Range { Min = 16383, Max = 0 };
            mix_pid.Gains = new PID.Gain { P = 0.1, I = -0.4, D = 0 };

            var time = Stopwatch.StartNew();
            appVm.DataArrived += (newData) =>
            {
                IsApplicable = appVm.CurrentAircraft != AircraftKind.CitationLongitude &&
                appVm.CurrentAircraft != AircraftKind.DA40NG &&
                appVm.CurrentAircraft != AircraftKind.F22;


                lastData = newData;
                Value = Math.Round(newData.eng1_mix_percent, 1).ToString();

                var pv = newData.eng1_fuel_flow_pph;
                var ov = mix_pid.Compute(pv, setpoint, time.Elapsed.TotalSeconds);
                time.Restart();
                if (newData.flaps_position_idx > 0 || newData.sim_on_ground == 1)
                {
                    ov = 16383;
                    mix_pid.ClearError();
                }

                if (IsActive)
                {
                    // Trace.WriteLine($"MIX: pph={newData.fuel_flow_pph_1} setp={setpoint} mix={Math.Round(ov / 16383, 3)} pv={pv}");
                    appVm.DoCommand(SimEvents.MIXTURE_SET, (uint)ov);
                }
            };
        }
    }
}
