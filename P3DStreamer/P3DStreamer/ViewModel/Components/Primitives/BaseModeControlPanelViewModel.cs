using P3DStreamer.DataModel;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class MCPItemViewModel : BaseComponentViewModel
    {
        public string Text { get => Get<string>(); set => Set(value); }
    }

    class MCPKnobViewModel : MCPItemViewModel
    {
        public ICommand Up { get => Get<ICommand>(); set => Set(value); }
        public ICommand Down { get => Get<ICommand>(); set => Set(value); }
    }

    class MCPModeViewModel : MCPItemViewModel
    {
        public bool IsPlaceholder { get => Get<bool>(); set => Set(value); }
        public ICommand Invoke { get => Get<ICommand>(); set => Set(value); }
    }

    class BaseModeControlPanelViewModel : BaseViewModel
    {
        public ObservableCollection<MCPModeViewModel> Modes { get; } = new ObservableCollection<MCPModeViewModel>();

        protected AppViewModel _appVm;
        protected GENERIC_DATA newData;

        public BaseModeControlPanelViewModel(AppViewModel appVm)
        {
            _appVm = appVm;
            appVm.DataArrived += (nd) => newData = nd;
        }

        protected MCPModeViewModel DefineMode(string name, SimEvents evt, Func<bool> getData, Func<bool> getIsWarning = null, uint eventData = 1)
        {
            return DefineMode(name, () => _appVm.DoCommand(evt, eventData), getData, getIsWarning);
        }

        protected void AddPlaceholder()
        {
            var mode = new MCPModeViewModel
            {
                IsPlaceholder = true,
            };
            Modes.Add(mode);
        }

        protected MCPModeViewModel DefineMode(string name, Action toggle, Func<bool> getData, Func<bool> getIsWarning = null)
        {
            var mode = new MCPModeViewModel
            {
                Text = name,
                Invoke = new RelayCommand(toggle),
                WarningColor = System.Windows.Media.Colors.White,
            };
            Modes.Add(mode);

            _appVm.DataArrived += (nd) =>
            {
                mode.IsActive = getData();
                if (getIsWarning != null)
                {
                    mode.IsWarning = getIsWarning();
                }
            };
            return mode;
        }

        protected MCPKnobViewModel DefineKnob(SimEvents evtUp, SimEvents evtDown, uint val = 1, int count = 1)
        {
            var knob = new MCPKnobViewModel();
            knob.Up = new RelayCommand(() =>
            {
                for (var i = 0; i < count; i++)
                    _appVm.DoCommand(evtUp, val);
            });
            knob.Down = new RelayCommand(() =>
            {
                for (var i = 0; i < count; i++)
                    _appVm.DoCommand(evtDown, val);
            });
            return knob;
        }
    }

}
