using System.Diagnostics;
using System.Linq;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using SessionTracker.AutomaticReset;
using SessionTracker.Controls;
using SessionTracker.Reset;
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
            CreateAutomaticSessionResetSetting(generalSectionFlowPanel, _settingService.AutomaticSessionResetSetting);
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

        // Warning: this will not update the selectedItem when the setting Value is changed. Not sure if that could cause an infinite loop
        private void CreateAutomaticSessionResetSetting(FlowPanel parent, SettingEntry<AutomaticSessionReset> automaticSessionResetSetting)
        {
            var settingFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                BasicTooltipText = automaticSessionResetSetting.GetDescriptionFunc(),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            new Label // settingLabel
            {
                Text = automaticSessionResetSetting.GetDisplayNameFunc(),
                BasicTooltipText = automaticSessionResetSetting.GetDescriptionFunc(),
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Parent = settingFlowPanel
            };

            var settingDropDown = new Dropdown()
            {
                BasicTooltipText = automaticSessionResetSetting.GetDescriptionFunc(),
                Width = 400,
                Parent = settingFlowPanel,
            };

            var dropDownTextDict = ResetDropDownService.GetDropDownTextsForAutomaticSessionResetSetting();

            foreach (var dropDownText in dropDownTextDict.Values)
                settingDropDown.Items.Add(dropDownText);

            settingDropDown.SelectedItem = dropDownTextDict[_settingService.AutomaticSessionResetSetting.Value];
            settingDropDown.ValueChanged += (s, e) => 
            {
                _settingService.AutomaticSessionResetSetting.Value = dropDownTextDict.First(d => d.Value == e.CurrentValue).Key;
            };
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