using P3DStreamer.DataModel;
using P3DStreamer.Extensions;
using P3DStreamer.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;

namespace P3DStreamer
{
    public enum AvionicsKind
    {
        Unknown,
        G1000,
        G3000,
        G3X,
        F22,
    }

    public enum EngineKind
    {
        Unknown,
        Piston,
        Turboprop,
        Jet,
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class AircraftInfoAttribute : Attribute
    {
        public string Name { get; set; }
        public string CfgPath { get; set; }
        public AvionicsKind Avionics { get; set; }
        public EngineKind Engine { get; set; } = EngineKind.Piston;
        public int EngineCount { get; set; } = 1;
    }

    public enum AircraftKind
    {
        Unknown, 

        [AircraftInfo(Name = "Bonanza G36 Asobo")]
        [AircraftInfo(Name = "Bonanza G36 Turbo")]
        
      //  [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-bonanza-g36\SimObjects\Airplanes\Asobo_Bonanza_G36")]
        [AircraftInfo(CfgPath = @"Community\Bonanza-Turbo-V3\SimObjects\Airplanes\Asobo_Bonanza_G36")]
        [AircraftInfo(Avionics = AvionicsKind.G1000)]
        Bonanza,
        [AircraftInfo(Name = "Asobo Baron G58")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-baron-g58\SimObjects\Airplanes\Asobo_Baron_G58")]
        [AircraftInfo(Avionics = AvionicsKind.G1000, EngineCount = 2)]
        Baron,
        [AircraftInfo(Name = "Cessna 208B Grand Caravan EX")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-208b-grand-caravan-ex\SimObjects\Airplanes\Asobo_208B_GRAND_CARAVAN_EX")]
        [AircraftInfo(Avionics = AvionicsKind.G1000, Engine = EngineKind.Turboprop)]
        Caravan,
        [AircraftInfo(Name = "Cessna Skyhawk G1000 Asobo")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-c172sp-as1000\SimObjects\Airplanes\Asobo_C172sp_AS1000")]
        [AircraftInfo(Avionics = AvionicsKind.G1000)]
        C172,
        
        [AircraftInfo(Name = "Cessna Citation Longitude Asobo")]
        [AircraftInfo(Name = "Cessna Citation Longitude (Alaska Airlines Executive Transport)")]
        [AircraftInfo(CfgPath = @"Community\aircraft-longitudeFDEfix\SimObjects\AirPlanes\Asobo_Longitude")]
      //  [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-longitude\SimObjects\Airplanes\Asobo_Longitude")]
        [AircraftInfo(Avionics = AvionicsKind.G3000, EngineCount = 2, Engine = EngineKind.Jet)]
        CitationLongitude,
        [AircraftInfo(Name = "VL3 Asobo")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-vl3\SimObjects\Airplanes\Asobo_VL3")]
        [AircraftInfo(Avionics = AvionicsKind.G3X)]
        VL3,
        [AircraftInfo(Name = "TBM 930 Asobo")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-tbm930\SimObjects\Airplanes\Asobo_TBM930")]
        [AircraftInfo(Avionics = AvionicsKind.G3000, Engine = EngineKind.Turboprop)]
        TBM930,
        [AircraftInfo(Name = "DA40-NG Asobo")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-da40-ng\SimObjects\Airplanes\Asobo_DA40_NG")]
       // [AircraftInfo(CfgPath = @"Community\DA40-NGX2\SimObjects\Airplanes\Asobo_DA40_NG")]
        [AircraftInfo(Avionics = AvionicsKind.G1000)]
        DA40NG,
        [AircraftInfo(Name = "SR22 Asobo")]
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-sr22\SimObjects\Airplanes\Asobo_SR22")]
        [AircraftInfo(Avionics = AvionicsKind.G1000)]
        SR22,
        [AircraftInfo(CfgPath = @"Official\OneStore\asobo-aircraft-icon\SimObjects\Airplanes\Asobo_Icon")]
        [AircraftInfo(Avionics = AvionicsKind.G3X)]
        [AircraftInfo(Name = "Icon A5 Asobo")]
        A5,
        [AircraftInfo(Name = "F-22 Raptor")]
        [AircraftInfo(Avionics = AvionicsKind.F22)]
        [AircraftInfo(CfgPath = @"Community\evanburnsdev-f22raptor\SimObjects\Airplanes\raptor")]
        F22,
    }

    class AircraftInfo
    {
        public AvionicsKind Avionics { get; set; }
        public AircraftKind Aircraft { get; set; }
        public string Cfg { get; set; }
    }

    class AppViewModel : BaseViewModel
    {
        public event Action<bool> Connected;
        public event Action<GENERIC_DATA> DataArrived;

        public AircraftKind CurrentAircraft { get => Get<AircraftKind>(); set => Set(value); }
        public AvionicsKind CurrentAircraftAvionics { get => Get<AvionicsKind>(); set => Set(value); }
        public FlightModelCfg FlightModelCfg { get => Get<FlightModelCfg>(); set => Set(value); }
        public EngineCfg EngineCfg { get => Get<EngineCfg>(); set => Set(value); }
        public SystemsCfg SystemsCfg { get => Get<SystemsCfg>(); set => Set(value); }
        public EICASViewModel EICAS { get; }
        public TerrainProtectionSystemViewModel Terrain { get; }
        public MixViewModel Mix { get; }
        public APModeControlPanelViewModel AP { get; }
        public AutoThrottleViewModel AT { get; }
        public MasterCautionViewModel MasterCaution { get; }
        public FlightInfoViewModel Info { get; }
        public PropPitchViewModel Prop { get; }
        public SecondaryModeControlPanel MCP2 { get; }
        public SystemModeControlPanelViewModel MCP3 { get; }
        
        public AircraftViewModel Aircraft { get; }

        private SimConnector m_simConnect;
        private Dictionary<string, AircraftInfo> m_aircraftInfo = new Dictionary<string, AircraftInfo>();

        public AppViewModel()
        {
            Aircraft = new AircraftViewModel();

            Info = new FlightInfoViewModel(this);
            EICAS = new EICASViewModel(this);
            MasterCaution = new MasterCautionViewModel(this, EICAS);
            Terrain = new TerrainProtectionSystemViewModel(this);
            Mix = new MixViewModel(this);
            AP = new APModeControlPanelViewModel(this);
            Prop = new PropPitchViewModel(this);
            MCP2 = new SecondaryModeControlPanel(this);
            MCP3 = new SystemModeControlPanelViewModel(this);
            AT = new AutoThrottleViewModel(this);

            foreach (Enum aircraftKind in Enum.GetValues(typeof(AircraftKind)))
            {
                var attrs = aircraftKind.GetAttributeOfType<AircraftInfoAttribute>();
                var avionics = attrs.FirstOrDefault(a => a.Avionics != AvionicsKind.Unknown)?.Avionics;
                var cfgPath = attrs.FirstOrDefault(a => a.CfgPath != null)?.CfgPath;

                foreach (var attr in attrs.Where(a => !string.IsNullOrWhiteSpace(a.Name)))
                {
                    m_aircraftInfo[attr.Name] = new AircraftInfo
                    {
                        Aircraft = (AircraftKind)aircraftKind,
                        Avionics = avionics ?? AvionicsKind.Unknown,
                        Cfg = cfgPath,
                    };
                }
            }

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                var t = new Thread(SimConnectThreadProc);
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            });
        }

        void SimConnectThreadProc()
        {
            m_simConnect = new SimConnector();
            m_simConnect.Connected += (isConnected) => Connected?.Invoke(isConnected);
            m_simConnect.Data += (newData) =>
            {
                AppDataArrived(newData);
                DataArrived?.Invoke(newData);
            };
            m_simConnect.Connect();

            Dispatcher.Run();
        }

        private void AppDataArrived(GENERIC_DATA newData)
        {
            if (m_aircraftInfo.ContainsKey(newData.title))
            {
                var currentAircraft = CurrentAircraft;
                CurrentAircraft = m_aircraftInfo[newData.title].Aircraft;
                CurrentAircraftAvionics = m_aircraftInfo[newData.title].Avionics;

                if (currentAircraft != CurrentAircraft)
                {
                    var fspath = @"C:\Users\dave\AppData\Local\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalCache\Packages\";
                    var cfgPath = m_aircraftInfo[newData.title].Cfg;
                    FlightModelCfg = new FlightModelCfg(Path.Combine(fspath, cfgPath, "flight_model.cfg"));
                    SystemsCfg = new SystemsCfg(Path.Combine(fspath, cfgPath, "systems.cfg"));
                }

            }
            else
            {
                Trace.WriteLine("### Unknown Aircraft ### " + newData.title);
            }
        }

        public void DoCommand(SimEvents cmd, uint dwData = 0) => m_simConnect.SendEvent(cmd, dwData);
    }
}
