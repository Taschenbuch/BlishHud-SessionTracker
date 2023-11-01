using System;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Constants;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.Text;

namespace SessionTracker.StatTooltip
{
    public class SummaryTooltipContentService
    {
        public SummaryTooltipContentService(Model model, SettingService settingService, TextureService textureService)
        {
            _model          = model;
            _settingService = settingService;
            _textureService = textureService;
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
            stat.SessionHistoryEntries.Insert(0, ($"{DateTime.Now:HH:mm}", sessionValueText));
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