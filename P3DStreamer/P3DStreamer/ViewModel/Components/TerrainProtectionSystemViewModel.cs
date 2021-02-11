using P3DStreamer.DataModel;
using System;
using System.Diagnostics;
using System.Media;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class TerrainProtectionSystemViewModel : BaseComponentViewModel
    {
        public int ProtectLevel { get => Get<int>(); set => Set(value); }

        public ICommand ChangeProtectLevel { get; }

        public TerrainProtectionSystemViewModel(AppViewModel appVm)
        {
            ProtectLevel = 500;
            ChangeProtectLevel = new RelayCommand(() =>
            {
                switch (ProtectLevel)
                {
                    case 100:
                        ProtectLevel = 200;
                        break;
                    case 200:
                        ProtectLevel = 300;
                        break;
                    case 300:
                        ProtectLevel = 400;
                        break;
                    case 400:
                        ProtectLevel = 500;
                        break;
                    case 500:
                        ProtectLevel = 1000;
                        break;
                    case 1000:
                        ProtectLevel = 2000;
                        break;
                    case 2000:
                        ProtectLevel = 100;
                        break;
                }
            });

            Stopwatch lastActivation = Stopwatch.StartNew();
            appVm.DataArrived += (newData) =>
            {
                if (newData.radio_Height < ProtectLevel && // Under floor
                    (newData.gear_is_retractable == 0 || newData.gear_ext_pct == 0) &&    // Gear up
                    newData.flaps_position_idx == 0 && // Flaps up
                    IsActive &&
                    ProtectLevel > 0)
                {
                    if (lastActivation.Elapsed > TimeSpan.FromSeconds(2))
                    {
                        IsWarning = true;
                        lastActivation = Stopwatch.StartNew();

                        if (newData.ap_master == 1)
                        {
                            var nextAlt = (uint)(newData.indicated_alt + 200);

                            appVm.DoCommand(SimEvents.AP_ALT_VAR_SET_ENGLISH, nextAlt);
                            appVm.DoCommand(SimEvents.AP_VS_HOLD, 1);
                            appVm.DoCommand(SimEvents.AP_VS_VAR_SET_ENGLISH, 2500);

                            new SoundPlayer("terrain.wav").Play();
                        }
                    }
                }
                else
                {
                    if (lastActivation.Elapsed > TimeSpan.FromSeconds(2))
                    {
                        IsWarning = false;
                    }
                }
            };
        }
    }
}
