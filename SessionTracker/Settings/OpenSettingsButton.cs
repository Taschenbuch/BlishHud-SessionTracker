using Blish_HUD.Controls;
using SessionTracker.Settings.Window;

namespace SessionTracker.Settings
{
    public class OpenSettingsButton : StandardButton
    {
        public OpenSettingsButton(SettingsWindowService settingsWindowService)
        {
            _settingsWindowService = settingsWindowService;

            Text             = "Open Settings";
            BasicTooltipText = "Open session tracker module settings.";
            Width            = 150;

            Click += OnSettingsButtonClick;
        }

        protected override void DisposeControl()
        {
            Click -= OnSettingsButtonClick;
            base.DisposeControl();
        }

        private void OnSettingsButtonClick(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            _settingsWindowService.ShowWindowAndSelectStatsTab();
        }

        private readonly SettingsWindowService _settingsWindowService;
    }
}