using System.Collections.Generic;
using Blish_HUD.Controls;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Value.Tooltip
{
    public class StatTooltipService
    {
        public StatTooltipService(Dictionary<string, EntryTitleFlowPanel> titleFlowPanelByEntryId,
                                  Dictionary<string, Label> valueLabelByEntryId,
                                  Model model,
                                  SettingService settingService)
        {
            _titleFlowPanelByEntryId   = titleFlowPanelByEntryId;
            _valueLabelByEntryId       = valueLabelByEntryId;
            _summaryTooltipTextService = new SummaryTooltipTextService(model, settingService);
        }

        public void UpdateSummaryTooltip(Model model)
        {
            foreach (var entry in model.Entries)
            {
                var tooltipText = _summaryTooltipTextService.UpdateAndReturnSummaryText(entry);
                SetSummaryTooltips(tooltipText, entry.Id);
            }
        }

        public void ResetSummaryTooltip(Model model)
        {
            foreach (var entry in model.Entries)
            {
                var tooltipText = _summaryTooltipTextService.ResetAndReturnSummaryText(entry);
                SetSummaryTooltips(tooltipText, entry.Id);
            }
        }

        private void SetSummaryTooltips(string tooltipText, string entryId)
        {
            _valueLabelByEntryId[entryId].BasicTooltipText = tooltipText;
            _titleFlowPanelByEntryId[entryId].SetTooltip(tooltipText);
        }

        private readonly SummaryTooltipTextService _summaryTooltipTextService;
        private readonly Dictionary<string, EntryTitleFlowPanel> _titleFlowPanelByEntryId;
        private readonly Dictionary<string, Label> _valueLabelByEntryId;
    }
}