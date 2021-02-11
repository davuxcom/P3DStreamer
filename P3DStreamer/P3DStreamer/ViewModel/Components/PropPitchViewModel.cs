using System;
using System.Windows.Input;
using System.Diagnostics;
using P3DStreamer.DataModel;

namespace P3DStreamer.ViewModel
{
    class PropPitchViewModel : BaseComponentViewModel
    {
        public string Value { get => Get<string>(); set => Set(value); }

        public ICommand Up { get; }
        public ICommand Down { get; }

        GENERIC_DATA lastData = default(GENERIC_DATA);

        public PropPitchViewModel(AppViewModel appVm)
        {
            Up = new RelayCommand(() => {
                var ov = 16383 * ((lastData.eng1_prop_percent / 100) + 0.01);
                appVm.DoCommand(SimEvents.PROP_PITCH_SET, (uint)ov);
            });
            Down = new RelayCommand(() => {
                var ov = 16383 * ((lastData.eng1_prop_percent / 100) - 0.01);
                appVm.DoCommand(SimEvents.PROP_PITCH_SET, (uint)ov);
            });

            var mix_pid = new PID();
            mix_pid.PV = new PID.Range { Min = 0, Max = 1.2 };
            mix_pid.OV = new PID.Range { Max = 16383/2, Min = 16383 };
            mix_pid.Gains = new PID.Gain { P = 0.1, I = -3, D = 0 };
            var SETPOINT = 0.9985; // Ballmark of just under 2700rpm in C172+G1k

            var time = Stopwatch.StartNew();

            appVm.DataArrived += (newData) =>
            {
                IsApplicable = appVm.CurrentAircraft != AircraftKind.C172 &&
                appVm.CurrentAircraft != AircraftKind.CitationLongitude &&
                appVm.CurrentAircraft != AircraftKind.DA40NG &&
                appVm.CurrentAircraft != AircraftKind.F22;

                lastData = newData;
                Value = Math.Round(newData.eng1_prop_percent,1).ToString();

                if (IsActive)
                {
                    var pv = newData.eng1_rpm / newData.eng_rpm_max;
                    var ov = mix_pid.Compute(pv, SETPOINT, time.Elapsed.TotalSeconds);
                    time.Restart();

                    if (newData.flaps_position_idx > 0 || newData.sim_on_ground == 1)
                    {
                        ov = 16383;
                        mix_pid.ClearError();
                    }

                    appVm.DoCommand(SimEvents.PROP_PITCH_SET, (uint)ov);
                }
            };
        }
    }
}
