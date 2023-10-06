using System.Collections.Generic;
using Blish_HUD.Controls;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.SettingEntries;

namespace SessionTracker.Value.Tooltip
{
    public class StatTooltipService
    {
        public StatTooltipService(Dictionary<string, StatTitleFlowPanel> titleFlowPanelByStatId,
                                  Dictionary<string, Label> valueLabelByStatId,
                                  Model model,
                                  SettingService settingService)
        {
            _titleFlowPanelByStatId   = titleFlowPanelByStatId;
            _valueLabelByStatId       = valueLabelByStatId;
            _summaryTooltipTextService = new SummaryTooltipTextService(model, settingService);
        }

        public void UpdateSummaryTooltip(Model model)
        {
            foreach (var stat in model.Stats)
            {
                var tooltipText = _summaryTooltipTextService.UpdateAndReturnSummaryText(stat);
                SetSummaryTooltips(tooltipText, stat.Id);
            }
        }

        public void ResetSummaryTooltip(Model model)
        {
            foreach (var stat in model.Stats)
            {
                var tooltipText = _summaryTooltipTextService.ResetAndReturnSummaryText(stat);
                SetSummaryTooltips(tooltipText, stat.Id);
            }
        }

        private void SetSummaryTooltips(string tooltipText, string statId)
        {
            _valueLabelByStatId[statId].BasicTooltipText = tooltipText;
            _titleFlowPanelByStatId[statId].SetTooltip(tooltipText);
        }

        private readonly SummaryTooltipTextService _summaryTooltipTextService;
        private readonly Dictionary<string, StatTitleFlowPanel> _titleFlowPanelByStatId;
        private readonly Dictionary<string, Label> _valueLabelByStatId;
    }
}