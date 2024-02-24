using Blish_HUD.Controls;
using SessionTracker.OtherServices;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.SelectStats
{
    public class StatsSearchTextBox : TextBox
    {
        public StatsSearchTextBox(
            Services services, 
            Label noSearchResultsHintLabel,
            Dictionary<string, SelectStatContainer> statContainersByStatId,
            Dictionary<string, (Container CategoryContainer, FlowPanel CategoryFlowPanel, List<SelectStatContainer> StatContainers)> controlsByCatalogId, 
            FlowPanel rootFlowPanel, 
            Container parent)
        {
            PlaceholderText = "Search...";
            Top = 60;
            Left = 4;
            Width = 300;
            BasicTooltipText = "Search for stats. Only stats that include the search term will be displayed";
            Parent = parent;

            var categoryIds = services.Model.StatCategories.Select(c => c.Id).ToList();

            var clearSearchButton = new StandardButton()
            {
                Text = "x",
                Top = 60,
                Left = Right - 30,
                Width = 30,
                BasicTooltipText = "Clear search input",
                Visible = false,
                Parent = parent,
            };

            clearSearchButton.Click += (s, o) => Text = "";

            TextChanged += (s, o) =>
            {
                var hasSearchTerm = !string.IsNullOrWhiteSpace(Text);
                clearSearchButton.Visible = hasSearchTerm;
    
                // clear all
                foreach (var categoryId in categoryIds)
                {
                    controlsByCatalogId[categoryId].CategoryContainer.Parent = null;
                    controlsByCatalogId[categoryId].CategoryFlowPanel.ClearChildren(); // die ganzen fields wieder in local variables umwandeln? 
                }

                noSearchResultsHintLabel.Parent = null;

                // no search term -> add all
                var hasNoSearchTerm = string.IsNullOrWhiteSpace(Text);
                if (hasNoSearchTerm)
                {
                    foreach (var categoryId in categoryIds)
                    {
                        controlsByCatalogId[categoryId].StatContainers.ForEach(c => c.Parent = controlsByCatalogId[categoryId].CategoryFlowPanel);
                        controlsByCatalogId[categoryId].CategoryContainer.Parent = rootFlowPanel;
                    }
                    return;
                }

                // no stats found -> show hint
                var matchingStatsContainers = statContainersByStatId.Values.Where(c => c.Stat.Name.Localized.ToLower().Contains(Text.ToLower()));
                var hasNotFoundStats = !matchingStatsContainers.Any();
                if (hasNotFoundStats)
                {
                    noSearchResultsHintLabel.Parent = rootFlowPanel;
                    return;
                }

                // stats found -> add found stats
                foreach (var matchingStatContainer in matchingStatsContainers)
                {
                    matchingStatContainer.Parent = controlsByCatalogId[matchingStatContainer.Stat.CategoryId].CategoryFlowPanel;
                    controlsByCatalogId[matchingStatContainer.Stat.CategoryId].CategoryContainer.Parent = rootFlowPanel;
                }
            };
        }
    }
}
