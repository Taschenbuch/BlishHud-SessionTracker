using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.DateTimeUtcNow;
using SessionTracker.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class DebugSettingsTabView : View
    {
        public DebugSettingsTabView(SettingService settingService, DateTimeService dateTimeService, UpdateLoop updateLoop)
        {
            _settingService  = settingService;
            _dateTimeService = dateTimeService;
            _updateLoop      = updateLoop;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);

            ControlFactory.CreateHintLabel(
               _rootFlowPanel,
               "You are running a beta version of this module if you can see this tab.\n" +
               "Do not change those settings. They will not speed up stat updates or improve\n" +
               "your user experience. This tab just helps the developer to speed up testing this module. :-)\n");
            CreateApiIntervalDebugPanel();
            CreateUpdateLoopStateDebugPanel();
            _dateTimeService.CreateDateTimeDebugPanel(_rootFlowPanel);
        }

        protected override void Unload()
        {
            _settingService.DebugApiIntervalValueSetting.SettingChanged -= OnDebugApiIntervalValueSettingChanged;
            _updateLoop.StateChanged -= UpdateLoopStateChanged;
        }

        private void CreateUpdateLoopStateDebugPanel()
        {
            var updateLoopStateFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug update loop state", _rootFlowPanel);
            _updateLoopStateLabel = new Label()
            {
                Text = _updateLoop.State.ToString(),
                Left = 5,
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Parent = new Panel()
                {
                    WidthSizingMode = SizingMode.AutoSize,
                    HeightSizingMode = SizingMode.AutoSize,
                    Parent = updateLoopStateFlowPanel,
                }
            };

            _updateLoop.StateChanged += UpdateLoopStateChanged;
        }

        private void UpdateLoopStateChanged(object sender, System.EventArgs e)
        {
            _updateLoopStateLabel.Text = _updateLoop.State.ToString();
        }

        private void CreateApiIntervalDebugPanel()
        {
            var debugApiIntervalSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug API interval", _rootFlowPanel);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _settingService.DebugApiIntervalEnabledSetting);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _settingService.DebugApiIntervalValueSetting);
            CreateApiIntervallValueLabel(debugApiIntervalSectionFlowPanel);
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
        private readonly UpdateLoop _updateLoop;
        private FlowPanel _rootFlowPanel;
        private Label _apiIntervalInMillisecondsLabel;
        private Label _updateLoopStateLabel;
    }
}