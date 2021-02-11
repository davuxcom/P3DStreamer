using P3DStreamer.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class PFDOverlayViewModel : BaseViewModel
    {
        public AppViewModel App { get; }
        public string Title { get => Get<string>(); set => Set(value); }

        public ObservableCollection<CrewInfoDataViewModel> Data { get; } = new ObservableCollection<CrewInfoDataViewModel>();
        public ObservableCollection<CrewInfoDataViewModel> DataRight { get; } = new ObservableCollection<CrewInfoDataViewModel>();


        public ICommand TogglePanel { get; }
        public bool ShowTopData { get => Get<bool>(); set => Set(value); }
        public bool ShowRightTopData { get => Get<bool>(); set => Set(value); }
        public bool IsPanelOpen { get => Get<bool>(); set => Set(value); }
      //  public bool ShowGS { get => Get<bool>(); set => Set(value); }
        public string GSOrRA { get => Get<string>(); set => Set(value); }

        public PFDOverlayViewModel(AppViewModel appVm)
        {
            App = appVm;
            Title = "PFDTop";
            TogglePanel = new RelayCommand(() => IsPanelOpen = !IsPanelOpen);

            DataRight.Add(appVm.Info.ElevatorTrim);
            DataRight.Add(appVm.Info.Throttle);
            DataRight.Add(appVm.Info.Destination_ETE);

            Data.Add(appVm.Info.Destination_ETE);
            Data.Add(appVm.Info.WP_ETE);
            Data.Add(appVm.Info.WP_DTG);

            appVm.DataArrived += (newData) =>
            {
                ShowTopData = appVm.CurrentAircraftAvionics == AvionicsKind.G1000;
                ShowRightTopData = appVm.CurrentAircraftAvionics == AvionicsKind.G1000 || appVm.CurrentAircraftAvionics == AvionicsKind.F22;

                bool isRA = newData.sim_on_ground == 0 && newData.radio_Height < 2500;

                GSOrRA = isRA ? App.Info.RA.Value : App.Info.GS.Value;


              //  ShowGS = appVm.CurrentAircraft == AircraftKind.CitationLongitude;
            };
        }
    }

    class PFDViewModel : BaseViewModel
    {
        public AppViewModel App { get; }
        public string Title { get => Get<string>(); set => Set(value); }

        public ObservableCollection<CrewInfoDataViewModel> Data { get; } = new ObservableCollection<CrewInfoDataViewModel>();
        public System.Windows.Controls.Orientation PanelOrientation { get => Get<System.Windows.Controls.Orientation>(); set => Set(value); }

        public PFDViewModel(AppViewModel appVm)
        {
            App = appVm;
            Title = "PFD_EXT";

            appVm.DataArrived += (newData) =>
            {
                PanelOrientation = (appVm.CurrentAircraftAvionics == AvionicsKind.G3000 || appVm.CurrentAircraftAvionics == AvionicsKind.G3X) ? System.Windows.Controls.Orientation.Horizontal : System.Windows.Controls.Orientation.Vertical;
            };
        }
    }
}