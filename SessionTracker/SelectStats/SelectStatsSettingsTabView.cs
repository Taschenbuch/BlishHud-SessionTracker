using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD;
using SessionTracker.OtherServices;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Controls;
using SessionTracker.Models;
using Blish_HUD.Graphics.UI;
using System;
using SessionTracker.SettingEntries;

namespace SessionTracker.SelectStats
{
    public class SelectStatsSettingsTabView : View
    {
        public SelectStatsSettingsTabView(Services services)
        {
            _services = services;

            foreach (var category in services.Model.StatCategories)
                _controlsByCategoryId.Add(category.Id, new SelectStatsControls()); 
        }

        protected override void Unload()
        {
            _services.SettingService.SelectStatsIconSizeSetting.SettingChanged -= SelectStatsIconSizeSettingChanged;
            _controlsByCategoryId.Clear();
        }

        protected override void Build(Container buildPanel)
        {
            var rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);
            var settingsFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Select Stats", rootFlowPanel);
            settingsFlowPanel.ControlPadding = new Vector2(0, 5);
            AddHelp(settingsFlowPanel);

            var topControlsContainer = ControlFactory.CreateAdjustableChildLocationContainer(settingsFlowPanel);
            var searchTextBox = new StatsSearchTextBox(
                _services,
                _noSearchResultsHintLabel,
                _controlsByCategoryId,
                settingsFlowPanel,
                topControlsContainer);

            var iconSizePanel = AddIconSizeDropdown(_services, topControlsContainer, searchTextBox.Right);
            AddButtonsToExpandCollapseAllCategories(_controlsByCategoryId, _services.Model.StatCategories, topControlsContainer);
            AddButtonsToSelectAndUnselectAllStats(_controlsByCategoryId, topControlsContainer);

            var selectStatViewModelByStatId = _services.Model.Stats.ToDictionary(s => s.Id, s => new SelectStatViewModel(s, _services));
            var rootCategoriesFlowPanel = CreateRootCategoriesFlowPanel(settingsFlowPanel);

            foreach (var superCategory in _services.Model.StatCategories.Where(c => c.IsSuperCategory))
            {
                var superStatViewModels = superCategory.SubCategoryIds
                    .Select(categoryId => _services.Model.StatCategories.Single(c => c.Id == categoryId))
                    .SelectMany(category => category.StatIds)
                    .Distinct() // this way the super category counter does not count the same stat twice. May look a bit confusing because numbers wont add up.
                    .Select(statId => selectStatViewModelByStatId[statId])
                    .ToList();

                var categoryContainer = ControlFactory.CreateAdjustableChildLocationContainer(rootCategoriesFlowPanel);
                var categoryFlowPanel = CreateSuperCategoryFlowPanel(superCategory, categoryContainer);
                _controlsByCategoryId[superCategory.Id].CategoryContainer = categoryContainer;
                _controlsByCategoryId[superCategory.Id].CategoryFlowPanel = categoryFlowPanel;
                AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(_controlsByCategoryId[superCategory.Id].StatContainers, categoryContainer, SUPER_CATEGORY_LOCATION_RIGHT_SHIFT);
                new SelectedCountLabel(superStatViewModels, categoryContainer, SUPER_CATEGORY_LOCATION_RIGHT_SHIFT);

                foreach (var subCategoryId in superCategory.SubCategoryIds)
                {
                    var subCategory = _services.Model.StatCategories.Single(c => c.Id == subCategoryId);
                    var subStatViewModels = subCategory.StatIds.Select(statId => selectStatViewModelByStatId[statId]).ToList();
                    var subCategoryContainer = ControlFactory.CreateAdjustableChildLocationContainer(categoryFlowPanel);
                    var subCategoryFlowPanel = CreateSubCategoryFlowPanel(subCategory, subCategoryContainer);
                    new SelectedCountLabel(subStatViewModels, subCategoryContainer, 0);
                    AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(_controlsByCategoryId[subCategoryId].StatContainers, subCategoryContainer, 0);
                    _controlsByCategoryId[subCategoryId].CategoryContainer = subCategoryContainer;
                    _controlsByCategoryId[subCategoryId].CategoryFlowPanel = subCategoryFlowPanel;

                    foreach (var statId in subCategory.StatIds)
                    {
                        var statContainer = new SelectStatContainer(selectStatViewModelByStatId[statId], superCategory.Id, subCategoryId, _services, subCategoryFlowPanel);
                        _controlsByCategoryId[subCategoryId].StatContainers.Add(statContainer);
                        _controlsByCategoryId[superCategory.Id].StatContainers.Add(statContainer);
                    }
                }
            }
        }

