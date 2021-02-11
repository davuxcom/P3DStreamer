namespace P3DStreamer.ViewModel
{
    class TaskbarViewModel : BaseViewModel
    {
        public string Title { get => Get<string>(); set => Set(value); }

        public TaskbarViewModel(AppViewModel appVm)
        {
            Title = "TaskbarBlackout";
        }
    }
}