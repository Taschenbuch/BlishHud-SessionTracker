using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD;
using SessionTracker.OtherServices;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Controls;
using SessionTracker.Models;
using Blish_HUD.Graphics.UI;

namespace SessionTracker.SelectStats
{
    public class SelectStatsSettingsTabView : View
    {
        public SelectStatsSettingsTabView(Services services)
        {
            _services = services;
        }

        protected override void Unload()
        {
            _services.SettingService.SelectStatsIconSizeSetting.SettingChanged -= SelectStatsIconSizeSettingChanged;
            // todo x fehlt da ein base.Unload() aufruf?
        }

        protected override void Build(Container buildPanel)
        {
            var rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);
            var settingsFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Select Stats", rootFlowPanel);
            settingsFlowPanel.ControlPadding = new Vector2(0, 10); // todo x besser selbst steuern für jeden Bereich?
            AddHint(settingsFlowPanel);
            var topControlsContainer = ControlFactory.CreateAdjustableChildLocationContainer(settingsFlowPanel);
            CategoryButtonsCreator.AddSelectStatsFromGroupButtons(_statContainersByStatId, _services.Model.Stats, topControlsContainer);
            CategoryButtonsCreator.AddUnselectStatsFromGroupButtons(_statContainersByStatId, _services.Model.Stats, topControlsContainer);
            var searchTextBox = new StatsSearchTextBox(
                _services,
                _noSearchResultsHintLabel,
                _statContainersByStatId,
                _controlsByCategoryId,
                settingsFlowPanel, 
                topControlsContainer);

            AddIconSizeDropdown(_services, topControlsContainer, searchTextBox);
            AddButtonsToExpandCollapseAllCategories(_controlsByCategoryId, topControlsContainer);

            // todo x in class auslagern?
            foreach (var statCategory in _services.Model.StatCategories)
            {
                var categoryContainer = ControlFactory.CreateAdjustableChildLocationContainer(settingsFlowPanel);
                var categoryFlowPanel = new FlowPanel()
                {
                    Title = statCategory.Name.Localized,
                    FlowDirection = ControlFlowDirection.LeftToRight,
                    Width = 845,
                    HeightSizingMode = SizingMode.AutoSize,
                    OuterControlPadding = new Vector2(2),
                    CanCollapse = true,
                    ShowBorder = true,
                    Parent = categoryContainer
                };

                _controlsByCategoryId[statCategory.Id] = new SelectStatsControls
                {
                    CategoryContainer = categoryContainer,
                    CategoryFlowPanel = categoryFlowPanel
                };

                AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(statCategory, _controlsByCategoryId, categoryContainer);

                var statsInCategory = _services.Model.Stats
                    .Where(s => s.CategoryId == statCategory.Id)
                    .OrderBy(s => s.PositionInsideCategory);

                foreach (var stat in statsInCategory) // todo x man könnte getrennt categories und statContainer erzeugen. Danach erst statContainer Parents setzen.
                {
                    var statContainer = new SelectStatContainer(stat, _services, categoryFlowPanel);
                    _controlsByCategoryId[statCategory.Id].StatContainers.Add(statContainer);
                    _statContainersByStatId[stat.Id] = statContainer;
                }
            }
        }

        private void AddIconSizeDropdown(Services _services, Container parent, TextBox searchTextBox)
        {
            // todo x in class mit Dispose auslagern?
            var iconSizeSettingView = ControlFactory.CreateSetting(parent, _services.SettingService.SelectStatsIconSizeSetting);
            iconSizeSettingView.Top = searchTextBox.Top;
            iconSizeSettingView.Left = searchTextBox.Right + 10;
            _services.SettingService.SelectStatsIconSizeSetting.SettingChanged += SelectStatsIconSizeSettingChanged;
        }

        private static void AddButtonsToExpandCollapseAllCategories(
            Dictionary<string, SelectStatsControls> controlsByCategoryId,
            Container parent)
        {
            var expandAllCategories = new StandardButton()
            {
                Text = "Expand All",
                Width = 100,
                Top = 30,
                Left = 750,
                Parent = parent,
            };

            var collapseAllCategories = new StandardButton()
            {
                Text = "Collapse All",
                Width = 100,
                Top = expandAllCategories.Bottom + 2,
                Left = expandAllCategories.Left,
                Parent = parent,
            };

            expandAllCategories.Click += (sender, e) => controlsByCategoryId.Values.ToList().ForEach(c => c.CategoryFlowPanel.Expand());
            collapseAllCategories.Click += (sender, e) => controlsByCategoryId.Values.ToList().ForEach(c => c.CategoryFlowPanel.Collapse());
        }

        private static void AddHint(Container parent)
        {
            ControlFactory.CreateHintLabel(
                parent,
                "- To select which stats are tracked click on the stat icons or use the buttons to (un)select multiple stats.\n" +
                "- Not selected stats are displayed transparent\n" +
                "- It is recommended to enable 'hide stats with value = 0' setting and/or 'fixed window height' setting if you want to track many stats.\n" +
                "- After selecting the stats, you can rearrange them in the other settings tab");
        }

        private static void AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(
            StatCategory statCategory,
            Dictionary<string, SelectStatsControls> controlsByCategoryId,
            LocationContainer categoryContainer
            )
        {
            var selectSingleCategoryStatsButton = new StandardButton()
            {
                Text = "Select",
                Location = new Point(655, 5),
                Width = 80,
                Parent = categoryContainer,
            };

            var unselectSingleCategoryStatsButton = new StandardButton()
            {
                Text = "Unselect",
                Location = new Point(selectSingleCategoryStatsButton.Right + 2, 5),
                Width = 80,
                Parent = categoryContainer,
            };

            selectSingleCategoryStatsButton.Click += (sender, e) => controlsByCategoryId[statCategory.Id].StatContainers.ForEach(s => s.SelectStat());
            unselectSingleCategoryStatsButton.Click += (sender, e) => controlsByCategoryId[statCategory.Id].StatContainers.ForEach(s => s.UnselectStat());
        }

        private void SelectStatsIconSizeSettingChanged(object sender, ValueChangedEventArgs<SettingEntries.SelectStatsWindowIconSize> e)
        {
            foreach (var statContainer in _controlsByCategoryId.Values.SelectMany(s => s.StatContainers))
                statContainer.SetIconSize((int)_services.SettingService.SelectStatsIconSizeSetting.Value);
        }

        private readonly Services _services;
        private readonly Label _noSearchResultsHintLabel = ControlFactory.CreateHintLabel(null, "No stats match search term!");
        private readonly Dictionary<string, SelectStatContainer> _statContainersByStatId = new Dictionary<string, SelectStatContainer>();
        private readonly Dictionary<string, SelectStatsControls> _controlsByCategoryId = new Dictionary<string, SelectStatsControls>();
    }
}