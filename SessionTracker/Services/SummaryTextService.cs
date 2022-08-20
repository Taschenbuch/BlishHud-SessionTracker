using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Models;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Services
{
    public class SummaryTextService
    {
        public SummaryTextService(SettingService settingService)
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
                : sessionValuePerHour.DoubleToCulturedStringWith1DecimalPlace();
        }

        private string CreateSummaryText(Entry entry, TimeSpan sessionDuration)
        {
            var sessionValuePerHourText = CreateSessionValuePerHourText(entry, sessionDuration);

            return $"== TOTAL ==\n" +
                   $"{entry.Value.Total.IntToCulturedString()} {entry.LabelText.Localized}\n" +
                   $"\n== CURRENT SESSION ==\n" + // todo übersetzen oder lassen?
                   $"{sessionValuePerHourText} {entry.LabelText.Localized}/hour\n" +
                   $"{sessionDuration:hh':'mm} hour : minute\n" + // todo  hour : minute
                   $"\n" +
                   $"time | {entry.LabelText.Localized}\n" + // todo time
                   $"{string.Join("\n", entry.SessionHistory)}";
        }

        private void InsertNewHistoryEntryAtBeginning(Entry entry)
        {
            var sessionValueText = entry.CurrencyId == CurrencyIds.COIN_IN_COPPER
                ? ValueTextService.CreateCoinValueText(entry.Value.Session, _settingService.CoinDisplayFormatSetting.Value)
                : entry.Value.Session.IntToCulturedString();

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