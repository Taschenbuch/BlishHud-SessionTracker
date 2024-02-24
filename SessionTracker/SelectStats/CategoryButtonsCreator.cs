using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Constants;
using SessionTracker.Controls;
using SessionTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SessionTracker.SelectStats
{
    public class CategoryButtonsCreator
    {
        public static void AddSelectStatsFromGroupButtons(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, Container parent)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = parent,
            };

            new Label
            {
                Text = "Select",
                Location = new Point(5, 4),
                Width = 58,
                Parent = ControlFactory.CreateAdjustableChildLocationContainer(buttonsFlowPanel)
            };

            var selectAllButton = new StandardButton
            {
                Text = "All",
                BasicTooltipText = "Select all stats.",
                Width = 50,
                Parent = buttonsFlowPanel
            };

            var pvpButton = new StandardButton
            {
                Text = "PvP",
                BasicTooltipText = "Select pvp related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var wvwButton = new StandardButton
            {
                Text = "WvW",
                BasicTooltipText = "Select wvw related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var fractalsButton = new StandardButton
            {
                Text = "Fractals",
                BasicTooltipText = "Select fractal related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var strikesButton = new StandardButton
            {
                Text = "Strikes",
                BasicTooltipText = "Select strike related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var raidsButton = new StandardButton
            {
                Text = "Raids",
                BasicTooltipText = "Select raid related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var openWorldButton = new StandardButton
            {
                Text = "Open World",
                BasicTooltipText = "Select open world related stats.",
                Width = 100,
                Parent = buttonsFlowPanel
            };

            var materialStorageButton = new StandardButton
            {
                Text = "Material Storage",
                BasicTooltipText = "Select material storage stats.",
                Width = 120,
                Parent = buttonsFlowPanel
            };

            selectAllButton.Click += (s, e) =>
            {
                foreach (var selectStatContainer in statContainersByStatId.Values)
                    selectStatContainer.SelectStat();
            };

            pvpButton.Click += (s, e) => SelectOrUnselectPvpStats(statContainersByStatId, stats, true);
            wvwButton.Click += (s, e) => SelectOrUnselectWvwStats(statContainersByStatId, stats, true);
            fractalsButton.Click += (s, e) => SelectOrUnselectFractalStats(statContainersByStatId, stats, true);
            strikesButton.Click += (s, e) => SelectOrUnselectStrikeStats(statContainersByStatId, stats, true);
            raidsButton.Click += (s, e) => SelectOrUnselectRaidStats(statContainersByStatId, stats, true);
            openWorldButton.Click += (s, e) => SelectOrUnselectOpenWorldStats(statContainersByStatId, stats, true);
            materialStorageButton.Click += (s, e) => SelectOrUnselectMaterialStorageStats(statContainersByStatId, true);
        }

        public static void AddUnselectStatsFromGroupButtons(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, Container parent)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                Top = 30,
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = parent,
            };

            new Label
            {
                Text = "Unselect",
                Location = new Point(5, 4),
                Width = 58,
                Parent = ControlFactory.CreateAdjustableChildLocationContainer(buttonsFlowPanel)
            };

            var unselectAllButton = new StandardButton
            {
                Text = "All",
                BasicTooltipText = "Unselect all stats",
                Width = 50,
                Parent = buttonsFlowPanel
            };

            var pvpButton = new StandardButton
            {
                Text = "PvP",
                BasicTooltipText = "Unselect pvp related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var wvwButton = new StandardButton
            {
                Text = "WvW",
                BasicTooltipText = "Unselect wvw related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var fractalsButton = new StandardButton
            {
                Text = "Fractals",
                BasicTooltipText = "Unselect fractal related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var strikesButton = new StandardButton
            {
                Text = "Strikes",
                BasicTooltipText = "Unselect strike related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var raidsButton = new StandardButton
            {
                Text = "Raids",
                BasicTooltipText = "Unselect raid related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var openWorldButton = new StandardButton
            {
                Text = "Open World",
                BasicTooltipText = "Unselect open world related stats.",
                Width = 100,
                Parent = buttonsFlowPanel
            };

            var materialStorageButton = new StandardButton
            {
                Text = "Material Storage",
                BasicTooltipText = "Unselect material storage stats.",
                Width = 120,
                Parent = buttonsFlowPanel
            };

            unselectAllButton.Click += (s, e) =>
            {
                foreach (var selectStatContainer in statContainersByStatId.Values)
                    selectStatContainer.UnselectStat();
            };

            pvpButton.Click += (s, e) => SelectOrUnselectPvpStats(statContainersByStatId, stats, false);
            wvwButton.Click += (s, e) => SelectOrUnselectWvwStats(statContainersByStatId, stats, false);
            fractalsButton.Click += (s, e) => SelectOrUnselectFractalStats(statContainersByStatId, stats, false);
            strikesButton.Click += (s, e) => SelectOrUnselectStrikeStats(statContainersByStatId, stats, false);
            raidsButton.Click += (s, e) => SelectOrUnselectRaidStats(statContainersByStatId, stats, false);
            openWorldButton.Click += (s, e) => SelectOrUnselectOpenWorldStats(statContainersByStatId, stats, false);
            materialStorageButton.Click += (s, e) => SelectOrUnselectMaterialStorageStats(statContainersByStatId, false);
        }

        private static void SelectOrUnselectOpenWorldStats(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, bool isSelected)
        {
            SelectOrUnselectByCurrencyId(CurrencyIds.OpenWorld, statContainersByStatId, stats, isSelected);
        }

        private static void SelectOrUnselectRaidStats(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, bool isSelected)
        {
            SelectOrUnselectByItemId(ItemIds.Raid, statContainersByStatId, stats, isSelected);
            SelectOrUnselectByCurrencyId(CurrencyIds.Raid, statContainersByStatId, stats, isSelected);
        }

        private static void SelectOrUnselectFractalStats(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, bool isSelected)
        {
            SelectOrUnselectByItemId(ItemIds.Fractal, statContainersByStatId, stats, isSelected);
            SelectOrUnselectByCurrencyId(CurrencyIds.Fractal, statContainersByStatId, stats, isSelected);
        }

        private static void SelectOrUnselectPvpStats(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, bool isSelected)
        {
            SelectOrUnselectByCategoryId(CategoryId.PVP, statContainersByStatId, isSelected);
            SelectOrUnselectByCurrencyId(CurrencyIds.Pvp, statContainersByStatId, stats, isSelected);
            statContainersByStatId[StatId.DEATHS].SelectOrUnselectStat(isSelected);
        }

        private static void SelectOrUnselectWvwStats(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, bool isSelected)
        {
            SelectOrUnselectByCategoryId(CategoryId.WVW, statContainersByStatId, isSelected);
            SelectOrUnselectByCurrencyId(CurrencyIds.Wvw, statContainersByStatId, stats, isSelected);
            SelectOrUnselectByItemId(ItemIds.Wvw, statContainersByStatId, stats, isSelected);
            statContainersByStatId[StatId.DEATHS].SelectOrUnselectStat(isSelected);
        }

        private static void SelectOrUnselectStrikeStats(Dictionary<string, SelectStatContainer> statContainersByStatId, List<Stat> stats, bool isSelected)
        {
            SelectOrUnselectByCurrencyId(CurrencyIds.Strike, statContainersByStatId, stats, isSelected);
        }

        private static void SelectOrUnselectMaterialStorageStats(Dictionary<string, SelectStatContainer> statContainersByStatId, bool isSelected)
        {
            SelectOrUnselectByCategoryIdStartingWith(CategoryId.MATERIAL_STORAGE_ID_PREFIX, statContainersByStatId, isSelected);
        }

        private static void SelectOrUnselectByCategoryId(string categoryId, Dictionary<string, SelectStatContainer> statContainersByStatId, bool isSelected)
        {
            foreach (var selectStatContainer in statContainersByStatId.Values.Where(c => c.Stat.CategoryId == categoryId))
                selectStatContainer.SelectOrUnselectStat(isSelected);
        }

        private static void SelectOrUnselectByCategoryIdStartingWith(string searchTerm, Dictionary<string, SelectStatContainer> statContainersByStatId, bool isSelected)
        {
            foreach (var selectStatContainer in statContainersByStatId.Values.Where(c => c.Stat.CategoryId.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)))
                selectStatContainer.SelectOrUnselectStat(isSelected);
        }

        private static void SelectOrUnselectByCurrencyId(
            ReadOnlyCollection<int> currencyIds,
            Dictionary<string,
            SelectStatContainer> statContainersByStatId,
            List<Stat> stats,
            bool isSelected)
        {
            foreach (var stat in stats.Where(stat => currencyIds.Contains(stat.ApiId)))
                statContainersByStatId[stat.Id].SelectOrUnselectStat(isSelected);
        }

        private static void SelectOrUnselectByItemId(
            ReadOnlyCollection<int> itemIds,
            Dictionary<string, SelectStatContainer> statContainersByStatId,
            List<Stat> stats,
            bool isSelected)
        {
            foreach (var stat in stats.Where(stat => itemIds.Contains(stat.ApiId)))
                statContainersByStatId[stat.Id].SelectOrUnselectStat(isSelected);
        }
    }
}
