using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace P3DStreamer.ViewModel
{
    class MasterCautionViewModel : BaseComponentViewModel
    {
        public ICommand Dismiss { get; }

        List<EICASMessageViewModel> m_active = new List<EICASMessageViewModel>();

        public MasterCautionViewModel(AppViewModel appVm, EICASViewModel eicas)
        {
            Dismiss = new RelayCommand(() =>
            {
                m_active.Clear();
                IsActive = false;
            });

            foreach (var msg in eicas.Messages)
            {
                msg.PropertyChanged += (_, e) =>
                 {
                     if (e.PropertyName == nameof(msg.IsApplicable))
                     {
                         if (msg.IsApplicable && msg.Kind != EICASMessageKind.Advisory && !m_active.Contains(msg))
                         {
                             m_active.Add(msg);
                         }
                         else if (!msg.IsApplicable && m_active.Contains(msg))
                         {
                             m_active.Remove(msg);
                         }
                         IsActive = m_active.Any();
                     }
                 };
            }
        }
    }
}
