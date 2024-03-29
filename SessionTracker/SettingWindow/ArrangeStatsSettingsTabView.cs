﻿using System.Linq;
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
            var settingsFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Arrange Stats", rootFlowPanel);
            settingsFlowPanel.OuterControlPadding = new Vector2(10);

            var warningLabel = new FormattedLabelBuilder()
                .AutoSizeWidth()
                .AutoSizeHeight()
                .SetVerticalAlignment(VerticalAlignment.Top)
                .CreatePart("UPDATE: Stats CANNOT be selected here anymore. This tab is for arranging stats now.\nUse the other tab to select stats first!", builder => builder
                .SetFontSize(Blish_HUD.ContentService.FontSize.Size18)
                .SetTextColor(Color.Yellow)
                .MakeBold())
                .Build()
                .Parent = settingsFlowPanel;

            new CollapsibleHelp(HELP_TEXT, 850, settingsFlowPanel);

            var hasNoSelectedStats = !_services.Model.Stats.Where(s => s.IsSelectedByUser).Any();
            if (hasNoSelectedStats)
            {
                var noStatsSelectedHint = ControlFactory.CreateHintLabel(settingsFlowPanel, "Select stats in 'Select Stats' tab first. Then you can arrange them here!");
                noStatsSelectedHint.TextColor = Color.Yellow;
                return;
            }

            var twoColumnFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                ControlPadding = new Vector2(10),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = settingsFlowPanel
            };

            var statRowsFlowPanel = new FlowPanel // required because this can be cleared without clearing the controls/labels above it.
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(0, 5),
                ControlPadding = new Vector2(0, 5),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = twoColumnFlowPanel
            };

            AddMoveStatsOfCategoryToTopButtons(statRowsFlowPanel, scrollbar, twoColumnFlowPanel);
            ShowStatRows(scrollbar, statRowsFlowPanel);
        }

        private void AddMoveStatsOfCategoryToTopButtons(Container statRowsFlowPanel, Scrollbar scrollbar, Container parent)
        {
            var buttonsFlowPanel = new FlowPanel 
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(0, 5),
                ControlPadding = new Vector2(0, 5),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            var resetButton = new StandardButton
            {
                Text = "Reset stats arrangement",
                BasicTooltipText = "Reset order of stats to how they are displayed in 'Select Stats' tab.",
                Width = 230,
                Parent = buttonsFlowPanel
            };

            var defaultStatsOrder = _services.Model.GetDistinctStatsSortedByCategory();

            resetButton.Click += (s, e) =>
            {
                _services.Model.Stats.Clear();
                _services.Model.Stats.AddRange(defaultStatsOrder);
                UpdateStatRows(scrollbar, statRowsFlowPanel);
            };

            ControlFactory.CreateHintLabel(buttonsFlowPanel, "Super categories");

            var previousCategory = new StatCategory();
            foreach (var category in _services.Model.StatCategories.OrderBy(c => c.IsSubCategory))
            {
                if(category.IsSubCategory && previousCategory.IsSuperCategory)
                {
                    ControlFactory.CreateHintLabel(buttonsFlowPanel, "Sub categories");
                }
                previousCategory = category;

                var orderedCategoryStats = _services.Model.GetDistinctStatIds(category) // order never changes
                        .Select(id => _services.Model.GetStat(id))
                        .Where(s => s.IsSelectedByUser)
                        .ToList();

                var moveCategoryToTopButton = new StandardButton()
                {
                    Text = category.Name.Localized,
                    BasicTooltipText = 
                    "Move selected stats from this category to the top and arrange them like in 'Select stats' tab. " +
                    "Button is disabled when no stats from this category are selected",
                    Enabled = orderedCategoryStats.Any(),
                    Width = 230,
                    Parent = buttonsFlowPanel,
                };

                moveCategoryToTopButton.Click += (s, e) =>
                {
                    var remainingStats = _services.Model.Stats // includes selected and not selected stats. Determin in click handler because their order may have changed
                        .Except(orderedCategoryStats)
                        .ToList();

                    var isNoStatOfCategorySelected = !orderedCategoryStats.Any();
                    if (isNoStatOfCategorySelected)
                        return;

                    _services.Model.Stats.Clear();
                    _services.Model.Stats.AddRange(orderedCategoryStats);
                    _services.Model.Stats.AddRange(remainingStats);
                    UpdateStatRows(scrollbar, statRowsFlowPanel);
                };
            }
        }

        private void ShowStatRows(Scrollbar scrollbar, Container parent)
        {
            // MoveSelectedStatsToTop is required for up/down arrange buttons to work.
            // Otherwise index+/-1 will just move a selected stat between unselected stats which would look like as if the stat is not moving in the UI.
            _services.Model.MoveSelectedStatsToTop();
            parent.ClearChildren();
            foreach (var stat in _services.Model.Stats.Where(s => s.IsSelectedByUser))
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
                    _services.Model.Stats.Insert(index - 1, stat);
                    UpdateStatRows(scrollbar, parent);
                }
            };

            moveStatDownwardsButton.Click += (s, e) =>
            {
                var index = _services.Model.Stats.IndexOf(stat);
                var lastStatIndex = _services.Model.Stats.Where(s => s.IsSelectedByUser).Count() - 1;
                if (index < lastStatIndex)
                {
                    _services.Model.Stats.Remove(stat);
                    _services.Model.Stats.Insert(index + 1, stat);
                    UpdateStatRows(scrollbar, parent);
                }
            };
        }
   
        private void UpdateStatRows(Scrollbar scrollbar, Container parent)
        {
            var scrollDistance = scrollbar.ScrollDistance;
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
        private const string HELP_TEXT =
            "Here you can arrange stats that are selected in the 'Select stats' tab.\n" +
            "- A selected stat will appear only once. A stat is not duplicated if it belongs to multiple categories.\n" +
            "- Use the up and down buttons next to a stat to move it.\n" +
            "- Use the move-category-up-buttons on the right to move all stats of a category to the top at once. " +
            "A move-category-up-button is disabled when no stats of that category are selected.";
    }
}