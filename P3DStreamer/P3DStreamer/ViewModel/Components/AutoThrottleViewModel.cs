using P3DStreamer.DataModel;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class AutoThrottleViewModel : BaseComponentViewModel
    {
        public double DesiredSpeed { get => Get<double>(); set => Set(value); }

        public ICommand Up { get; }
        public ICommand Down { get; }

        public AutoThrottleViewModel(AppViewModel appVm)
        {
            Up = new RelayCommand(() => DesiredSpeed += 5);
            Down = new RelayCommand(() => DesiredSpeed -= 5);

            DesiredSpeed = 90;

            var pid = new PID();
            pid.PV = new PID.Range { Min = 0, Max = 400 };
            pid.OV = new PID.Range { Min = (16383 * 0.3), Max = (16383 * 1.0) };
            pid.Gains = new PID.Gain { P = 1, I = 5, D = 0 };

            var time = Stopwatch.StartNew();
            appVm.DataArrived += (newData) =>
            {
               // IsApplicable = appVm.CurrentAircraft != AircraftKind.CitationLongitude;
                IsWarning = newData.eng1_throttle_pct < 5 && IsActive;

                if (IsActive)
                {
                    var setpoint = DesiredSpeed; // newData.ap_desired_airspeed;
                    var pv = newData.airspeed_ias_kts;
                    var ov = pid.Compute(pv, setpoint, time.Elapsed.TotalSeconds);
                    time.Restart();

                   // Trace.WriteLine($"AT: pv={pv} setp={setpoint} ov%={Math.Round(ov / 16383, 3)}");
                    appVm.DoCommand(SimEvents.THROTTLE_SET, (uint)ov);
                }
            };
        }
    }
}
