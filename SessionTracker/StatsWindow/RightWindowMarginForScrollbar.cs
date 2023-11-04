using Blish_HUD;
using Blish_HUD.Controls;
using SessionTracker.SettingEntries;

namespace SessionTracker.StatsWindow
{
    public class RightWindowMarginForScrollbar : Label
    {
        public RightWindowMarginForScrollbar(Container parent, SettingService settingService)
        {
            _settingService = settingService;
            Parent = parent;

            settingService.RightMarginForScrollbarSetting.SettingChanged += RightMarginForScrollbarSettingChanged;
            RightMarginForScrollbarSettingChanged();
        }

        protected override void DisposeControl()
        {
            _settingService.RightMarginForScrollbarSetting.SettingChanged -= RightMarginForScrollbarSettingChanged;
            base.DisposeControl();
        }

        private void RightMarginForScrollbarSettingChanged(object sender = null, ValueChangedEventArgs<int> e = null)
        {
            Width = _settingService.RightMarginForScrollbarSetting.Value;
        }

        private readonly SettingService _settingService;
    }
}
