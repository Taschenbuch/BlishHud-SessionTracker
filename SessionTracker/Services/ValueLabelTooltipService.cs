using System.Collections.Generic;
using Blish_HUD.Controls;
using SessionTracker.Models;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Services
{
    public class ValueLabelTooltipService
    {
        public ValueLabelTooltipService(Dictionary<string, Label> valueLabelByEntryId, SettingService settingService)
        {
            _valueLabelByEntryId = valueLabelByEntryId;
            _summaryTextService  = new ValueTooltipTextService(settingService);
        }

        public void UpdateSummaryTooltip(Model model)
        {
            foreach (var entry in model.Entries)
            {
                var tooltipText = _summaryTextService.UpdateAndReturnSummaryText(entry);
                SetSummaryTooltips(tooltipText, entry.Id);
            }

            RemoveKillsDeathsRatioTooltipsBecauseTheyMakeNoSense(_valueLabelByEntryId);
        }

        public void ResetSummaryTooltip(Model model)
        {
            foreach (var entry in model.Entries)
            {
                var tooltipText = _summaryTextService.ResetAndReturnSummaryText(entry);
                SetSummaryTooltips(tooltipText, entry.Id);
            }

            RemoveKillsDeathsRatioTooltipsBecauseTheyMakeNoSense(_valueLabelByEntryId);
        }

        private void SetSummaryTooltips(string tooltipText, string entryId)
        {
            _valueLabelByEntryId[entryId].BasicTooltipText = tooltipText;
        }

        private static void RemoveKillsDeathsRatioTooltipsBecauseTheyMakeNoSense(Dictionary<string, Label> valueLabelByEntryId)
        {
            valueLabelByEntryId[EntryId.WVW_KDR].BasicTooltipText = string.Empty;
            valueLabelByEntryId[EntryId.PVP_KDR].BasicTooltipText = string.Empty;
        }

        private readonly ValueTooltipTextService _summaryTextService;
        private readonly Dictionary<string, Label> _valueLabelByEntryId;
    }
}