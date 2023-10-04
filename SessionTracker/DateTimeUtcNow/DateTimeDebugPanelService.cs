using Blish_HUD.Controls;
using Blish_HUD.Settings;
using SessionTracker.AutomaticReset;
using System.Collections.Generic;
using System;
using SessionTracker.Reset;
using System.Linq;
using SessionTracker.Controls;

namespace SessionTracker.DateTimeUtcNow
{
    public class DateTimeDebugPanelService
    {
        public static void CreateDateTimeDebugPanel(Container parent, SettingEntry<bool> debugDateTimeEnabledSetting, SettingEntry<string> debugDateTimeValueSetting)
        {
            var debugDateTimeFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Debug DateTime", parent);
            ControlFactory.CreateSetting(debugDateTimeFlowPanel, debugDateTimeEnabledSetting);

            var dateTimeTextBox = new TextBox()
            {
                Text = debugDateTimeValueSetting.Value,
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
                debugDateTimeValueSetting.Value = dateTimeTextBox.Text;
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
            if (!DateTime.TryParse(dateTimeTextBoxText, out var dateTimeUtc))
                return;

            foreach (var automaticSessionReset in automaticSessionResetsWithDateTime)
            {
                var dateTimeText = ResetService.GetNextResetDateTimeUtc(automaticSessionReset, dateTimeUtc).ToString(DEBUG_DATE_FORMAT);
                resetDateTimelabelDict[automaticSessionReset].Text = $"{dateTimeText} {automaticSessionReset}";
            }
        }

        private static void UpdateDateTimeButtonText(StandardButton button, string dateTimeText)
        {
            if (DateTime.TryParse(dateTimeText, out DateTime mockedDateTimeUtc))
                button.Text = $"{mockedDateTimeUtc.ToString(DEBUG_DATE_FORMAT)}";
        }

        private const string DEBUG_DATE_FORMAT = "ddd dd:MM:yyyy HH:mm:ss";
    }
}
