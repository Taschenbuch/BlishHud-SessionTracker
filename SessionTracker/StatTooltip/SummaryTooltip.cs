using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Properties;
using static Blish_HUD.ContentService;

namespace SessionTracker.StatTooltip
{
    public class SummaryTooltip : Tooltip
    {
        public SummaryTooltip()
        {
            _rootFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = this
            };
        }

        public void UpdateTooltip(SummaryTooltipContent summaryTooltipContent)
        {
            _rootFlowPanel.ClearChildren();
            AddTitleWithDescription(summaryTooltipContent, _rootFlowPanel);
            AddEmptyLine(_rootFlowPanel);
            AddTotalSection(summaryTooltipContent, _rootFlowPanel);
            AddEmptyLine(_rootFlowPanel);
            AddSessionSection(summaryTooltipContent, _rootFlowPanel);
            AddEmptyLine(_rootFlowPanel);
            AddHistory(summaryTooltipContent, _rootFlowPanel);
        }

        private static void AddHistory(SummaryTooltipContent summaryTooltipContent, FlowPanel rootFlowPanel)
        {
            var historyFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = rootFlowPanel
            };

            var timeFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = historyFlowPanel
            };

            var numberFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = historyFlowPanel
            };

            // history header
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(Localization.SummaryTooltip_historyTimeColumnTitle, builder => builder
                .SetFontSize(FontSize.Size14)
                .MakeBold())
                .Build()
                .Parent = timeFlowPanel;

            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(" |  ", _ => { })
                .CreatePart(summaryTooltipContent.Name, builder => builder
                .SetFontSize(FontSize.Size14)
                .MakeBold())
                .Build()
                .Parent = numberFlowPanel;

            // history entries
            foreach (var sessionHistoryEntry in summaryTooltipContent.SessionHistoryEntries)
            {
                new FormattedLabelBuilder()
                    .AutoSizeWidth()
                    .SetHeight(20)
                    .CreatePart(sessionHistoryEntry.time, builder => builder
                    .SetFontSize(FontSize.Size14))
                    .Build()
                    .Parent = timeFlowPanel;

                new FormattedLabelBuilder()
                    .AutoSizeWidth()
                    .SetHeight(20)
                    .CreatePart(" |  ", _ => { })
                    .CreatePart(sessionHistoryEntry.sessionValueText, builder => builder
                    .SetFontSize(FontSize.Size14)
                    .SetTextColor(VALUE_COLOR)
                    .MakeBold())
                    .Build()
                    .Parent = numberFlowPanel;
            }
        }

        private static void AddSessionSection(SummaryTooltipContent summaryTooltipContent, FlowPanel rootFlowPanel)
        {
            // session header
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(Localization.SummaryTooltip_HeaderCurrentSession, builder => builder
                .MakeBold()
                .SetFontSize(FontSize.Size16))
                .Build()
                .Parent = rootFlowPanel;

            var sessionFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = rootFlowPanel
            };

            var numberFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = sessionFlowPanel
            };

            var unitFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = sessionFlowPanel
            };

            // INFO: align numbers to right
            // Aligning them to the right with rightToLeft flowPanels, does not work, because rightToLeft ist bugged and will cutoff longer numbers even with AutoSize. 
            // This issue does not happen with leftToRight flowPanels

            // session value
            AddBoldYellowLabel(summaryTooltipContent.SessionValueText, numberFlowPanel);

            // session value unit
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart($" {summaryTooltipContent.Name}", builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = unitFlowPanel;

            AddBoldYellowLabel(summaryTooltipContent.SessionValuePerHourText, numberFlowPanel);

            // per hour unit
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart($" {summaryTooltipContent.Name} / {Localization.SummaryTooltip_Hour}", builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = unitFlowPanel;

            // session duration
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(summaryTooltipContent.SessionDurationText, builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = numberFlowPanel;

            // session duration unit
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart($" {Localization.SummaryTooltip_HoursMinutes} (Duration)", builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = unitFlowPanel;
        }

        private static void AddTotalSection(SummaryTooltipContent summaryTooltipContent, FlowPanel rootFlowPanel)
        {
            // total header
            new FormattedLabelBuilder()
                            .AutoSizeWidth()
                            .AutoSizeHeight()
                            .CreatePart("Total", builder => builder
                            .MakeBold()
                            .SetFontSize(FontSize.Size16))
                            .Build()
                            .Parent = rootFlowPanel;

            // total value
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(summaryTooltipContent.TotalValueText, builder => builder
                .SetFontSize(FontSize.Size14)
                .SetTextColor(VALUE_COLOR)
                .MakeBold())
                .CreatePart($" {summaryTooltipContent.Name}", builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = rootFlowPanel;
        }

        private static void AddTitleWithDescription(SummaryTooltipContent summaryTooltipContent, FlowPanel rootFlowPanel)
        {
            var iconAndNameFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = rootFlowPanel
            };

            // icon
            var statImage = new Image(summaryTooltipContent.IconTexture)
            {
                Size = new Point(25),
                Parent = iconAndNameFlowPanel
            };

            // name
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart($" {summaryTooltipContent.Name}", builder => builder
                .SetTextColor(VALUE_COLOR)
                .MakeBold()
                .SetFontSize(FontSize.Size22))
                .Build()
                .Parent = iconAndNameFlowPanel;

            // description
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(summaryTooltipContent.Description, builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = rootFlowPanel;
        }

        private static void AddEmptyLine(FlowPanel flowPanel)
        {
            new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .CreatePart(" ", builder => builder
                .SetFontSize(FontSize.Size14))
                .Build()
                .Parent = flowPanel;
        }

        private static void AddBoldYellowLabel(string text, Container parent)
        {
            new FormattedLabelBuilder()
                            .AutoSizeWidth()
                            .AutoSizeHeight()
                            .CreatePart(text, builder => builder
                            .SetFontSize(FontSize.Size14)
                            .SetTextColor(VALUE_COLOR)
                            .MakeBold())
                            .Build()
                            .Parent = parent;
        }

        private static readonly Color VALUE_COLOR = Color.Yellow;
        private readonly FlowPanel _rootFlowPanel;
    }
}
