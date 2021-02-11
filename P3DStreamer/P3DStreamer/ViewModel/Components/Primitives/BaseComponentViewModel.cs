using System.Windows.Media;

namespace P3DStreamer.ViewModel
{
    internal class BaseComponentViewModel : BaseViewModel
    {
        public bool IsActive { get => Get<bool>(); set => Set(value); }
        public bool IsWarning { get => Get<bool>(); set => Set(value); }
        public Color WarningColor { get => Get<Color>(); set => Set(value); }
        public bool IsApplicable { get => Get<bool>(); set => Set(value); }

        public BaseComponentViewModel()
        {
            IsApplicable = true;
        }
    }
}
