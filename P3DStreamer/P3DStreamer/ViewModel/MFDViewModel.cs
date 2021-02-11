using System.Collections.ObjectModel;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class DualCrewInfoData
    {
        public CrewInfoDataViewModel Right { get; set; }
        public CrewInfoDataViewModel Left { get; set; }
    }

    public enum ViewKind
    {
        Wide, Narrow
    };

    class MFDOverlayViewModel : BaseViewModel
    {
        public AppViewModel App { get; }
        public string Title { get => Get<string>(); set => Set(value); }

        public ObservableCollection<CrewInfoDataViewModel> Data { get; } = new ObservableCollection<CrewInfoDataViewModel>();
        public ObservableCollection<CrewInfoDataViewModel> DataRight { get; } = new ObservableCollection<CrewInfoDataViewModel>();
        public ObservableCollection<CrewInfoDataViewModel> DataInfo { get; } = new ObservableCollection<CrewInfoDataViewModel>();

        public bool ShowTopData { get => Get<bool>(); set => Set(value); }
        public bool IsSystemOpen { get => Get<bool>(); set => Set(value); }
        public bool IsLightsOpen { get => Get<bool>(); set => Set(value); }


        public MFDOverlayViewModel(AppViewModel appVm)
        {
            App = appVm;
            Title = "MFDTop";
            Data.Add(appVm.Info.OAT);
            DataRight.Add(appVm.Info.TAS);
            DataRight.Add(appVm.Info.GS);

            DataInfo.Add(appVm.Info.Batt1);
            DataInfo.Add(appVm.Info.Batt1Time);
            DataInfo.Add(appVm.Info.Batt2);
            DataInfo.Add(appVm.Info.Batt2Time);
            DataInfo.Add(appVm.Info.BattStby);
        //    DataInfo.Add(appVm.Info.Pushback);
            DataInfo.Add(appVm.Info.BattStbyTime);


            appVm.DataArrived += (newData) =>
            {
                ShowTopData = appVm.CurrentAircraftAvionics == AvionicsKind.G1000;
            };
        }
    }

    class MFDViewModel : BaseViewModel
    {
        public AppViewModel App { get; }
        public string Title { get => Get<string>(); set => Set(value); }

        public ObservableCollection<CrewInfoDataViewModel> TopData { get; } = new ObservableCollection<CrewInfoDataViewModel>();
        public ObservableCollection<DualCrewInfoData> Data { get; } = new ObservableCollection<DualCrewInfoData>();
        public ObservableCollection<CrewInfoDataViewModel> DataSingleColumn { get; } = new ObservableCollection<CrewInfoDataViewModel>();
        public ViewKind View { get => Get<ViewKind>(); set => Set(value); }
        public bool IsSystemOpen { get => Get<bool>(); set => Set(value); }
        public bool IsLightsOpen { get => Get<bool>(); set => Set(value); }

        public MFDViewModel(AppViewModel appVm)
        {
            App = appVm;
            Title = "MFD_EXT";

            Data.Add(new DualCrewInfoData { Left = appVm.Info.GS, Right = appVm.Info.TAS });
            Data.Add(new DualCrewInfoData { Left = appVm.Info.WP_ETE, Right = appVm.Info.WP_DTG });

            Data.Add(new DualCrewInfoData { Left = appVm.Info.Endurance, Right = appVm.Info.Range });
            Data.Add(new DualCrewInfoData { Left = appVm.Info.FuelQty, Right = appVm.Info.FuelBurn });

            DataSingleColumn.Add(appVm.Info.Destination_ETE);
            DataSingleColumn.Add(appVm.Info.Endurance);
            DataSingleColumn.Add(appVm.Info.FuelQty);

            TopData.Add(appVm.Info.FlightOrGroundTimer);

            appVm.DataArrived += (newData) =>
            {
                View = (appVm.CurrentAircraftAvionics == AvionicsKind.G3000 || appVm.CurrentAircraftAvionics == AvionicsKind.G3X) ? ViewKind.Narrow : ViewKind.Wide;
            };
        }
    }
}
