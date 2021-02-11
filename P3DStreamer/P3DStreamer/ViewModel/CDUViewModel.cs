
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class CDUOverlayViewModel : BaseViewModel
    {
        public string Title { get => Get<string>(); set => Set(value); }
        public ICommand Toggle { get; }

        public CDUOverlayViewModel(AppViewModel appVm, MainWindowViewModel main, string title)
        {
            Title = title;
            Toggle = new RelayCommand(() => main.FlipCDUs());
        }
    }

    class CDUViewModel : BaseViewModel
    {
        public AppViewModel App { get; }

        public string Title { get => Get<string>(); set => Set(value); }
    
        public CDUViewModel(AppViewModel appVm, string title)
        {
            App = appVm;
            Title = title;
        }
    }
}
