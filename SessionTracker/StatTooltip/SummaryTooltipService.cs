using System.Collections.Generic;
using Blish_HUD.Controls;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.StatsWindow;

namespace SessionTracker.StatTooltip
{
    public class SummaryTooltipService
    {
        public SummaryTooltipService(
            Dictionary<string, StatTitleFlowPanel> titleFlowPanelByStatId,
            Dictionary<string, Label> valueLabelByStatId,
            Model model,
            SettingService settingService,
            TextureService textureService)
        {
            _titleFlowPanelByStatId    = titleFlowPanelByStatId;
            _valueLabelByStatId        = valueLabelByStatId;
            _textureService            = textureService;
            _summaryTooltipTextService = new SummaryTooltipContentService(model, settingService, textureService);
        }

        public void UpdateSummaryTooltip(Model model)
        {
            foreach (var stat in model.Stats)
            {
                _summaryTooltipTextService.UpdateSessionHistory(stat);
                var summaryTooltipContent = _summaryTooltipTextService.CreateSummaryToolTipContent(stat);
                UpdateTooltips(summaryTooltipContent, stat.Id);
            }
        }

        public void ResetSummaryTooltip(Model model)
        {
            foreach (var stat in model.Stats)
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
        private readonly TextureService _textureService;
    }
}