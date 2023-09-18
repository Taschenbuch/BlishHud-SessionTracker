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
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.ValueDisplayFormatSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.LabelTypeSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.UiHeightIsFixedSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.UiHeightSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.FontSizeIndexSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.StatTitlePaddingSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.TitleLabelColorSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.ValueLabelColorSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.BackgroundColorSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.BackgroundOpacitySetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.DragWindowWithMouseIsEnabledSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.WindowCanBeClickedThroughSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.CornerIconIsVisibleSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.CoinDisplayFormatSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.HideStatsWithValueZeroSetting);
            ControlFactory.CreateSetting(generalSectionFlowPanel, _settingService.ScrollbarFixDelay);
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