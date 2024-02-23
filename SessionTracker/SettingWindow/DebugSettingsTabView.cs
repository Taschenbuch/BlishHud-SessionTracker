using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.OtherServices;

namespace SessionTracker.SettingsWindow
{
    public class DebugSettingsTabView : View
    {
        public DebugSettingsTabView(Services services)
        {
            _services = services;
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
            _services.DateTimeService.CreateDateTimeDebugPanel(_rootFlowPanel);
        }

        protected override void Unload()
        {
            _services.SettingService.DebugApiIntervalValueSetting.SettingChanged -= OnDebugApiIntervalValueSettingChanged;
            _services.UpdateLoop.StateChanged -= UpdateLoopStateChanged;
        }

        private void CreateUpdateLoopStateDebugPanel()
        {
            var updateLoopStateFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug update loop state", _rootFlowPanel);
            _updateLoopStateLabel = new Label()
            {
                Text = _services.UpdateLoop.State.ToString(),
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

            _services.UpdateLoop.StateChanged += UpdateLoopStateChanged;
        }

        private void UpdateLoopStateChanged(object sender, System.EventArgs e)
        {
            _updateLoopStateLabel.Text = _services.UpdateLoop.State.ToString();
        }

        private void CreateApiIntervalDebugPanel()
        {
            var debugApiIntervalSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug API interval", _rootFlowPanel);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _services.SettingService.DebugApiIntervalEnabledSetting);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _services.SettingService.DebugApiIntervalValueSetting);
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
            _services.SettingService.DebugApiIntervalValueSetting.SettingChanged += OnDebugApiIntervalValueSettingChanged;
        }

        private void OnDebugApiIntervalValueSettingChanged(object sender = null, Blish_HUD.ValueChangedEventArgs<int> e = null)
        {
            _apiIntervalInMillisecondsLabel.Text = $"{_services.SettingService.DebugApiIntervalValueSetting.Value} ms";
        }

        private readonly Services _services;
        private FlowPanel _rootFlowPanel;
        private Label _apiIntervalInMillisecondsLabel;
        private Label _updateLoopStateLabel;
    }
}