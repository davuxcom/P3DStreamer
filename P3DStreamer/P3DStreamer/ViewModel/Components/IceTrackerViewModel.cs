namespace P3DStreamer.ViewModel
{
    enum IceStatusKind { Accumulating, Holding, Shedding, None };

    class IceTrackerViewModel : BaseViewModel
    {
        public bool HasIce { get => Get<bool>(); set => Set(value); }
        public IceStatusKind Status { get => Get<IceStatusKind>(); set => Set(value); }

        public IceTrackerViewModel(AppViewModel appVm)
        {
            long ctr = 0;
            double lastIceValue = 0;
            appVm.DataArrived += (newData) =>
            {
                HasIce = newData.structural_ice_pct > 0;
                if (!HasIce)
                {
                    Status = IceStatusKind.None;
                }

                if (++ctr % 10 == 0)
                {
                    var nextIce = newData.structural_ice_pct;

                    if (HasIce)
                    {
                        if (nextIce == lastIceValue)
                        {
                            Status = IceStatusKind.Holding;
                        }
                        else if (nextIce > lastIceValue)
                        {
                            Status = IceStatusKind.Accumulating;
                        }
                        else
                        {
                            Status = IceStatusKind.Shedding;
                        }
                    }

                    lastIceValue = newData.structural_ice_pct;
                }
            };
        }
    }
}
