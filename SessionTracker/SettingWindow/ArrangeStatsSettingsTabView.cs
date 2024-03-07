using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.OtherServices;

namespace SessionTracker.SettingsWindow
{
    public class ArrangeStatsSettingsTabView : View
    {
        public ArrangeStatsSettingsTabView(Services services)
        {
            _services = services;
        }

        protected override void Unload()
        {
        }

        protected override void Build(Container buildPanel)
        {
            var rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel); // must be created before getting scrollbar. scrollbar is null otherwise
            var scrollbar = (Scrollbar)buildPanel.Children.FirstOrDefault(c => c is Scrollbar);
            var trackedStatsSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Arrange Stats", rootFlowPanel);
            trackedStatsSectionFlowPanel.OuterControlPadding = new Vector2(10);

            var warningLabel = new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .SetVerticalAlignment(VerticalAlignment.Top)
                .CreatePart("UPDATE: You CANNOT select stats here anymore! In this tab you can only arrange stats now!\nUse the other tab to select stats!", builder => builder
                .SetFontSize(Blish_HUD.ContentService.FontSize.Size18)
                .SetTextColor(Color.Yellow)
                .MakeBold())
                .Build()
                .Parent = trackedStatsSectionFlowPanel;
                 
            ControlFactory.CreateHintLabel(
                trackedStatsSectionFlowPanel,
                // todo x ersetzen durch help button
                // todo x text updaten
                "You can arrange the stats with the up and down buttons.");

            var selectedStats = _services.Model.Stats.Where(s => s.IsVisible).ToList();
            var hasNoSelectedStats = !selectedStats.Any();
            if (hasNoSelectedStats)
            {
                ControlFactory.CreateHintLabel(trackedStatsSectionFlowPanel, "You have to select stats in 'Select Stats' tab first!");
                return;
            }

            var statRowsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(0, 5),
                ControlPadding = new Vector2(0, 5),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = trackedStatsSectionFlowPanel
            };

            ShowStatRows(scrollbar, statRowsFlowPanel);
        }

        private void ShowStatRows(Scrollbar scrollbar, Container parent)
        {
            // MoveSelectedStatsToTop is required for up/down arrange buttons to work. otherwise index+/-1 will just move a selected stat between unselected stats
            _services.Model.MoveSelectedStatsToTop(); 

            foreach (var stat in _services.Model.Stats.Where(s => s.IsVisible))
                ShowStatRow(stat, scrollbar, parent);
        }

        private void ShowStatRow(Stat stat, Scrollbar scrollbar, Container parent)
        {
            var statFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                BackgroundColor = new Color(Color.Black, 0.5f),
                Width = 400,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            var moveStatUpwardsButton = new GlowButton
            {
                Icon = _services.TextureService.MoveUpTexture,
                ActiveIcon = _services.TextureService.MoveUpActiveTexture,
                BasicTooltipText = "Move up",
                Size = new Point(25, 25),
                Parent = statFlowPanel
            };

            var moveStatDownwardsButton = new GlowButton()
            {
                Icon = _services.TextureService.MoveDownTexture,
                ActiveIcon = _services.TextureService.MoveDownActiveTexture,
                BasicTooltipText = "Move down",
                Size = new Point(25, 25),
                Parent = statFlowPanel
            };

            new Image(_services.TextureService.StatTextureByStatId[stat.Id])
            {
                BasicTooltipText = stat.GetTextWithNameAndDescription(),
                Size = new Point(24),
                Location = new Point(20, 0),
                Parent = ControlFactory.CreateAdjustableChildLocationContainer(statFlowPanel),
            };

            new Label
            {
                Text             = stat.Name.Localized,
                BasicTooltipText = stat.GetTextWithNameAndDescription(),
                AutoSizeWidth    = true,
                AutoSizeHeight   = true,
                Location         = new Point(5, 3),
                Parent           = ControlFactory.CreateAdjustableChildLocationContainer(statFlowPanel)
            };

            moveStatUpwardsButton.Click += (s, e) =>
            {
                var index = _services.Model.Stats.IndexOf(stat);
                var firstStatIndex = 0;
                if (index > firstStatIndex)
                {
                    _services.Model.Stats.Remove(stat); 
                    _services.Model.Stats.Insert(index - 1, stat); // todo x. falsch? bezieht sich ja nur noch auf visible stats
                    UpdateStatRows(scrollbar, parent);
                }
            };

            moveStatDownwardsButton.Click += (s, e) =>
            {
                var index = _services.Model.Stats.IndexOf(stat);
                var lastStatIndex = _services.Model.Stats.Where(s => s.IsVisible).Count() - 1;
                if (index < lastStatIndex)
                {
                    _services.Model.Stats.Remove(stat);
                    _services.Model.Stats.Insert(index + 1, stat); // todo x. falsch? bezieht sich ja nur noch auf visible stats
                    UpdateStatRows(scrollbar, parent);
                }
            };
        }
   
        private void UpdateStatRows(Scrollbar scrollbar, Container parent)
        {
            var scrollDistance = scrollbar.ScrollDistance;
            parent.ClearChildren();
            ShowStatRows(scrollbar, parent);
            _services.Model.UiHasToBeUpdated = true;

            Task.Run(async () =>
            {
                await Task.Delay(_services.SettingService.ScrollbarFixDelay.Value);
                if(scrollbar != null) // not sure if necessary, but may happen when this view gets destroyed while this task is still running
                    scrollbar.ScrollDistance = scrollDistance;
            });
        }

        private readonly Services _services;
    }
}