using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Models;
using SessionTracker.Properties;
using SessionTracker.Settings.SettingEntries;
using SessionTracker.Value.Text;

namespace SessionTracker.Value.Tooltip
{
    public class SummaryTooltipTextService
    {
        public SummaryTooltipTextService(Model model, SettingService settingService)
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
            var sessionValueText = CreateValueText(entry.Value.Session, entry.CurrencyId);
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
            // summary tooltip makes no sense for KDRs. Thus they get a shorter tooltip.
            if (entry.Id == EntryId.WVW_KDR || entry.Id == EntryId.PVP_KDR)
                return entry.GetNameAndDescription();

            var sessionValuePerHour     = GetValuePerHourAsInteger(entry.Value.Session);
            var sessionValuePerHourText = CreateValueText(sessionValuePerHour, entry.CurrencyId);
            var totalValueText          = CreateValueText(entry.Value.Total, entry.CurrencyId);

            return $"{entry.GetNameAndDescription()}\n" +
                   $"\n" +
                   $"== TOTAL ==\n" +
                   $"{totalValueText} {entry.LabelText.Localized}\n" +
                   $"\n== {Localization.SummaryTooltip_HeaderCurrentSession} ==\n" +
                   $"{sessionValuePerHourText} {entry.LabelText.Localized} / {Localization.SummaryTooltip_Hour}\n" +
                   $"{_model.SessionDuration:hh':'mm} {Localization.SummaryTooltip_HoursMinutes}\n" +
                   $"\n" +
                   $"{Localization.SummaryTooltip_historyTimeColumnTitle} | {entry.LabelText.Localized}\n" +
                   $"{string.Join("\n", entry.SessionHistory)}";
        }

        private int GetValuePerHourAsInteger(int value)
        {
            if (value == 0)
                return 0;

            // - the value == 0 guard should be actually enough to catch the session start/reset case. Because on session start all values are 0.
            // - this prevents division by 0 and cases where a value is divided by a very small session duration which would create unrealistic high values
            var sessionJustStarted = _model.SessionDuration.TotalHours < ONE_MINUTE_IN_HOURS; 
            if (sessionJustStarted) 
                return 0;

            // int: because everything that has less than 1/h is not interesting to track anyway.
            // showing decimals would make the more common >>1/h values harder to read. (e.g. 12345 vs 12345.67)
            return (int) (value / _model.SessionDuration.TotalHours);
        }

        private string CreateValueText(int value, int entryCurrencyId)
        {
            return entryCurrencyId == CurrencyIds.COIN_IN_COPPER
                ? ValueTextService.CreateCoinValueText(value, _settingService.CoinDisplayFormatSetting.Value)
                : value.To0DecimalPlacesCulturedString();
        }

        private readonly Model _model;
        private readonly SettingService _settingService;
        private const int MAX_NUMBER_OF_ENTRIES = 12;
        private const double ONE_MINUTE_IN_HOURS = 0.017;
    }
}