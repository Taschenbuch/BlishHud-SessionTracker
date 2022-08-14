using Blish_HUD;
using Blish_HUD.Controls;
using SessionTracker.Settings;

namespace SessionTracker.Controls
{
    public class HintFlowPanel : FlowPanel
    {
        public HintFlowPanel(SettingsWindowService settingsWindowService, SettingService settingService, Container parent)
        {
            Parent                 = parent;
            _parent                = parent;
            _settingService        = settingService;
            _settingsWindowService = settingsWindowService;

            var hintText = "No stats selected.\n" +
                           "Select the stats you want to see\n" +
                           "in the session tracker module settings. :)";

            _hintLabel = new Label()
            {
                Text             = hintText,
                BasicTooltipText = hintText,
                ShowShadow       = true,
                AutoSizeHeight   = true,
                AutoSizeWidth    = true,
                Parent           = this,
            };

            _settingsButton = new OpenSettingsButton(_settingsWindowService, this);

            OnFontSizeIndexSettingChanged(null, null);
            settingService.FontSizeIndexSetting.SettingChanged += OnFontSizeIndexSettingChanged;
        }

        public void SetVisibility(bool isVisible)
        {
            // those are overwritten below. Do not replace with Visibility = isVisible.
            // Because that would only hide it and leave a empty whole in the parent container
            if (isVisible)
                Show();
            else
                Hide();
        }

        public override void Show()
        {
            Parent = _parent;
            base.Show();
        }

        public override void Hide()
        {
            Parent = null;
            base.Hide();
        }

        private void OnSettingsButtonClick(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            _settingsWindowService.ShowWindow();
        }

        private void OnFontSizeIndexSettingChanged(object sender, ValueChangedEventArgs<int> e)
        {
            _hintLabel.Font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];
        }


        protected override void DisposeControl()
        {
            _settingService.FontSizeIndexSetting.SettingChanged -= OnFontSizeIndexSettingChanged;
            _settingsButton?.Dispose();
            base.DisposeControl();
        }

        private readonly SettingService _settingService;
        private readonly SettingsWindowService _settingsWindowService;
        private readonly Container _parent;
        private readonly Label _hintLabel;
        private readonly OpenSettingsButton _settingsButton;
    }
}