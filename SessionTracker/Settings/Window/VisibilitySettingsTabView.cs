using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class VisibilitySettingsTabView : View
    {
        public VisibilitySettingsTabView(SettingService settingService)
        {
            _settingService = settingService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);

            var visibilitySectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("UI Visibility", _rootFlowPanel);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.UiVisibilityKeyBindingSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.UiIsVisibleSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.WindowIsVisibleOutsideOfWvwAndSpvpSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.WindowIsVisibleInSpvpSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.WindowIsVisibleInWvwSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.WindowIsVisibleOnWorldMapSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, buildPanel.Width, _settingService.WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting);
        }
        
        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
    }
}