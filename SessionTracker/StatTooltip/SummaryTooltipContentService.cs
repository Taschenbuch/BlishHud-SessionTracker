using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Models;
using SessionTracker.OtherServices;
using SessionTracker.SettingEntries;
using SessionTracker.Text;

namespace SessionTracker.StatTooltip
{
    public class SummaryTooltipContentService
    {
        public SummaryTooltipContentService(Services services)
        {
            _model          = services.Model;
            _settingService = services.SettingService;
            _textureService = services.TextureService;
        }

        public SummaryTooltipContent CreateSummaryToolTipContent(Stat stat)
        {
            if (stat.IsKdr)
            {
                // summary tooltip makes no sense for KDRs. Thus they get a reduced tooltip.
                return new SummaryTooltipContent
                {
                    Name = stat.Name.Localized,
                    Description = stat.Description.Localized,
                    IconTexture = _textureService.StatTextureByStatId[stat.Id],
                    TotalValueText = "-",
                    SessionValueText = "-",
                    SessionValuePerHourText = "-",
                    SessionDurationText = $"{_model.SessionDuration.Value:hh':'mm}",
                };
            }
            else
            {
                return new SummaryTooltipContent
                {
                    Name = stat.Name.Localized,
                    Description = stat.Description.Localized,
                    IconTexture = _textureService.StatTextureByStatId[stat.Id],
                    TotalValueText = StatValueTextService.CreateValueText(stat.Value.Total, stat, _settingService.CoinDisplayFormatSetting.Value),
                    SessionValueText = StatValueTextService.CreateValueText(stat.Value.Session, stat, _settingService.CoinDisplayFormatSetting.Value),
                    SessionValuePerHourText = StatValueTextService.CreateValuePerHourText(stat, _model.SessionDuration.Value, _settingService.CoinDisplayFormatSetting.Value),
                    SessionDurationText = $"{_model.SessionDuration.Value:hh':'mm}",
                    SessionHistoryEntries = stat.SessionHistoryEntries,
                };
            }
        }

        public void ResetSessionHistory(Stat stat)
        {
            stat.SessionHistoryEntries.Clear();
            InsertNewHistoryEntryAtBeginning(stat);
        }

        public void UpdateSessionHistory(Stat stat)
        {
            InsertNewHistoryEntryAtBeginning(stat);

            var historyIsTooLong = stat.SessionHistoryEntries.Count > MAX_NUMBER_OF_HISTORY_ENTRIES;
            if (historyIsTooLong)
                RemoveOldestHistoryEntry(stat.SessionHistoryEntries);
        }

        private void InsertNewHistoryEntryAtBeginning(Stat stat)
        {
            var sessionValueText = StatValueTextService.CreateValueText(stat.Value.Session, stat, _settingService.CoinDisplayFormatSetting.Value);
            RemoveConsecutiveDuplicate(stat);
            stat.SessionHistoryEntries.Insert(0, ($"{DateTime.Now:HH:mm}", sessionValueText));
        }

        // this prevents showing consecutive duplicate values
        // e.g. it is showing this:
        // 00:25 3 // the newest entry is allowed to be a duplicate, so that users see the last update.
        // 00:20 2
        // 00:10 1
        // 00:00 0
        // instead of showing many duplicates like this:
        // 00:25 3
        // 00:20 2
        // 00:15 1
        // 00:10 1
        // 00:05 0
        // 00:00 0
        private static void RemoveConsecutiveDuplicate(Stat stat)
        {
            if (stat.SessionHistoryEntries.Count >= 2)
            {
                var lastValue = stat.SessionHistoryEntries[0].sessionValueText;
                var secondLastValue = stat.SessionHistoryEntries[1].sessionValueText;
                var v = lastValue == secondLastValue;
                if (v)
                    stat.SessionHistoryEntries.RemoveAt(0);
            }
        }

        private static void RemoveOldestHistoryEntry(List<(string time, string sessionValue)> historyEntries)
        {
            historyEntries.Remove(historyEntries.Last());
        }

        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly TextureService _textureService;
        private const int MAX_NUMBER_OF_HISTORY_ENTRIES = 12;
    }
}