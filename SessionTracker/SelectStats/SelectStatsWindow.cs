using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Blish_HUD;
using SessionTracker.OtherServices;
using System.Collections.Generic;
using System.Linq;
using SessionTracker.Controls;
using SessionTracker.Models;

namespace SessionTracker.SelectStats
{
    public class SelectStatsWindow : StandardWindow
    {
        public SelectStatsWindow(Services services)
            : base(services.TextureService.SelectStatsWindowBackgroundTexture, new Rectangle(40, 30, 950, 950), new Rectangle(60, 40, 950, 930))
        {
            _services = services;

            Emblem = services.TextureService.SettingsWindowEmblemTexture; // hack: has to be first to prevent bug of emblem not being visible
            Title = "Select Stats";
            Location = new Point(300, 300);
            SavesPosition = true;
            Id = "Ecksofa.SessionTracker: select stats window";
            Parent = GameService.Graphics.SpriteScreen;

            var rootFlowPanel = AddRootFlowPanel(this);
            AddHint(rootFlowPanel);
            var topControlsContainer = ControlFactory.CreateAdjustableChildLocationContainer(rootFlowPanel);
            CategoryButtonsCreator.AddSelectStatsFromGroupButtons(_statContainersByStatId, services.Model.Stats, topControlsContainer);
            CategoryButtonsCreator.AddUnselectStatsFromGroupButtons(_statContainersByStatId, services.Model.Stats, topControlsContainer);
            var searchTextBox = new StatsSearchTextBox(
                services,
                _noSearchResultsHintLabel,
                _statContainersByStatId,
                _controlsByCatalogId,
                rootFlowPanel, 
                topControlsContainer);

            AddIconSizeDropdown(services, topControlsContainer, searchTextBox);
            AddButtonsToExpandCollapseAllCategories(_controlsByCatalogId, topControlsContainer);

            // todo x in class auslagern?
            foreach (var statCategory in services.Model.StatCategories)
            {
                var categoryContainer = ControlFactory.CreateAdjustableChildLocationContainer(rootFlowPanel);
                var categoryFlowPanel = new FlowPanel()
                {
                    Title = statCategory.Name.Localized,
                    FlowDirection = ControlFlowDirection.LeftToRight,
                    Width = 890,
                    HeightSizingMode = SizingMode.AutoSize,
                    OuterControlPadding = new Vector2(2),
                    CanCollapse = true,
                    ShowBorder = true,
                    Parent = categoryContainer
                };

                _controlsByCatalogId[statCategory.Id] = (categoryContainer,  categoryFlowPanel, new List<SelectStatContainer>());
                AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(statCategory, _controlsByCatalogId, categoryContainer);

                var statsInCategory = services.Model.Stats
                    .Where(s => s.CategoryId == statCategory.Id)
                    .OrderBy(s => s.PositionInsideCategory);

                foreach (var stat in statsInCategory) // todo x man könnte getrennt categories und statContainer erzeugen. Danach erst statContainer Parents setzen.
                {
                    var statContainer = new SelectStatContainer(stat, services, categoryFlowPanel);
                    _controlsByCatalogId[statCategory.Id].StatContainers.Add(statContainer);
                    _statContainersByStatId[stat.Id] = statContainer;
                }
            }
        }

        private static FlowPanel AddRootFlowPanel(Container parent)
        {
            return new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                CanScroll = true,
                OuterControlPadding = new Vector2(10, 0),
                ControlPadding = new Vector2(0, 10),
                Width = 920, // fixed width to not cutoff scrollbar
                HeightSizingMode = SizingMode.Fill,
                Parent = parent,
            };
        }

        private void AddIconSizeDropdown(Services services, Container parent, TextBox searchTextBox)
        {
            // todo x in class mit Dispose auslagern?
            var iconSizeSettingView = ControlFactory.CreateSetting(parent, services.SettingService.SelectStatsIconSizeSetting);
            iconSizeSettingView.Top = searchTextBox.Top;
            iconSizeSettingView.Left = searchTextBox.Right + 10;
            services.SettingService.SelectStatsIconSizeSetting.SettingChanged += SelectStatsIconSizeSettingChanged;
        }

        private static void AddButtonsToExpandCollapseAllCategories(
            Dictionary<string, (Container CategoryContainer, FlowPanel CategoryFlowPanel, List<SelectStatContainer> StatContainers)> controlsByCatalogId, 
            Container parent)
        {
            var expandAllCategories = new StandardButton()
            {
                Text = "Expand All",
                Width = 100,
                Top = 30,
                Left = 790,
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

            expandAllCategories.Click += (sender, e) => controlsByCatalogId.Values.ToList().ForEach(c => c.CategoryFlowPanel.Expand());
            collapseAllCategories.Click += (sender, e) => controlsByCatalogId.Values.ToList().ForEach(c => c.CategoryFlowPanel.Collapse());
        }

        private static void AddHint(FlowPanel rootFlowPanel)
        {
            var hintPanel = new LocationContainer
            {
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                BackgroundColor = Color.Black * 0.6f,
                Parent = rootFlowPanel,
            };

            ControlFactory.CreateHintLabel(
                hintPanel,
                "- To select which stats are tracked click on the stat icons or use the buttons to (un)select multiple stats.\n" +
                "ICON: colorful = selected stat | dark-transparent = unselected stat \n" +
                "- It is recommended to enable 'hide stats with value = 0' setting and/or 'fixed window height' setting if you want to track many stats");
        }

        private static void AddButtonsToSelectAndUnselectAllStatsOfSingleCategory(
            StatCategory statCategory,
            Dictionary<string, (Container CategoryContainer, FlowPanel CategoryFlowPanel, List<SelectStatContainer> StatContainers)> controlsByCatalogId,
            LocationContainer categoryContainer
            )
        {
            var selectSingleCategoryStatsButton = new StandardButton()
            {
                Text = "Select",
                Location = new Point(690, 5),
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

            selectSingleCategoryStatsButton.Click += (sender, e) => controlsByCatalogId[statCategory.Id].StatContainers.ForEach(s => s.SelectStat());
            unselectSingleCategoryStatsButton.Click += (sender, e) => controlsByCatalogId[statCategory.Id].StatContainers.ForEach(s => s.UnselectStat());
        }

        private void SelectStatsIconSizeSettingChanged(object sender, ValueChangedEventArgs<SettingEntries.SelectStatsWindowIconSize> e)
        {
            foreach (var statContainer in _controlsByCatalogId.Values.SelectMany(s => s.StatContainers))
                statContainer.SetIconSize((int)_services.SettingService.SelectStatsIconSizeSetting.Value);
        }

        protected override void DisposeControl()
        {
            _services.SettingService.SelectStatsIconSizeSetting.SettingChanged -= SelectStatsIconSizeSettingChanged;
            base.DisposeControl();
        }

        private readonly Services _services;
        private readonly Label _noSearchResultsHintLabel = ControlFactory.CreateHintLabel(null, "No stats match search term!");
        private readonly Dictionary<string, SelectStatContainer> _statContainersByStatId = new Dictionary<string, SelectStatContainer>();
        private readonly Dictionary<string, (Container CategoryContainer, FlowPanel CategoryFlowPanel, List<SelectStatContainer> StatContainers)> _controlsByCatalogId
            = new Dictionary<string, (Container CategoryContainer, FlowPanel CategoryFlowPanel, List<SelectStatContainer> StatContainers)>();
    }
}