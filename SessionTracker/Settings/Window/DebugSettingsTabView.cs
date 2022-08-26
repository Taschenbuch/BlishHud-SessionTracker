using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class DebugSettingsTabView : View
    {
        public DebugSettingsTabView(SettingService settingService)
        {
            _settingService = settingService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);

            var debugSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug (developer only settings)", _rootFlowPanel);
            ControlFactory.CreateSetting(debugSectionFlowPanel, buildPanel.Width, _settingService.DebugModeIsEnabledSetting);
        }
        
        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
    }
}