        private static FlowPanel CreateRootCategoriesFlowPanel(Container parent)
        {
            return new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                Width = 862,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = parent
            };
        }

        private static FlowPanel CreateSuperCategoryFlowPanel(StatCategory category, Container parent)
        {
            return new FlowPanel()
            {
                Title = category.Name.Localized,
                FlowDirection = ControlFlowDirection.LeftToRight,
                Width = 862,
                HeightSizingMode = SizingMode.AutoSize,
                OuterControlPadding = new Vector2(SUPER_CATEGORY_LOCATION_RIGHT_SHIFT, 0),
                CanCollapse = true,
                Parent = parent
            };
        }

        private static FlowPanel CreateSubCategoryFlowPanel(StatCategory category, Container parent)
        {
            return new FlowPanel()
            {
                Title = category.Name.Localized,
                FlowDirection = ControlFlowDirection.LeftToRight,
                Width = 848,
                HeightSizingMode = SizingMode.AutoSize,
                OuterControlPadding = new Vector2(2),
                CanCollapse = true,
                Parent = parent
            };
        }

        private static void AddHelp(Container parent)
        {
            var helpButton = new StandardButton()
            {
                Text = SHOW_HELP_BUTTON_TEXT,
                BackgroundColor = Color.Red,
                Width = 100,
                Top = 5,
                Left = 5,
            };

            var collapsedHeight = helpButton.Height + 30;
            var collapsedWidth = helpButton.Width + 30;

            var helpPanel = new Panel()
            {
                ShowBorder = true,
                Height = collapsedHeight,
                Width = collapsedWidth,
                Parent = parent
            };

            var isHelpExpanded = false;

            helpButton.Click += (s, e) =>
            {
                isHelpExpanded = !isHelpExpanded;
                helpButton.Text = isHelpExpanded ? HIDE_HELP_BUTTON_TEXT : SHOW_HELP_BUTTON_TEXT;
                helpPanel.Height = isHelpExpanded ? EXPANDED_HELP_HEIGHT : collapsedHeight;
                helpPanel.Width = isHelpExpanded ? EXPANDED_HELP_WIDTH : collapsedWidth;
            };

            var helpContainer = new LocationContainer()
            {
                BackgroundColor = Color.Black * 0.5f,
                Width = EXPANDED_HELP_WIDTH - 20,
                Height = EXPANDED_HELP_HEIGHT - 20,
                Top = 3,
                Left = 4,
                Parent = helpPanel
            };

            helpButton.Parent = helpContainer;

            // VORSICHT: label ist aus irgendeinem grund immer Zentriert und verschiebt sich.
            var helpText =  // todo x line breaks nötig weil cut off. wrapText property klappt nicht. schneidet oben/unten ab.
                "todo x XXXXXXXX Step by step anleitung: collapse all button. categories bei bedarf öffnen (expand all oder auf category title clicken). stat suchen per search. \n" +
                "- To select stats click on a stat icon or use the (un)select buttons above the categories. Not selected stats are transparent.\n" +
                "- WARNING: some stats are part of multiple categories. Unselecting a whole category can result in unselecting stats from other categories as well." +
                "E.g. 'deaths' stat is part of 'WvW' category and 'PvP' category. E.g. 'Mystic Coin' stat is part of 'Raid', 'Fractal' and 'Material Storage' category.\n" +
                "- Expand/Collapse categories for a better overview by clicking on category headers (no need to click the arrow icon) or the 'Expand/Collapse all' buttons. " +
                "BUG: expand/collapse causes the scrollbar to jump to the top. At the moment i have no idea how to fix that.\n" +
                "- Search input helps to find stats faster.\n" +
                "- If you want to track many stats it is recommended to enable 'hide stats with value = 0' setting and/or 'fixed window height' setting.\n" +
                "- Selected counter will count duplicated stats as 1. Because of that it may look like numbers dont add up.\n" +
                "- Arrange stats: Not here. Rearrange stats in the other settings tab";

            new Label
            {
                Text = helpText,
                VerticalAlignment = VerticalAlignment.Top,
                WrapText = true,
                Width = helpContainer.Width - 2 * HELP_LABEL_BORDER_SIZE,
                Height = helpContainer.Height - 2 * HELP_LABEL_BORDER_SIZE,
                Top = collapsedHeight - 20,
                Left = HELP_LABEL_BORDER_SIZE,
                Parent = helpContainer
            };
        }

        private Panel AddIconSizeDropdown(Services services, Container parent, int leftPosition)
        {
            var settingDisplayName = services.SettingService.SelectStatsIconSizeSetting.GetDisplayNameFunc();
            var settingTooltipText = services.SettingService.SelectStatsIconSizeSetting.GetDescriptionFunc();

            var iconSizePanel = new Panel
            {
                Left = leftPosition + 20,
                BasicTooltipText = settingTooltipText,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = parent
            };

            var iconSizeLabel = new Label
            {
                Text = settingDisplayName,
                BasicTooltipText = settingTooltipText,
                Top = 4,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Parent = iconSizePanel,
            };

            var iconSizeDropDown = new Dropdown
            {
                BasicTooltipText = settingTooltipText,
                Left = iconSizeLabel.Right + 5,
                Width = 60,
                Parent = iconSizePanel
            };

            foreach (string dropDownValue in Enum.GetNames(typeof(SelectStatsWindowIconSize)))
                iconSizeDropDown.Items.Add(dropDownValue);

            iconSizeDropDown.SelectedItem = services.SettingService.SelectStatsIconSizeSetting.Value.ToString();
            iconSizeDropDown.ValueChanged += (s, o) =>
            {
                services.SettingService.SelectStatsIconSizeSetting.Value = (SelectStatsWindowIconSize)Enum.Parse(typeof(SelectStatsWindowIconSize), iconSizeDropDown.SelectedItem);
            };

            services.SettingService.SelectStatsIconSizeSetting.SettingChanged += SelectStatsIconSizeSettingChanged;
            return iconSizePanel;
        }

        private static void AddButtonsToExpandCollapseAllCategories(
            Dictionary<string, SelectStatsControls> controlsByCategoryId, 
            List<StatCategory> categories, 
            Container parent)
        {
            var expandAllCategories = new StandardButton()
            {
                Text = "Expand All",
                BasicTooltipText = "Expand all categories",
                Width = 90,
                Left = 460,
                Parent = parent,
            };

            var collapseAllCategories = new StandardButton()
            {
                Text = "Collapse All",
                BasicTooltipText = "Collapse all sub categories and expand all super categories",
                Width = 90,
                Top = expandAllCategories.Top,
                Left = expandAllCategories.Right + 2,
                Parent = parent,
            };

            expandAllCategories.Click += (sender, e) =>
            {
                categories.Where(c => c.IsSuperCategory).ToList().ForEach(c => controlsByCategoryId[c.Id].CategoryFlowPanel.Expand());
                categories.Where(c => c.IsSubCategory).ToList().ForEach(c => controlsByCategoryId[c.Id].CategoryFlowPanel.Expand());
            };
            collapseAllCategories.Click += (sender, e) =>
            {
                // super categories are expanded on purpose when "collapse" is clicked
                categories.Where(c => c.IsSuperCategory).ToList().ForEach(c => controlsByCategoryId[c.Id].CategoryFlowPanel.Expand());
                categories.Where(c => c.IsSubCategory).ToList().ForEach(c => controlsByCategoryId[c.Id].CategoryFlowPanel.Collapse());
            };
        }

        private static void AddButtonsToSelectAndUnselectAllStats(Dictionary<string, SelectStatsControls> controlsByCategoryId, Container parent)
        {
            var selectStatsButton = new StandardButton()
            {
                Text = "Select all",
                BasicTooltipText = "Select all stats in all categories",
                Left = 654,
                Width = 80,
                Parent = parent,
            };

            var unselectStatsButton = new StandardButton()
            {
                Text = "Unselect all",
                BasicTooltipText = "Unselect all stats in all categories",
                Top = selectStatsButton.Top,
                Left = selectStatsButton.Right + 2,
                Width = 80,
                Parent = parent,
            };

            selectStatsButton.Click += (sender, e) => controlsByCategoryId.Values.SelectMany(c => c.StatContainers).ToList().ForEach(s => s.SelectStat());
            unselectStatsButton.Click += (sender, e) => controlsByCategoryId.Values.SelectMany(c => c.StatContainers).ToList().ForEach(s => s.UnselectStat());
        }

        private static void AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(List<SelectStatContainer> statContainers, Container parent, int locationRightShift)
        {
            var selectStatsButton = new StandardButton()
            {
                Text = "Select",
                BasicTooltipText = "Select all stats inside this category",
                Location = new Point(640 + locationRightShift, 5),
                Width = 80,
                Parent = parent,
            };

            var unselectStatsButton = new StandardButton()
            {
                Text = "Unselect",
                BasicTooltipText = "Unselect all stats inside this category",
                Location = new Point(selectStatsButton.Right + 2, 5),
                Width = 80,
                Parent = parent,
            };

            selectStatsButton.Click += (sender, e) => statContainers.ForEach(s => s.SelectStat());
            unselectStatsButton.Click += (sender, e) => statContainers.ForEach(s => s.UnselectStat());
        }

        private void SelectStatsIconSizeSettingChanged(object sender, ValueChangedEventArgs<SelectStatsWindowIconSize> e)
        {
            foreach (var statContainer in _controlsByCategoryId.Values.SelectMany(c => c.StatContainers))
                statContainer.SetIconSize((int)_services.SettingService.SelectStatsIconSizeSetting.Value);
        }

        private readonly Services _services;
        private readonly Label _noSearchResultsHintLabel = ControlFactory.CreateHintLabel(null, "No stats match search term!");
        private readonly Dictionary<string, SelectStatsControls> _controlsByCategoryId = new Dictionary<string, SelectStatsControls>();
        private const int HELP_LABEL_BORDER_SIZE = 10;
        private const int EXPANDED_HELP_HEIGHT = 280;
        private const int EXPANDED_HELP_WIDTH = 850;
        private const string SHOW_HELP_BUTTON_TEXT = "Show Help";
        private const string HIDE_HELP_BUTTON_TEXT = "Hide Help";
        private const int SUPER_CATEGORY_LOCATION_RIGHT_SHIFT = 14;
    }
}