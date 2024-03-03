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
            get => Stat.IsVisible;
            set
            {
                if (Stat.IsVisible == value)
                    return; 

                Stat.IsVisible = value;
                IsSelectedChanged?.Invoke(this, EventArgs.Empty);
                _services.Model.UiHasToBeUpdated = true;
            }
        }

        private readonly Services _services;
    }
}
