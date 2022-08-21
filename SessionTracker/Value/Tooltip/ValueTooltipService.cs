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
        public ValueTooltipService(Model model, SettingService settingService)
        {
            _model          = model;
            _settingService = settingService;
        }

        public string ResetAndReturnSummaryText(Entry entry)
        {
            entry.SessionHistory.Clear();
            InsertNewHistoryEntryAtBeginning(entry);

            return CreateSummaryText(entry);
        }

        public string UpdateAndReturnSummaryText(Entry entry)
        {
            InsertNewHistoryEntryAtBeginning(entry);

            if (HistoryIsTooLong(entry.SessionHistory.Count))
                RemoveOldestHistoryEntry(entry.SessionHistory);

            return CreateSummaryText(entry);
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

        private string CreateSummaryText(Entry entry)
        {
            var sessionValuePerHourText = CreateSessionValuePerHourText(entry, _model.SessionDuration);

            return $"== TOTAL ==\n" +
                   $"{entry.Value.Total.To0DecimalPlacesCulturedString()} {entry.LabelText.Localized}\n" +
                   $"\n== {Localization.SummaryTooltip_HeaderCurrentSession} ==\n" +
                   $"{sessionValuePerHourText} {entry.LabelText.Localized} / {Localization.SummaryTooltip_Hour}\n" +
                   $"{_model.SessionDuration:hh':'mm} {Localization.SummaryTooltip_HoursMinutes}\n" +
                   $"\n" +
                   $"{Localization.SummaryTooltip_historyTimeColumnTitle} | {entry.LabelText.Localized}\n" +
                   $"{string.Join("\n", entry.SessionHistory)}";
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

        private readonly Model _model;
        private readonly SettingService _settingService;
        private const int MAX_NUMBER_OF_ENTRIES = 12;
    }
}