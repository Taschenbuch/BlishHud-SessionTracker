using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using SessionTracker.Constants;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class StatsSettingsTabView : View
    {
        public StatsSettingsTabView(Model model, SettingService settingService, TextureService textureService)
        {
            _model = model;
            _settingService = settingService;
            _textureService = textureService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);
            _scrollbar = (Scrollbar)buildPanel.Children.FirstOrDefault(c => c is Scrollbar);

            var trackedStatsSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Tracked Stats", _rootFlowPanel);

            ControlFactory.CreateHintLabel(
                trackedStatsSectionFlowPanel,
                "Click the hide 'All'-button and then show or hide the stats you want to see by clicking on the\n" +
                "stats category buttons (e.g. 'PvP', 'Fractals') or by clicking on the individual stats below.\n" +
                "After that click on the 'Move visible to top'-button to make hiding or reordering easier.\n" +
                "You can reorder the stats with the up and down buttons.");

            ShowCategoryButtons(_visibilityCheckBoxByStatId, trackedStatsSectionFlowPanel);
            HideCategoryButtons(_visibilityCheckBoxByStatId, trackedStatsSectionFlowPanel);
            ShowMoveVisibleToTopButton(trackedStatsSectionFlowPanel);

            _statRowsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(0, 5),
                ControlPadding = new Vector2(0, 5),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = trackedStatsSectionFlowPanel
            };

            ShowStatRows(_model.Stats, _statRowsFlowPanel);
        }

        private void ShowMoveVisibleToTopButton(FlowPanel statsFlowPanel)
        {
            var moveVisibleStatRowsToTopButton = new StandardButton
            {
                Text = "Move visible to top",
                BasicTooltipText = "Move all visible stats to the top for easier hiding or reordering.",
                Width = 200,
                Parent = statsFlowPanel
            };

            moveVisibleStatRowsToTopButton.Click += (s, e) => MoveVisibleStatsToTop();
        }

        private void MoveVisibleStatsToTop()
        {
            var sortedStats = _model.Stats.OrderByDescending(stat => stat.IsVisible).ToList();
            _model.Stats.Clear();
            _model.Stats.AddRange(sortedStats);
            UpdateStatRows();
        }

        private void ShowCategoryButtons(Dictionary<string, Checkbox> visibilityCheckBoxByStatId, FlowPanel statsFlowPanel)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = statsFlowPanel,
            };

            new Label
            {
                Text = "Show",
                Location = new Point(5, 4),
                Width = 50,
                Parent = ControlFactory.CreateAdjustableChildLocationContainer(buttonsFlowPanel)
            };

            var showAllButton = new StandardButton
            {
                Text = "All",
                BasicTooltipText = "Show all stats rows in UI.",
                Width = 50,
                Parent = buttonsFlowPanel
            };

            var pvpButton = new StandardButton
            {
                Text = "PvP",
                BasicTooltipText = "Click to show pvp related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var wvwButton = new StandardButton
            {
                Text = "WvW",
                BasicTooltipText = "Click to show wvw related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var fractalsButton = new StandardButton
            {
                Text = "Fractals",
                BasicTooltipText = "Click to show fractal related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var strikesButton = new StandardButton
            {
                Text = "Strikes",
                BasicTooltipText = "Click to show strike related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var raidsButton = new StandardButton
            {
                Text = "Raids",
                BasicTooltipText = "Click to show raid related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var openWorldButton = new StandardButton
            {
                Text = "Open World",
                BasicTooltipText = "Click to show some open world related stats.",
                Width = 100,
                Parent = buttonsFlowPanel
            };

            showAllButton.Click += (s, e) =>
            {
                foreach (var visibilityCheckBox in visibilityCheckBoxByStatId.Values)
                    visibilityCheckBox.Checked = true;
            };

            pvpButton.Click += (s, e) =>
            {
                ShowOrHideByStatIdStartingWith("pvp", visibilityCheckBoxByStatId, true);
                ShowOrHideByCurrencyId(CurrencyIds.Pvp, visibilityCheckBoxByStatId, true);
                visibilityCheckBoxByStatId[StatId.DEATHS].Checked = true;
                MoveVisibleStatsToTop();
            };

            wvwButton.Click += (s, e) =>
            {
                ShowOrHideByStatIdStartingWith("wvw", visibilityCheckBoxByStatId, true);
                ShowOrHideByCurrencyId(CurrencyIds.Wvw, visibilityCheckBoxByStatId, true);
                ShowOrHideByItemId(ItemIds.Wvw, visibilityCheckBoxByStatId, true);
                visibilityCheckBoxByStatId[StatId.DEATHS].Checked = true;
                MoveVisibleStatsToTop();
            };

            fractalsButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.Fractal, visibilityCheckBoxByStatId, true);
                MoveVisibleStatsToTop();
            };

            strikesButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.Strike, visibilityCheckBoxByStatId, true);
                MoveVisibleStatsToTop();
            };

            raidsButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.Raid, visibilityCheckBoxByStatId, true);
                MoveVisibleStatsToTop();
            };

            openWorldButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.OpenWorld, visibilityCheckBoxByStatId, true);
                MoveVisibleStatsToTop();
            };
        }

        private void HideCategoryButtons(Dictionary<string, Checkbox> visibilityCheckBoxByStatId, FlowPanel statsFlowPanel)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = statsFlowPanel,
            };

            new Label
            {
                Text = "Hide",
                Location = new Point(5, 4),
                Width = 50,
                Parent = ControlFactory.CreateAdjustableChildLocationContainer(buttonsFlowPanel)
            };

            var hideAllButton = new StandardButton
            {
                Text = "All",
                BasicTooltipText = "Hide all stats rows in UI.",
                Width = 50,
                Parent = buttonsFlowPanel
            };

            var pvpButton = new StandardButton
            {
                Text = "PvP",
                BasicTooltipText = "Click to show pvp related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var wvwButton = new StandardButton
            {
                Text = "WvW",
                BasicTooltipText = "Click to show wvw related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var fractalsButton = new StandardButton
            {
                Text = "Fractals",
                BasicTooltipText = "Click to show fractal related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var strikesButton = new StandardButton
            {
                Text = "Strikes",
                BasicTooltipText = "Click to show strike related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var raidsButton = new StandardButton
            {
                Text = "Raids",
                BasicTooltipText = "Click to show raid related stats.",
                Width = 80,
                Parent = buttonsFlowPanel
            };

            var openWorldButton = new StandardButton
            {
                Text = "Open World",
                BasicTooltipText = "Click to show some open world related stats.",
                Width = 100,
                Parent = buttonsFlowPanel
            };

            hideAllButton.Click += (s, e) =>
            {
                foreach (var visibilityCheckBox in visibilityCheckBoxByStatId.Values)
                    visibilityCheckBox.Checked = false;
            };

            pvpButton.Click += (s, e) =>
            {
                ShowOrHideByStatIdStartingWith("pvp", visibilityCheckBoxByStatId, false);
                ShowOrHideByCurrencyId(CurrencyIds.Pvp, visibilityCheckBoxByStatId, false);
                visibilityCheckBoxByStatId[StatId.DEATHS].Checked = false;
                MoveVisibleStatsToTop();
            };

            wvwButton.Click += (s, e) =>
            {
                ShowOrHideByStatIdStartingWith("wvw", visibilityCheckBoxByStatId, false);
                ShowOrHideByCurrencyId(CurrencyIds.Wvw, visibilityCheckBoxByStatId, false);
                ShowOrHideByItemId(ItemIds.Wvw, visibilityCheckBoxByStatId, false);
                visibilityCheckBoxByStatId[StatId.DEATHS].Checked = false;
                MoveVisibleStatsToTop();
            };

            fractalsButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.Fractal, visibilityCheckBoxByStatId, false);
                MoveVisibleStatsToTop();
            };

            strikesButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.Strike, visibilityCheckBoxByStatId, false);
                MoveVisibleStatsToTop();
            };

            raidsButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.Raid, visibilityCheckBoxByStatId, false);
                MoveVisibleStatsToTop();
            };

            openWorldButton.Click += (s, e) =>
            {
                ShowOrHideByCurrencyId(CurrencyIds.OpenWorld, visibilityCheckBoxByStatId, false);
                MoveVisibleStatsToTop();
            };
        }


        private static void ShowOrHideByStatIdStartingWith(string searchTerm, Dictionary<string, Checkbox> visibilityCheckBoxByStatId, bool isShown)
        {
            foreach (var checkBoxStringPair in visibilityCheckBoxByStatId.Where(i => i.Key.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)))
                checkBoxStringPair.Value.Checked = isShown;
        }

        private void ShowOrHideByCurrencyId(ReadOnlyCollection<int> currencyIds, Dictionary<string, Checkbox> visibilityCheckBoxByStatId, bool isShown)
        {
            foreach (var stat in _model.Stats.Where(stat => currencyIds.Contains(stat.ApiId)))
                visibilityCheckBoxByStatId[stat.Id].Checked = isShown;
        }

        private void ShowOrHideByItemId(ReadOnlyCollection<int> itemIds, Dictionary<string, Checkbox> visibilityCheckBoxByStatId, bool isShown)
        {
            foreach (var stat in _model.Stats.Where(stat => itemIds.Contains(stat.ApiId)))
                visibilityCheckBoxByStatId[stat.Id].Checked = isShown;
        }

        private void ShowStatRows(List<Stat> stats, FlowPanel statRowsFlowPanel)
        {
            foreach (var stat in stats)
                ShowStatRow(statRowsFlowPanel, stat);
        }

        private void ShowStatRow(FlowPanel statRowsFlowPanel, Stat stat)
        {
            var statFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                BackgroundColor = DetermineBackgroundColor(stat.IsVisible),
                Width = 400,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = statRowsFlowPanel
            };

            var checkBoxContainer = ControlFactory.CreateAdjustableChildLocationContainer(statFlowPanel);

            var isVisibleCheckbox = new Checkbox
            {
                Checked = stat.IsVisible,
                BasicTooltipText = SHOW_HIDE_STAT_TOOLTIP,
                Size = new Point(16, 16),
                Location = new Point(5, 5),
                Parent = checkBoxContainer
            };

            _visibilityCheckBoxByStatId[stat.Id] = isVisibleCheckbox;

            var moveStatUpwardsButton = new GlowButton
            {
                Icon = _textureService.MoveUpTexture,
                ActiveIcon = _textureService.MoveUpActiveTexture,
                BasicTooltipText = "Move up",
                Size = new Point(25, 25),
                Parent = statFlowPanel
            };

            var moveStatDownwardsButton = new GlowButton()
            {
                Icon = _textureService.MoveDownTexture,
                ActiveIcon = _textureService.MoveDownActiveTexture,
                BasicTooltipText = "Move down",
                Size = new Point(25, 25),
                Parent = statFlowPanel
            };

            var clickFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                HeightSizingMode = SizingMode.Fill,
                WidthSizingMode = SizingMode.Fill,
                BasicTooltipText = SHOW_HIDE_STAT_TOOLTIP,
                Parent = statFlowPanel
            };

            var iconContainer = ControlFactory.CreateAdjustableChildLocationContainer(clickFlowPanel);
            var asyncTexture2D = _textureService.StatTextureByStatId[stat.Id];

            new Image(asyncTexture2D)
            {
                BasicTooltipText = stat.GetTextWithNameAndDescription(),
                Size = new Point(24),
                Location = new Point(20, 0),
                Parent = iconContainer,
            };

            var labelContainer = ControlFactory.CreateAdjustableChildLocationContainer(clickFlowPanel);

            new Label
            {
                Text             = stat.Name.Localized,
                BasicTooltipText = stat.GetTextWithNameAndDescription(),
                AutoSizeWidth    = true,
                AutoSizeHeight   = true,
                Location         = new Point(5, 3),
                Parent           = labelContainer
            };

            clickFlowPanel.Click += (s, e) => isVisibleCheckbox.Checked = isVisibleCheckbox.Checked == false;

            isVisibleCheckbox.CheckedChanged += (s, e) =>
            {
                stat.IsVisible = e.Checked;
                statFlowPanel.BackgroundColor = DetermineBackgroundColor(e.Checked);
                _model.UiHasToBeUpdated = true;
            };

            moveStatUpwardsButton.Click += (s, e) =>
            {
                var index = _model.Stats.IndexOf(stat);

                const int firstStatIndex = 0;
                if (index > firstStatIndex)
                {
                    _model.Stats.Remove(stat);
                    _model.Stats.Insert(index - 1, stat);
                    UpdateStatRows();
                }
            };

            moveStatDownwardsButton.Click += (s, e) =>
            {
                var index = _model.Stats.IndexOf(stat);

                var lastStatIndex = _model.Stats.Count - 1;
                if (index < lastStatIndex)
                {
                    _model.Stats.Remove(stat);
                    _model.Stats.Insert(index + 1, stat);
                    UpdateStatRows();
                }
            };
        }

        private static Color DetermineBackgroundColor(bool isVisible)
        {
            return isVisible
                ? VISIBLE_COLOR
                : NOT_VISIBLE_COLOR;
        }

        private void UpdateStatRows()
        {
            var scrollDistance = _scrollbar.ScrollDistance;
            _statRowsFlowPanel.ClearChildren();
            ShowStatRows(_model.Stats, _statRowsFlowPanel);
            _model.UiHasToBeUpdated = true;

            Task.Run(async () =>
            {
                await Task.Delay(_settingService.ScrollbarFixDelay.Value);
                _scrollbar.ScrollDistance = scrollDistance;
            });
        }

        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly TextureService _textureService;
        private Scrollbar _scrollbar;
        private FlowPanel _rootFlowPanel;
        private FlowPanel _statRowsFlowPanel;
        private readonly Dictionary<string, Checkbox> _visibilityCheckBoxByStatId = new Dictionary<string, Checkbox>();
        private static readonly Color VISIBLE_COLOR = new Color(17, 64, 9) * 0.9f;
        private static readonly Color NOT_VISIBLE_COLOR = new Color(Color.Black, 0.5f);
        private const string SHOW_HIDE_STAT_TOOLTIP = "Show or hide stat by clicking on the checkbox or directly on the row. Values for hidden stats are still tracked.";
    }
}