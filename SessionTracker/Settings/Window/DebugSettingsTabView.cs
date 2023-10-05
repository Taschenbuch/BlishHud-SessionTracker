using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.DateTimeUtcNow;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class DebugSettingsTabView : View
    {
        public DebugSettingsTabView(SettingService settingService, DateTimeService dateTimeService)
        {
            _settingService = settingService;
            _dateTimeService = dateTimeService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);

            ControlFactory.CreateHintLabel(
               _rootFlowPanel,
               "You are running a beta version of this module if you can see this tab.\n" +
               "Do not change those settings. They will not speed up stat updates or improve\n" +
               "your user experience. This tab just helps the developer to speed up testing this module. :-)\n");

            var debugApiIntervalSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug API interval", _rootFlowPanel);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _settingService.DebugApiIntervalEnabledSetting);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _settingService.DebugApiIntervalValueSetting);
            CreateApiIntervallValueLabel(debugApiIntervalSectionFlowPanel);

            _dateTimeService.CreateDateTimeDebugPanel(_rootFlowPanel);
        }

        protected override void Unload()
        {
            _settingService.DebugApiIntervalValueSetting.SettingChanged -= OnDebugApiIntervalValueSettingChanged;
        }

        private void CreateApiIntervallValueLabel(Container parent)
        {

            _apiIntervalInMillisecondsLabel = new Label()
            {
                Left = 300,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Parent = new Panel()
                {
                    WidthSizingMode = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    Parent = parent,
                }
        };

            OnDebugApiIntervalValueSettingChanged();
            _settingService.DebugApiIntervalValueSetting.SettingChanged += OnDebugApiIntervalValueSettingChanged;
        }

        private void OnDebugApiIntervalValueSettingChanged(object sender = null, Blish_HUD.ValueChangedEventArgs<int> e = null)
        {
            _apiIntervalInMillisecondsLabel.Text = $"{_settingService.DebugApiIntervalValueSetting.Value} ms";
        }

        private readonly SettingService _settingService;
        private readonly DateTimeService _dateTimeService;
        private FlowPanel _rootFlowPanel;
        private Label _apiIntervalInMillisecondsLabel;
    }
}