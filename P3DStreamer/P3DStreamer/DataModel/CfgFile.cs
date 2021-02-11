using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P3DStreamer.DataModel
{
    class FlightModelCfg : CfgFile
    {
        public bool HasAutoSpoilers => Values["AIRPLANE_GEOMETRY"]["auto_spoiler_available"] == "1";


        public double Unsuable_Fuel_Gallons => GetUnusableFuel();
        public double Total_Fuel_Gallons => GetTotalFuel();

        public FlightModelCfg(string path) : base(path)
        {

        }

        private (double total, double unusable) GetFuelInfo(string key)
        {
            if (!Values["FUEL"].ContainsKey(key))
            {
                return (0, 0);
            }
            var parts = Values["FUEL"][key].Split(',').Select(v => double.Parse(v.Trim())).ToArray();
            return (parts[3], parts[4]);
        }

        private double GetTotalFuel()
        {
            var left = GetFuelInfo("leftmain");
            var right = GetFuelInfo("rightmain");
            var center1 = GetFuelInfo("center1");
            var center2 = GetFuelInfo("center2");
            var center3 = GetFuelInfo("center3");
            return left.total +
                right.total +
                center1.total +
                center2.total +
                center3.total;
        }

        private double GetUnusableFuel()
        {
            var left = GetFuelInfo("leftmain");
            var right = GetFuelInfo("rightmain");
            var center1 = GetFuelInfo("center1");
            var center2 = GetFuelInfo("center2");
            var center3 = GetFuelInfo("center3");
            return left.unusable +
                right.unusable +
                center1.unusable +
                center2.unusable +
                center3.unusable;
        }
    }

    class EngineCfg : CfgFile
    {
        public EngineCfg(string path) : base(path)
        {

        }

    }

    class SystemsCfg : CfgFile
    {
        public bool HasBattery3 => Values["ELECTRICAL"].ContainsKey("battery.3");

        public bool HasAutoThrottle => Values["AUTOPILOT"]["autothrottle_available"] == "1";
        public SystemsCfg(string path) : base(path)
        {

        }

    }

    class CfgFile
    {
        public Dictionary<string, Dictionary<string, string>> Values { get; } = new Dictionary<string, Dictionary<string, string>>();

        public CfgFile(string path)
        {
            var lines = File.ReadAllLines(path);

            Dictionary<string, string> current = null;
            foreach(var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (line.StartsWith("["))
                    {
                        current = new Dictionary<string, string>();
                        Values[line.Replace("[", "").Replace("]", "").ToUpper()] = current;
                    }
                    else
                    {
                        var strippedLine = line.Split(';')[0].Trim();
                        if (!string.IsNullOrWhiteSpace(strippedLine) && strippedLine.Contains("="))
                        {
                            var parts = strippedLine.Split('=');
                            var key = parts[0].ToLower();
                            var value = strippedLine.Substring(key.Length + 1).Trim();
                            current[key.Trim()] = value;
                        }
                    }
                }
            }
        }
    }
}
