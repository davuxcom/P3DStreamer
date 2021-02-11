using P3DStreamer.DataModel;
using System;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace P3DStreamer.ViewModel
{
    enum EICASMessageKind
    {
        Warning,
        Caution,
        Advisory,
    }

    class EICASMessageViewModel : BaseViewModel
    {
        public string Text { get => Get<string>(); set => Set(value); }
        public bool IsApplicable { get => Get<bool>(); set => Set(value); }
        public bool HasCommand { get => Get<bool>(); set => Set(value); }
        public ICommand Command { get => Get<ICommand>(); set => Set(value); }
        public string CommandText { get => Get<string>(); set => Set(value); }
        public EICASMessageKind Kind { get => Get<EICASMessageKind>(); set => Set(value); }

        public static EICASMessageViewModel Create(EICASMessageKind kind, string message)
        {
            switch (kind)
            {
                case EICASMessageKind.Advisory: return new InfoMessageViewModel(message);
                case EICASMessageKind.Caution: return new CautionMessageViewModel(message);
                case EICASMessageKind.Warning: return new WarningMessageViewModel(message);
                default: throw new NotImplementedException();
            }
        }
    }

    class CautionMessageViewModel : EICASMessageViewModel
    {
        public CautionMessageViewModel(string message)
        {
            Text = message;
            Kind = EICASMessageKind.Caution;
        }
    }

    class WarningMessageViewModel : EICASMessageViewModel
    {
        public WarningMessageViewModel(string message)
        {
            Text = message;
            Kind = EICASMessageKind.Warning;
        }
    }

    class InfoMessageViewModel : EICASMessageViewModel
    {
        public InfoMessageViewModel(string message)
        {
            Text = message;
            Kind = EICASMessageKind.Advisory;
        }
    }

    class BaseEICASViewModel: BaseViewModel
    {
        public ObservableCollection<EICASMessageViewModel> Messages { get; } = new ObservableCollection<EICASMessageViewModel>();


        protected AppViewModel _appVm;
        protected GENERIC_DATA newData;

        public BaseEICASViewModel(AppViewModel appVm)
        {
            _appVm = appVm;
        }

        protected EICASMessageViewModel Define(EICASMessageKind type, string text, Func<bool> getIsApplicable = null, Func<string> getApplicableText = null)
        {
            var msg = EICASMessageViewModel.Create(type, text);
            Messages.Add(msg);

            _appVm.DataArrived += (nd) =>
            {
                if (getIsApplicable != null)
                {
                    msg.IsApplicable = getIsApplicable();
                }
                if (msg.IsApplicable && getApplicableText != null)
                {
                    msg.Text = getApplicableText();
                }
            };
            return msg;
        }

        protected void WithResolution(EICASMessageViewModel vm, string text, SimEvents resolutionEvt)
        {
            vm.HasCommand = true;
            vm.CommandText = text;
            vm.Command = new RelayCommand(() => _appVm.DoCommand(resolutionEvt));
        }
    }
}
