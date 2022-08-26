﻿using Blish_HUD.Controls;
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
            ControlFactory.CreateSetting(debugSectionFlowPanel, buildPanel.Width, _settingService.DebugApiIntervalEnabledSetting);
            ControlFactory.CreateSetting(debugSectionFlowPanel, buildPanel.Width, _settingService.DebugApiIntervalValueSetting);

            _apiIntervalInMillisecondsLabel = new Label()
            {
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                Parent = debugSectionFlowPanel
            };

            OnDebugApiIntervalValueSettingChanged(null, null);
            _settingService.DebugApiIntervalValueSetting.SettingChanged += OnDebugApiIntervalValueSettingChanged;
        }

        private void OnDebugApiIntervalValueSettingChanged(object sender, Blish_HUD.ValueChangedEventArgs<int> e)
        {
            _apiIntervalInMillisecondsLabel.Text = $"api interval: {_settingService.DebugApiIntervalValueSetting.Value} ms";
        }

        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
        private Label _apiIntervalInMillisecondsLabel;
    }
}