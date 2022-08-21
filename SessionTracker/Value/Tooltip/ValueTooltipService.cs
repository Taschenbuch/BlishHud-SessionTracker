using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Models;
using SessionTracker.Properties;
using SessionTracker.Settings.SettingEntries;
using SessionTracker.Value.Text;

namespace SessionTracker.Value.Tooltip
{
    public class ValueTooltipService
    {
        public ValueTooltipService(SettingService settingService)
        {
            _settingService = settingService;
        }

        public string ResetAndReturnSummaryText(Entry entry)
        {
            entry.SessionHistory.Clear();
            InsertNewHistoryEntryAtBeginning(entry);

            _startTime = DateTime.Now;
            return CreateSummaryText(entry, new TimeSpan(0));
        }

        public string UpdateAndReturnSummaryText(Entry entry)
        {
            InsertNewHistoryEntryAtBeginning(entry);

            if (HistoryIsTooLong(entry.SessionHistory.Count))
                RemoveOldestHistoryEntry(entry.SessionHistory);

            var sessionDuration = DateTime.Now - _startTime;

            return CreateSummaryText(entry, sessionDuration);
        }

        private string CreateSessionValuePerHourText(Entry entry, TimeSpan sessionDuration)
        {
            var sessionValuePerHour = sessionDuration.TotalHours == 0
                ? 0
                : entry.Value.Session / sessionDuration.TotalHours;

            return entry.CurrencyId == CurrencyIds.COIN_IN_COPPER
                ? ValueTextService.CreateCoinValueText((int)sessionValuePerHour, _settingService.CoinDisplayFormatSetting.Value)
                : sessionValuePerHour.To0DecimalPlacesCulturedString();
        }

        private string CreateSummaryText(Entry entry, TimeSpan sessionDuration)
        {
            var sessionValuePerHourText = CreateSessionValuePerHourText(entry, sessionDuration);

            return $"== TOTAL ==\n" +
                   $"{entry.Value.Total.To0DecimalPlacesCulturedString()} {entry.LabelText.Localized}\n" +
                   $"\n== {Localization.SummaryTooltip_HeaderCurrentSession} ==\n" +
                   $"{sessionValuePerHourText} {entry.LabelText.Localized} / {Localization.SummaryTooltip_Hour}\n" +
                   $"{sessionDuration:hh':'mm} {Localization.SummaryTooltip_HoursMinutes}\n" +
                   $"\n" +
                   $"{Localization.SummaryTooltip_historyTimeColumnTitle} | {entry.LabelText.Localized}\n" +
                   $"{string.Join("\n", entry.SessionHistory)}";
        }

        private void InsertNewHistoryEntryAtBeginning(Entry entry)
        {
            var sessionValueText = entry.CurrencyId == CurrencyIds.COIN_IN_COPPER
                ? ValueTextService.CreateCoinValueText(entry.Value.Session, _settingService.CoinDisplayFormatSetting.Value)
                : entry.Value.Session.To0DecimalPlacesCulturedString();

            entry.SessionHistory.Insert(0, $"{DateTime.Now:HH:mm} | {sessionValueText}");
        }

        private static bool HistoryIsTooLong(int entriesCount)
        {
            return entriesCount > MAX_NUMBER_OF_ENTRIES;
        }

        private static void RemoveOldestHistoryEntry(List<string> historyEntries)
        {
            historyEntries.Remove(historyEntries.Last());
        }

        private DateTime _startTime;
        private readonly SettingService _settingService;
        private const int MAX_NUMBER_OF_ENTRIES = 12;
    }
}