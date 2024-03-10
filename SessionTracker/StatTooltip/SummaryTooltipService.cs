using System.Collections.Generic;
using Blish_HUD.Controls;
using SessionTracker.OtherServices;
using SessionTracker.StatsWindow;

namespace SessionTracker.StatTooltip
{
    public class SummaryTooltipService
    {
        public SummaryTooltipService(
            Dictionary<string, StatTitleFlowPanel> titleFlowPanelByStatId,
            Dictionary<string, Label> valueLabelByStatId,
            Services services)
        {
            _titleFlowPanelByStatId    = titleFlowPanelByStatId;
            _valueLabelByStatId        = valueLabelByStatId;
            _services                  = services;
            _summaryTooltipTextService = new SummaryTooltipContentService(services);
        }

        public void UpdateSummaryTooltip()
        {
            foreach (var stat in _services.Model.Stats)
            {
                _summaryTooltipTextService.UpdateSessionHistory(stat);
                var summaryTooltipContent = _summaryTooltipTextService.CreateSummaryToolTipContent(stat);
                UpdateTooltips(summaryTooltipContent, stat.Id);
            }
        }

        public void ResetSummaryTooltip()
        {
            foreach (var stat in _services.Model.Stats)
            {
                _summaryTooltipTextService.ResetSessionHistory(stat);
                var summaryTooltipContent = _summaryTooltipTextService.CreateSummaryToolTipContent(stat);
                UpdateTooltips(summaryTooltipContent, stat.Id);
            }
        }

        private void UpdateTooltips(SummaryTooltipContent summaryTooltipContent, string statId)
        {
            _titleFlowPanelByStatId[statId].UpdateTooltips(summaryTooltipContent);
            ((SummaryTooltip)_valueLabelByStatId[statId].Tooltip).UpdateTooltip(summaryTooltipContent);
        }

        private readonly SummaryTooltipContentService _summaryTooltipTextService;
        private readonly Dictionary<string, StatTitleFlowPanel> _titleFlowPanelByStatId;
        private readonly Dictionary<string, Label> _valueLabelByStatId;
        private readonly Services _services;
    }
}