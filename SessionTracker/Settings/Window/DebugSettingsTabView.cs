using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Settings;
using SessionTracker.Controls;
using SessionTracker.Services;
using SessionTracker.Settings.SettingEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using static Blish_HUD.ContentService;

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

            ControlFactory.CreateHintLabel(
               _rootFlowPanel,
               "You are running a beta version of this module if you can see this tab.\n" +
               "Do not change those settings. They will not speed up stat updates or improve\n" +
               "your user experience. This tab just helps the developer to speed up testing this module. :-)\n");

            var debugApiIntervalSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug API interval", _rootFlowPanel);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _settingService.DebugApiIntervalEnabledSetting);
            ControlFactory.CreateSetting(debugApiIntervalSectionFlowPanel, _settingService.DebugApiIntervalValueSetting);
            CreateApiIntervallValueLabel(debugApiIntervalSectionFlowPanel);

            var debugDateTimeSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug DateTime", _rootFlowPanel);
            ControlFactory.CreateSetting(debugDateTimeSectionFlowPanel, _settingService.DebugDateTimeEnabledSetting);
            CreateDateTimeTextBoxAndButton(debugDateTimeSectionFlowPanel, _settingService.DebugDateTimeEnabledSetting);
        }

        private void CreateDateTimeTextBoxAndButton(Container parent, SettingEntry<bool> debugDateTimeEnabledSetting)
        {
            var debugDateTimeFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            var dateTimeTextBox = new TextBox()
            {
                Text = _settingService.DebugDateTimeValueSetting.Value,
                BasicTooltipText = "new debug UTC for date time mocking. mainly for reset",
                Width = 200,
                Parent = debugDateTimeFlowPanel
            };

            var updateDateTimeButton = new StandardButton
            {
                BasicTooltipText = "click to apply as new debug UTC",
                Width = 210,
                Parent = debugDateTimeFlowPanel
            };

            updateDateTimeButton.Click += (s, e) =>
            {
                if (DateTime.TryParse(dateTimeTextBox.Text, out DateTime mockedDateTimeUtc))
                    DateTimeService.UtcNow = mockedDateTimeUtc;
            };

            debugDateTimeEnabledSetting.SettingChanged += (s, o) => updateDateTimeButton.Enabled = debugDateTimeEnabledSetting.Value;
            updateDateTimeButton.Enabled = debugDateTimeEnabledSetting.Value;

            var resetDateTimelabelDict = new Dictionary<AutomaticSessionReset, Label>();
            var automaticSessionResetsWithDateTime = Enum.GetValues(typeof(AutomaticSessionReset))
                .Cast<AutomaticSessionReset>()
                .Where(a => a != AutomaticSessionReset.Never && a != AutomaticSessionReset.OnModuleStart)
                .ToList();

            foreach (var automaticSessionReset in automaticSessionResetsWithDateTime)
                resetDateTimelabelDict[automaticSessionReset] = new Label
                {
                    Text = automaticSessionReset.ToString(),
                    AutoSizeHeight = true,
                    AutoSizeWidth = true,
                    Parent = debugDateTimeFlowPanel
                };

            dateTimeTextBox.TextChanged += (s, o) =>
            {
                _settingService.DebugDateTimeValueSetting.Value = dateTimeTextBox.Text;
                UpdateDateTimeButtonText(updateDateTimeButton, dateTimeTextBox.Text); // parameter to prevent that method is moved above label creation -> crash
                UpdateResetDateTimeLabels(resetDateTimelabelDict, automaticSessionResetsWithDateTime, dateTimeTextBox.Text);
            };

            UpdateDateTimeButtonText(updateDateTimeButton, dateTimeTextBox.Text);
            UpdateResetDateTimeLabels(resetDateTimelabelDict, automaticSessionResetsWithDateTime, dateTimeTextBox.Text);
        }

        private static void UpdateResetDateTimeLabels(
            Dictionary<AutomaticSessionReset, Label> resetDateTimelabelDict, 
            List<AutomaticSessionReset> automaticSessionResetsWithDateTime, 
            string dateTimeTextBoxText)
        {
            DateTime dateTimeUtc;
            if (!DateTime.TryParse(dateTimeTextBoxText, out dateTimeUtc))
                return;

            foreach (var automaticSessionReset in automaticSessionResetsWithDateTime)
            {
                var dateTimeText = ResetService.GetNextResetDateTimeUtc(automaticSessionReset, dateTimeUtc).ToString(_debugDateFormat);
                resetDateTimelabelDict[automaticSessionReset].Text = $"{dateTimeText} {automaticSessionReset}";
            }
        }

        private void UpdateDateTimeButtonText(StandardButton button, string dateTimeText)
        {
            if (DateTime.TryParse(dateTimeText, out DateTime mockedDateTimeUtc))
                button.Text = $"{mockedDateTimeUtc.ToString(_debugDateFormat)}";
        }

        private void CreateApiIntervallValueLabel(FlowPanel debugSectionFlowPanel)
        {
            _apiIntervalInMillisecondsLabel = new Label()
            {
                AutoSizeHeight = true,
                AutoSizeWidth = true,
                Parent = debugSectionFlowPanel
            };

            OnDebugApiIntervalValueSettingChanged();
            _settingService.DebugApiIntervalValueSetting.SettingChanged += OnDebugApiIntervalValueSettingChanged;
        }

        private void OnDebugApiIntervalValueSettingChanged(object sender = null, Blish_HUD.ValueChangedEventArgs<int> e = null)
        {
            _apiIntervalInMillisecondsLabel.Text = $"api interval: {_settingService.DebugApiIntervalValueSetting.Value} ms";
        }

        private readonly SettingService _settingService;
        private FlowPanel _rootFlowPanel;
        private Label _apiIntervalInMillisecondsLabel;
        private const string _debugDateFormat = "ddd dd:MM:yyyy HH:mm:ss";
    }
}