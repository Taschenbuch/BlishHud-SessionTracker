using System.Diagnostics;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using SessionTracker.Controls;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class GeneralSettingsTabView : View
    {
        public GeneralSettingsTabView(SettingService settingService)
        {
            _settingService = settingService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);

            var generalSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("General", _rootFlowPanel);
            CreatePatchNotesButton(generalSectionFlowPanel);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.LabelTypeSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.UiHeightIsFixedSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.UiHeightSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.BackgroundOpacitySetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.FontSizeIndexSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.StatTitlePaddingSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.TitleLabelColorSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.ValueLabelColorSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.SessionValuesAreVisibleSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.TotalValuesAreVisibleSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.DragWindowWithMouseIsEnabledSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.CornerIconIsVisibleSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.CoinDisplayFormatSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.ScrollbarFixDelay);
#if DEBUG
            ControlFactory.CreateSetting(generalSectionFlowPanel, buildPanel.Width, _settingService.DebugModeIsEnabledSetting);
#endif
        }

        private static void CreatePatchNotesButton(Container parent)
        {
            var patchNotesButton = new StandardButton
            {
                Text = "Patch notes",
                BasicTooltipText = "Show patch notes in your default web browser.",
                Parent = parent
            };

            patchNotesButton.Click += (s, e) =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://pkgs.blishhud.com/ecksofa.sessiontracker.html",
                    UseShellExecute = true
                });
            };
        }
        
        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
    }
}