using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using SessionTracker.AutomaticReset;
using SessionTracker.Controls;
using SessionTracker.Reset;
using SessionTracker.SettingEntries;

namespace SessionTracker.SettingsWindow
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
            CreateMinutesAfterModuleShutDownSetting(generalSectionFlowPanel, _settingService.MinutesUntilResetAfterModuleShutdownSetting, _settingService.AutomaticSessionResetSetting);
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

        private void CreateMinutesAfterModuleShutDownSetting(
            Container parent, 
            SettingEntry<int> minutesAfterModuleShutdownUntilResetSetting, 
            SettingEntry<AutomaticSessionReset> automaticSessionResetSetting)
        {
            _resetMinutesPanel = new Panel
            {
                BasicTooltipText = minutesAfterModuleShutdownUntilResetSetting.GetDescriptionFunc(),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            var minutesDropDown = new Dropdown
            {
                BasicTooltipText = minutesAfterModuleShutdownUntilResetSetting.GetDescriptionFunc(),
                Location = new Point(5, 0),
                Width = 60,
                Parent = _resetMinutesPanel
            };

            new Label // minutes label
            {
                BasicTooltipText = minutesAfterModuleShutdownUntilResetSetting.GetDescriptionFunc(),
                Location = new Point(minutesDropDown.Right + 5, 4),
                Text = minutesAfterModuleShutdownUntilResetSetting.GetDisplayNameFunc(),
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = _resetMinutesPanel,
            };

            var dropDownValues = new List<int> { 15, 30, 45, 60, 90, 120, 180, 240, 360, 480, 600, 720, 840, 960, 1080, 1200}.Select(m => m.ToString());
            foreach (string dropDownValue in dropDownValues)
                minutesDropDown.Items.Add(dropDownValue);

            minutesDropDown.SelectedItem = minutesAfterModuleShutdownUntilResetSetting.Value.ToString();
            minutesDropDown.ValueChanged += (s, o) =>
            {
                minutesAfterModuleShutdownUntilResetSetting.Value = int.Parse(minutesDropDown.SelectedItem);
            };

            automaticSessionResetSetting.SettingChanged += AutomaticSessionResetSettingChanged;
            AutomaticSessionResetSettingChanged();
        }

        private void AutomaticSessionResetSettingChanged(object sender = null, ValueChangedEventArgs<AutomaticSessionReset> e = null)
        {
            if (_settingService.AutomaticSessionResetSetting.Value == AutomaticSessionReset.MinutesAfterModuleShutdown)
                _resetMinutesPanel.Show();
            else
                _resetMinutesPanel.Hide();
        }

        protected override void Unload()
        {
            _settingService.AutomaticSessionResetSetting.SettingChanged -= AutomaticSessionResetSettingChanged;
        }

        // Warning: this will not update the selectedItem when the setting Value is changed. Not sure if that could cause an infinite loop
        private void CreateAutomaticSessionResetSetting(FlowPanel parent, SettingEntry<AutomaticSessionReset> automaticSessionResetSetting)
        {
            var automaticResetPanel = new Panel()
            {
                BasicTooltipText = automaticSessionResetSetting.GetDescriptionFunc(),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            var automaticResetLabel = new Label
            {
                Text = automaticSessionResetSetting.GetDisplayNameFunc(),
                BasicTooltipText = automaticSessionResetSetting.GetDescriptionFunc(),
                Location = new Point(5, 4),
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Parent = automaticResetPanel
            };

            var automaticResetDropDown = new Dropdown()
            {
                BasicTooltipText = automaticSessionResetSetting.GetDescriptionFunc(),
                Location = new Point(automaticResetLabel.Right + 5, 0),
                Width = 450,
                Parent = automaticResetPanel,
            };

            var dropDownTextDict = ResetDropDownService.GetDropDownTextsForAutomaticSessionResetSetting();

            foreach (var dropDownText in dropDownTextDict.Values)
                automaticResetDropDown.Items.Add(dropDownText);

            automaticResetDropDown.SelectedItem = dropDownTextDict[_settingService.AutomaticSessionResetSetting.Value];
            automaticResetDropDown.ValueChanged += (s, e) => 
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
        private Panel _resetMinutesPanel;
    }
}