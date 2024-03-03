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

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);
            _scrollbar = (Scrollbar)buildPanel.Children.FirstOrDefault(c => c is Scrollbar);

            var trackedStatsSectionFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("Tracked Stats", _rootFlowPanel);

            ControlFactory.CreateHintLabel(
                trackedStatsSectionFlowPanel,
                "Click the hide 'All'-button and then show or hide the stats you want to see by clicking on the\n" + // todo x text wird nicht mehr passen
                "stats category buttons (e.g. 'PvP', 'Fractals') or by clicking on the individual stats below.\n" +
                "After that click on the 'Move visible to top'-button to make hiding or reordering easier.\n" +
                "You can reorder the stats with the up and down buttons.");

            ShowMoveVisibleToTopButton(trackedStatsSectionFlowPanel); // todo x überflüssig

            _statRowsFlowPanel = new FlowPanel
            {
                FlowDirection = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(0, 5),
                ControlPadding = new Vector2(0, 5),
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode = SizingMode.AutoSize,
                Parent = trackedStatsSectionFlowPanel
            };

            ShowStatRows(_services.Model.Stats, _statRowsFlowPanel);
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
            var sortedStats = _services.Model.Stats.OrderByDescending(stat => stat.IsVisible).ToList();
            _services.Model.Stats.Clear();
            _services.Model.Stats.AddRange(sortedStats);
            UpdateStatRows();
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

            var clickFlowPanel = new FlowPanel()
            {
                FlowDirection = ControlFlowDirection.SingleLeftToRight,
                HeightSizingMode = SizingMode.Fill,
                WidthSizingMode = SizingMode.Fill,
                BasicTooltipText = SHOW_HIDE_STAT_TOOLTIP,
                Parent = statFlowPanel
            };

            var iconContainer = ControlFactory.CreateAdjustableChildLocationContainer(clickFlowPanel);
            var asyncTexture2D = _services.TextureService.StatTextureByStatId[stat.Id];

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
                _services.Model.UiHasToBeUpdated = true;
            };

            moveStatUpwardsButton.Click += (s, e) =>
            {
                var index = _services.Model.Stats.IndexOf(stat);

                const int firstStatIndex = 0;
                if (index > firstStatIndex)
                {
                    _services.Model.Stats.Remove(stat);
                    _services.Model.Stats.Insert(index - 1, stat);
                    UpdateStatRows();
                }
            };

            moveStatDownwardsButton.Click += (s, e) =>
            {
                var index = _services.Model.Stats.IndexOf(stat);

                var lastStatIndex = _services.Model.Stats.Count - 1;
                if (index < lastStatIndex)
                {
                    _services.Model.Stats.Remove(stat);
                    _services.Model.Stats.Insert(index + 1, stat);
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
            ShowStatRows(_services.Model.Stats, _statRowsFlowPanel);
            _services.Model.UiHasToBeUpdated = true;

            Task.Run(async () =>
            {
                await Task.Delay(_services.SettingService.ScrollbarFixDelay.Value);
                _scrollbar.ScrollDistance = scrollDistance;
            });
        }

        private Scrollbar _scrollbar;
        private FlowPanel _rootFlowPanel;
        private FlowPanel _statRowsFlowPanel;
        private readonly Services _services;
        private readonly Dictionary<string, Checkbox> _visibilityCheckBoxByStatId = new Dictionary<string, Checkbox>();
        private static readonly Color VISIBLE_COLOR = new Color(17, 64, 9) * 0.9f;
        private static readonly Color NOT_VISIBLE_COLOR = new Color(Color.Black, 0.5f);
        private const string SHOW_HIDE_STAT_TOOLTIP = "Show or hide stat by clicking on the checkbox or directly on the row. Values for hidden stats are still tracked.";
    }
}