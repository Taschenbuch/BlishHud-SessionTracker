using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.SettingEntries;
using System;

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

            var visibilitySectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Window Visibility", _rootFlowPanel);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.UiVisibilityKeyBindingSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.UiIsVisibleSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.WindowIsVisibleOutsideOfWvwAndSpvpSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.WindowIsVisibleInSpvpSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.WindowIsVisibleInWvwSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.WindowIsVisibleOnWorldMapSetting);
            ControlFactory.CreateSetting(visibilitySectionFlowPanel, _settingService.WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting);
        }
        
        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
    }
}