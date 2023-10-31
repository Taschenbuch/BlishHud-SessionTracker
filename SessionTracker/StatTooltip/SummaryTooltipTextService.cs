using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Constants;
using SessionTracker.Models;
using SessionTracker.Properties;
using SessionTracker.SettingEntries;
using SessionTracker.Text;
using SessionTracker.Value;

namespace SessionTracker.Tooltip
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
            var sessionValueText = StatValueTextService.CreateValueText(stat.Value.Session, stat, _settingService.CoinDisplayFormatSetting.Value);
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

            var sessionValuePerHourText = StatValueTextService.CreateValuePerHourText(stat, _model.SessionDuration.Value, _settingService.CoinDisplayFormatSetting.Value);
            var totalValueText          = StatValueTextService.CreateValueText(stat.Value.Total, stat, _settingService.CoinDisplayFormatSetting.Value);

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

        private readonly Model _model;
        private readonly SettingService _settingService;
        private const int MAX_NUMBER_OF_HISTORY_ENTRIES = 12;
    }
}