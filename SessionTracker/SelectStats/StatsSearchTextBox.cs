﻿using Blish_HUD.Controls;
using SessionTracker.Controls;
using SessionTracker.OtherServices;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.SelectStats
{
    public class StatsSearchTextBox : TextBox
    {
        public StatsSearchTextBox(
            Services services, 
            Dictionary<string, SelectStatsControls> controlsByCategoryId,
            FlowPanel rootFlowPanel, 
            Container parent)
        {
            PlaceholderText = "Search...";
            Left = 4;
            Width = 300;
            BasicTooltipText = "Search for stats. Only stats that include the search term will be displayed";
            Parent = parent;

            var noSearchResultsHintLabel = ControlFactory.CreateHintLabel(null, "No stats match search term!");

            var clearSearchButton = new StandardButton()
            {
                Text = "x",
                Top = Top,
                Left = Right - 30,
                Width = 30,
                BasicTooltipText = "Clear search input",
                Visible = false,
                Parent = parent,
            };

            clearSearchButton.Click += (s, o) => Text = "";

            var categoryIds = services.Model.StatCategories.Select(c => c.Id).ToList();
            var superCategoryIds = services.Model.StatCategories.Select(c => c.Id).ToList();

            TextChanged += (s, o) =>
            {
                var hasSearchTerm = !string.IsNullOrWhiteSpace(Text);
                clearSearchButton.Visible = hasSearchTerm;

                // clear all
                foreach (var categoryId in categoryIds)
                {
                    controlsByCategoryId[categoryId].CategoryContainer.Parent = null;
                    controlsByCategoryId[categoryId].CategoryFlowPanel.ClearChildren();
                }

                noSearchResultsHintLabel.Parent = null;

                // no search term -> show all stats
                if (!hasSearchTerm)
                {
                    var statContainers = controlsByCategoryId.Values.SelectMany(c => c.StatContainers).ToList();
                    showCategoriesWithStats(statContainers, controlsByCategoryId, rootFlowPanel);
                    return;
                }

                // no stats found -> show hint
                var matchingStatsContainers = controlsByCategoryId.Values
                    .SelectMany(c => c.StatContainers)
                    .Where(c => c.Stat.Name.Localized.ToLower().Contains(Text.ToLower()))
                    .ToList();

                var hasNotFoundStats = !matchingStatsContainers.Any();
                if (hasNotFoundStats)
                {
                    noSearchResultsHintLabel.Parent = rootFlowPanel;
                    return;
                }

                // stats found -> show found stats
                showCategoriesWithStats(matchingStatsContainers, controlsByCategoryId, rootFlowPanel);
            };
        }

        private static void showCategoriesWithStats(
            List<SelectStatContainer> matchingStatsContainers, 
            Dictionary<string, SelectStatsControls> controlsByCategoryId, 
            Container parent)
        {
            foreach (var matchingStatContainer in matchingStatsContainers)
            {
                var subCategoryId = matchingStatContainer.SubCategoryId;
                var superCategoryId = matchingStatContainer.SuperCategoryId;
                var subCategoryFlowPanel = controlsByCategoryId[subCategoryId].CategoryFlowPanel;
                var superCategoryFlowPanel = controlsByCategoryId[superCategoryId].CategoryFlowPanel;
                matchingStatContainer.Parent = subCategoryFlowPanel;
                controlsByCategoryId[subCategoryId].CategoryContainer.Parent = superCategoryFlowPanel;
                controlsByCategoryId[superCategoryId].CategoryContainer.Parent = parent;
                subCategoryFlowPanel.Expand();
                superCategoryFlowPanel.Expand();
            }
        }
    }
}
