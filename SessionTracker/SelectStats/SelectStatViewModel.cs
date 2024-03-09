using SessionTracker.Models;
using SessionTracker.OtherServices;
using System;

namespace SessionTracker.SelectStats
{
    public class SelectStatViewModel
    {
        public SelectStatViewModel(Stat stat, Services services)
        {
            Stat = stat;
            _services = services;
        }

        public Stat Stat { get; }
        public event EventHandler IsSelectedChanged;
        public bool IsSelected
        {
            get => Stat.IsSelectedByUser;
            set
            {
                if (Stat.IsSelectedByUser == value)
                    return; 

                Stat.IsSelectedByUser = value;
                IsSelectedChanged?.Invoke(this, EventArgs.Empty);
                _services.Model.UiHasToBeUpdated = true;
            }
        }

        private readonly Services _services;
    }
}
