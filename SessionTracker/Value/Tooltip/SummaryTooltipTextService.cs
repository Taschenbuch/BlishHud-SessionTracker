using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Constants;
using SessionTracker.Models;
using SessionTracker.Properties;
using SessionTracker.SettingEntries;
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

        public string ResetAndReturnSummaryText(Stat stat)
        {
            stat.SessionHistory.Clear();
            InsertNewHistoryEntryAtBeginning(stat);

            return CreateSummaryText(stat);
        }

        public string UpdateAndReturnSummaryText(Stat stat)
        {
            InsertNewHistoryEntryAtBeginning(stat);

            if (HistoryIsTooLong(stat.SessionHistory.Count))
                RemoveOldestHistoryEntry(stat.SessionHistory);

            return CreateSummaryText(stat);
        }

        private void InsertNewHistoryEntryAtBeginning(Stat stat)
        {
            var sessionValueText = CreateValueText(stat.Value.Session, stat.ApiId);
            stat.SessionHistory.Insert(0, $"{DateTime.Now:HH:mm} | {sessionValueText}");
        }

        private static bool HistoryIsTooLong(int entriesCount)
        {
            return entriesCount > MAX_NUMBER_OF_HISTORY_ENTRIES;
        }

        private static void RemoveOldestHistoryEntry(List<string> historyEntries)
        {
            historyEntries.Remove(historyEntries.Last());
        }

        private string CreateSummaryText(Stat stat)
        {
            // summary tooltip makes no sense for KDRs. Thus they get a shorter tooltip.
            if (stat.Id == StatId.WVW_KDR || stat.Id == StatId.PVP_KDR)
                return stat.GetTextWithNameAndDescription();

            var sessionValuePerHour     = GetValuePerHourAsInteger(stat.Value.Session);
            var sessionValuePerHourText = CreateValueText(sessionValuePerHour, stat.ApiId);
            var totalValueText          = CreateValueText(stat.Value.Total, stat.ApiId);

            return $"{stat.GetTextWithNameAndDescription()}\n" +
                   $"\n" +
                   $"== TOTAL ==\n" +
                   $"{totalValueText} {stat.Name.Localized}\n" +
                   $"\n== {Localization.SummaryTooltip_HeaderCurrentSession} ==\n" +
                   $"{sessionValuePerHourText} {stat.Name.Localized} / {Localization.SummaryTooltip_Hour}\n" +
                   $"{_model.SessionDuration.Value:hh':'mm} {Localization.SummaryTooltip_HoursMinutes}\n" +
                   $"\n" +
                   $"{Localization.SummaryTooltip_historyTimeColumnTitle} | {stat.Name.Localized}\n" +
                   $"{string.Join("\n", stat.SessionHistory)}";
        }

        private int GetValuePerHourAsInteger(int value)
        {
            if (value == 0)
                return 0;

            // - the value == 0 guard should be actually enough to catch the session start/reset case. Because on session start all values are 0.
            // - this prevents division by 0 and cases where a value is divided by a very small session duration which would create unrealistic high values
            var sessionJustStarted = _model.SessionDuration.Value.TotalHours < ONE_MINUTE_IN_HOURS; 
            if (sessionJustStarted) 
                return 0;

            // int: because everything that has less than 1/h is not interesting to track anyway.
            // showing decimals would make the more common >>1/h values harder to read. (e.g. 12345 vs 12345.67)
            return (int) (value / _model.SessionDuration.Value.TotalHours);
        }

        private string CreateValueText(int value, int statCurrencyId)
        {
            return statCurrencyId == CurrencyIds.COIN_IN_COPPER
                ? ValueTextService.CreateCoinValueText(value, _settingService.CoinDisplayFormatSetting.Value)
                : value.To0DecimalPlacesCulturedString();
        }

        private readonly Model _model;
        private readonly SettingService _settingService;
        private const int MAX_NUMBER_OF_HISTORY_ENTRIES = 12;
        private const double ONE_MINUTE_IN_HOURS = 0.017;
    }
}