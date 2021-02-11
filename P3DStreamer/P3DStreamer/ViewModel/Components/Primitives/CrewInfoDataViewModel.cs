using System.Windows.Media;

namespace P3DStreamer.ViewModel
{
    class CrewInfoDataViewModel : BaseViewModel
    {
        public string Name { get => Get<string>(); set => Set(value); }
        public string Value { get => Get<string>(); set => Set(value); }
        public bool IsWarning { get => Get<bool>(); set => Set(value); }
        public bool IsApplicable { get => Get<bool>(); set => Set(value); }
        public Color WarningColor { get => Get<Color>(); set => Set(value); }
        
        public CrewInfoDataViewModel(string name, string value) {
            IsApplicable = true;
            WarningColor = Colors.Yellow;
            Name = name; Value = value;
        }
    }
}
