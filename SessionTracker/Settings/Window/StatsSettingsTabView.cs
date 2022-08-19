using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Settings.SettingEntries;

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
                "Click the 'Hide all'-button and then select the stats you want to see by clicking on the\n" +
                "stats category buttons (e.g. 'PvP', 'Fractals') or by clicking on the individual stats below.\n" +
                "After that click on the 'Move visible to top'-button to make hiding or reordering easier.\n" +
                "You can reorder the stats with the up and down buttons.");

            ShowHideAndShowAllButtons(_visibilityCheckBoxByEntryId, trackedStatsSectionFlowPanel);
            ShowCategoryButtons(_visibilityCheckBoxByEntryId, trackedStatsSectionFlowPanel);
            ShowMoveVisibleToTopButton(_visibilityCheckBoxByEntryId, trackedStatsSectionFlowPanel);

            _entryRowsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(0, 5),
                ControlPadding = new Vector2(0, 5),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = trackedStatsSectionFlowPanel
            };

            ShowEntryRows(_model.Entries, _entryRowsFlowPanel);
        }
        private static void ShowHideAndShowAllButtons(Dictionary<string, Checkbox> visibilityCheckBoxByEntryId, FlowPanel entriesFlowPanel)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = entriesFlowPanel,
            };

            var hideAllButton = new StandardButton
            {
                Text = "Hide all",
                BasicTooltipText = "Hide all stats rows in UI.",
                Parent = buttonsFlowPanel
            };

            var showAllButton = new StandardButton
            {
                Text = "Show all",
                BasicTooltipText = "Show all stats rows in UI.",
                Parent = buttonsFlowPanel
            };

            hideAllButton.Click += (s, e) =>
            {
                foreach (var visibilityCheckBox in visibilityCheckBoxByEntryId.Values)
                    visibilityCheckBox.Checked = false;
            };

            showAllButton.Click += (s, e) =>
            {
                foreach (var visibilityCheckBox in visibilityCheckBoxByEntryId.Values)
                    visibilityCheckBox.Checked = true;
            };
        }

        private void ShowMoveVisibleToTopButton(Dictionary<string, Checkbox> visibilityCheckBoxByEntryId, FlowPanel entriesFlowPanel)
        {
            var moveVisibleEntryRowsToTopButton = new StandardButton
            {
                Text = "Move visible to top",
                BasicTooltipText = "Move all visible stats to the top for easier hiding or reordering.",
                Width = 200,
                Parent = entriesFlowPanel
            };

            moveVisibleEntryRowsToTopButton.Click += (s, e) => MoveVisibleEntriesToTop();
        }

        private void MoveVisibleEntriesToTop()
        {
            var sortedEntries = _model.Entries.OrderByDescending(entry => entry.IsVisible).ToList();
            _model.Entries.Clear();
            _model.Entries.AddRange(sortedEntries);
            UpdateEntryRows();
        }

        private void ShowCategoryButtons(Dictionary<string, Checkbox> visibilityCheckBoxByEntryId, FlowPanel entriesFlowPanel)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = entriesFlowPanel,
            };

            var buttonWidth = 100;

            var pvpButton = new StandardButton
            {
                Text = "PvP",
                BasicTooltipText = "Click to show pvp related stats.",
                Width = buttonWidth,
                Parent = buttonsFlowPanel
            };

            var wvwButton = new StandardButton
            {
                Text = "WvW",
                BasicTooltipText = "Click to show wvw related stats.",
                Width = buttonWidth,
                Parent = buttonsFlowPanel
            };

            var fractalsButton = new StandardButton
            {
                Text = "Fractals",
                BasicTooltipText = "Click to show fractal related stats.",
                Width = buttonWidth,
                Parent = buttonsFlowPanel
            };

            var strikesButton = new StandardButton
            {
                Text = "Strikes",
                BasicTooltipText = "Click to show strike related stats.",
                Width = buttonWidth,
                Parent = buttonsFlowPanel
            };

            var raidsButton = new StandardButton
            {
                Text = "Raids",
                BasicTooltipText = "Click to show raid related stats.",
                Width = buttonWidth,
                Parent = buttonsFlowPanel
            };

            var openWorldButton = new StandardButton
            {
                Text = "Open World",
                BasicTooltipText = "Click to show some open world related stats.",
                Width = buttonWidth,
                Parent = buttonsFlowPanel
            };

            pvpButton.Click += (s, e) =>
            {
                ShowByEntryIdStartingWith("pvp", visibilityCheckBoxByEntryId);
                ShowByCurrencyId(CurrencyIds.Pvp, visibilityCheckBoxByEntryId);
                visibilityCheckBoxByEntryId[EntryId.DEATHS].Checked = true;
                MoveVisibleEntriesToTop();
            };

            wvwButton.Click += (s, e) =>
            {
                ShowByEntryIdStartingWith("wvw", visibilityCheckBoxByEntryId);
                ShowByCurrencyId(CurrencyIds.Wvw, visibilityCheckBoxByEntryId);
                visibilityCheckBoxByEntryId[EntryId.DEATHS].Checked = true;
                MoveVisibleEntriesToTop();
            };

            fractalsButton.Click += (s, e) =>
            {
                ShowByCurrencyId(CurrencyIds.Fractal, visibilityCheckBoxByEntryId);
                MoveVisibleEntriesToTop();
            };

            strikesButton.Click += (s, e) =>
            {
                ShowByCurrencyId(CurrencyIds.Strike, visibilityCheckBoxByEntryId);
                MoveVisibleEntriesToTop();
            };

            raidsButton.Click += (s, e) =>
            {
                ShowByCurrencyId(CurrencyIds.Raid, visibilityCheckBoxByEntryId);
                MoveVisibleEntriesToTop();
            };

            openWorldButton.Click += (s, e) =>
            {
                ShowByCurrencyId(CurrencyIds.OpenWorld, visibilityCheckBoxByEntryId);
                MoveVisibleEntriesToTop();
            };
        }

        private static void ShowByEntryIdStartingWith(string searchTerm, Dictionary<string, Checkbox> visibilityCheckBoxByEntryId)
        {
            foreach (var checkBoxStringPair in visibilityCheckBoxByEntryId.Where(i => i.Key.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase)))
                checkBoxStringPair.Value.Checked = true;
        }

        private void ShowByCurrencyId(ReadOnlyCollection<int> currencyIds, Dictionary<string, Checkbox> visibilityCheckBoxByEntryId)
        {
            foreach (var entry in _model.Entries.Where(entry => currencyIds.Contains(entry.CurrencyId)))
                visibilityCheckBoxByEntryId[entry.Id].Checked = true;
        }

        private void ShowEntryRows(List<Entry> entries, FlowPanel entryRowsFlowPanel)
        {
            foreach (var entry in entries)
                ShowEntryRow(entryRowsFlowPanel, entry);
        }

        private void ShowEntryRow(FlowPanel entryRowsFlowPanel, Entry entry)
        {
            var entryFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                BackgroundColor = DetermineBackgroundColor(entry.IsVisible),
                Width = 400,
                HeightSizingMode = SizingMode.AutoSize,
                Parent = entryRowsFlowPanel
            };

            var checkBoxContainer = ControlFactory.CreateAdjustableChildLocationContainer(entryFlowPanel);

            var isVisibleCheckbox = new Checkbox
            {
                Checked = entry.IsVisible,
                BasicTooltipText = SHOW_HIDE_STAT_TOOLTIP,
                Size = new Point(16, 16),
                Location = new Point(5, 5),
                Parent = checkBoxContainer
            };

            _visibilityCheckBoxByEntryId[entry.Id] = isVisibleCheckbox;

            var moveEntryUpwardsButton = new GlowButton
            {
                Icon = _textureService.MoveUpTexture,
                ActiveIcon = _textureService.MoveUpActiveTexture,
                BasicTooltipText = "Move up",
                Size = new Point(25, 25),
                Parent = entryFlowPanel
            };

            var moveEntryDownwardsButton = new GlowButton()
            {
                Icon = _textureService.MoveDownTexture,
                ActiveIcon = _textureService.MoveDownActiveTexture,
                BasicTooltipText = "Move down",
                Size = new Point(25, 25),
                Parent = entryFlowPanel
            };

            var clickFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                HeightSizingMode = SizingMode.Fill,
                WidthSizingMode = SizingMode.Fill,
                BasicTooltipText = SHOW_HIDE_STAT_TOOLTIP,
                Parent = entryFlowPanel
            };

            var iconContainer = ControlFactory.CreateAdjustableChildLocationContainer(clickFlowPanel);
            var asyncTexture2D = _textureService.EntryTextureByEntryId[entry.Id];

            new Image(asyncTexture2D)
            {
                BasicTooltipText = entry.LabelTooltip,
                Size = new Point(24),
                Location = new Point(20, 0),
                Parent = iconContainer,
            };

            var labelContainer = ControlFactory.CreateAdjustableChildLocationContainer(clickFlowPanel);

            new Label
            {
                Text = entry.LabelText,
                BasicTooltipText = entry.LabelTooltip,
                AutoSizeWidth = true,
                AutoSizeHeight = true,
                Location = new Point(5, 3),
                Parent = labelContainer
            };

            clickFlowPanel.Click += (s, e) => isVisibleCheckbox.Checked = isVisibleCheckbox.Checked == false;

            isVisibleCheckbox.CheckedChanged += (s, e) =>
            {
                entry.IsVisible = e.Checked;
                entryFlowPanel.BackgroundColor = DetermineBackgroundColor(e.Checked);
                _model.UiHasToBeUpdated = true;
            };

            moveEntryUpwardsButton.Click += (s, e) =>
            {
                var index = _model.Entries.IndexOf(entry);

                const int firstEntryIndex = 0;
                if (index > firstEntryIndex)
                {
                    _model.Entries.Remove(entry);
                    _model.Entries.Insert(index - 1, entry);
                    UpdateEntryRows();
                }
            };

            moveEntryDownwardsButton.Click += (s, e) =>
            {
                var index = _model.Entries.IndexOf(entry);

                var lastEntryIndex = _model.Entries.Count - 1;
                if (index < lastEntryIndex)
                {
                    _model.Entries.Remove(entry);
                    _model.Entries.Insert(index + 1, entry);
                    UpdateEntryRows();
                }
            };
        }

        private static Color DetermineBackgroundColor(bool isVisible)
        {
            return isVisible
                ? VISIBLE_COLOR
                : NOT_VISIBLE_COLOR;
        }

        private void UpdateEntryRows()
        {
            var scrollDistance = _scrollbar.ScrollDistance;
            _entryRowsFlowPanel.ClearChildren();
            ShowEntryRows(_model.Entries, _entryRowsFlowPanel);
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
        private FlowPanel _entryRowsFlowPanel;
        private readonly Dictionary<string, Checkbox> _visibilityCheckBoxByEntryId = new Dictionary<string, Checkbox>();
        private static readonly Color VISIBLE_COLOR = new Color(17, 64, 9) * 0.9f;
        private static readonly Color NOT_VISIBLE_COLOR = new Color(Color.Black, 0.5f);
        private const string SHOW_HIDE_STAT_TOOLTIP = "Show or hide stat by clicking on the checkbox or directly on the row. Values for hidden stats are still tracked.";
    }
}