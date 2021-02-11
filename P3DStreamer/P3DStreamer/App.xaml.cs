using P3DStreamer.Extensions;
using P3DStreamer.ViewModel;
using System.Diagnostics;
using System.Windows;

namespace P3DStreamer
{
    public partial class App : Application
    {
        enum ExperienceKind { FullWindow, None};

        Window CreateNone(object dataContext) => Create(dataContext, ExperienceKind.None);
        Window Create(object dataContext, ExperienceKind experience = ExperienceKind.FullWindow)
        {
            var ret = new Window { DataContext = dataContext };
            if (experience == ExperienceKind.None)
            {
                ret.Style = (Style)FindResource("BorderlessWindowStyle");
            }

            ret.SourceInitialized += (_, __) => ret.Cloak();
            // TODO: add closing
            ret.Show();
            return ret;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;

            var appVm = new AppViewModel();

            var mfd = new MFDViewModel(appVm);
            var mfd_overlay = new MFDOverlayViewModel(appVm);
            // hacky
            mfd.PropertyChanged += (_, __) =>
            {
                mfd_overlay.IsSystemOpen = mfd.IsSystemOpen;
                mfd_overlay.IsLightsOpen = mfd.IsLightsOpen;
            };
            Create(new PFDViewModel(appVm));
            Create(mfd);
            CreateNone(new PFDOverlayViewModel(appVm));
            CreateNone(mfd_overlay);
            CreateNone(new TaskbarViewModel(appVm));

            var main = new MainWindowViewModel(appVm);
            var mainWindow = CreateNone(main);
            main._window = mainWindow;

            Create(new CDUViewModel(appVm, "CDUExt"));
            CreateNone(new CDUOverlayViewModel(appVm, main, "CDUTop1"));
            CreateNone(new CDUOverlayViewModel(appVm, main, "CDUTop2"));
        }
    }
}
