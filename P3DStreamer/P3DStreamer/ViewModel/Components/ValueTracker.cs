using System;
using System.Diagnostics;

namespace P3DStreamer.ViewModel
{
    internal class ValueTracker
    {
        private double _low;
        private double _high;

        private Stopwatch _time = Stopwatch.StartNew();
        private double _last = double.NaN;

        public ValueTracker(double low = 0, double high = 100)
        {
            _low = low;
            _high = high;
        }

        public double Update(double val)
        {
            double ret = 0;

            if (!double.IsNaN(_last))
            {
                var dR = _high - _low;
                var dt = _time.Elapsed.TotalSeconds;
                var dx = val - _last;

                if (dx < 0)
                {
                    var dGap = val - _low;
                   

                    var dGap_per_dx = dGap / dx;
                    ret = -1 * dGap_per_dx * dt;

                }
                else if (dx > 0)
                {
                    var dGap = _high - val;

                    var dGap_per_dx = dGap / dx;
                    ret = dGap_per_dx * dt;
                }

            }
            _last = val;
            _time.Restart();

            return ret;
        }
    }
}