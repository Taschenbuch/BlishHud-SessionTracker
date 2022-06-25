using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.Services;

namespace SessionTracker.Settings
{
    public class ModuleSettingsView : View
    {
        public ModuleSettingsView(Model model, EntriesContainer entriesContainer, SettingService settingService, TextureService textureService)
        {
            _model            = model;
            _entriesContainer = entriesContainer;
            _settingService   = settingService;
            _textureService   = textureService;
        }

        protected override void Build(Container buildPanel)
        {
            _rootFlowPanel = ControlFactory.CreateSettingsRootFlowPanel(buildPanel);
            _scrollbar     = (Scrollbar)buildPanel.Children.FirstOrDefault(c => c is Scrollbar);

            var generalFlowPanel = ControlFactory.CreateSettingsGroupFlowPanel("General", _rootFlowPanel);
            CreateResetSessionButton(generalFlowPanel);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.LabelTypeSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.BackgroundOpacitySetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.FontSizeIndexSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.TitleLabelColorSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.ValueLabelColorSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.SessionValuesAreVisibleSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.TotalValuesAreVisibleSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.WindowIsVisibleEverywhere);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.WindowIsOnlyVisibleInWvwSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.DragWindowWithMouseIsEnabledSetting);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.CornerIconIsVisible);
            ControlFactory.CreateSetting(generalFlowPanel, buildPanel.Width, _settingService.UiVisibilityKeyBindingSetting);

            _entriesFlowPanel                = ControlFactory.CreateSettingsGroupFlowPanel("Tracked stats", _rootFlowPanel);
            _entriesFlowPanel.ControlPadding = new Vector2(0, 10);
            ControlFactory.CreateHintLabel(
                "Hint: click the Hide-all-button and then select the stats you want to see.\n" +
                "Currencies are hidden by default because there are so many of them.",
                _entriesFlowPanel);
            ShowHideAndShowAllButtons(_visibilityCheckBoxes, _entriesFlowPanel);
            ShowEntryRows(_entriesFlowPanel);
        }

        private void CreateResetSessionButton(Container parent)
        {
            var resetSessionButton = new StandardButton
            {
                Text             = "Reset session",
                BasicTooltipText = "Reset the current session. All values will start at 0 again after a short time.",
                Parent           = parent
            };

            resetSessionButton.Click += (s, e) => _entriesContainer.ResetSession();
        }

        private static void ShowHideAndShowAllButtons(List<Checkbox> visibilityCheckBoxes, FlowPanel entriesFlowPanel)
        {
            var buttonsFlowPanel = new FlowPanel
            {
                FlowDirection    = ControlFlowDirection.SingleLeftToRight,
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent           = entriesFlowPanel,
            };

            var hideAllButton = new StandardButton
            {
                Text             = "Hide all",
                BasicTooltipText = "Hide all stats rows in UI.",
                Parent           = buttonsFlowPanel
            };

            var showAllButton = new StandardButton
            {
                Text             = "Show all",
                BasicTooltipText = "Show all stats rows in UI.",
                Parent           = buttonsFlowPanel
            };

            hideAllButton.Click += (s, e) =>
            {
                foreach (var visibilityCheckBox in visibilityCheckBoxes)
                    visibilityCheckBox.Checked = false;
            };

            showAllButton.Click += (s, e) =>
            {
                foreach (var visibilityCheckBox in visibilityCheckBoxes)
                    visibilityCheckBox.Checked = true;
            };
        }

        private void ShowEntryRows(FlowPanel entriesFlowPanel)
        {
            foreach (var entry in _model.Entries)
                ShowEntryRow(entriesFlowPanel, entry);
        }

        private void ShowEntryRow(FlowPanel entriesFlowPanel, Entry entry)
        {
            var entryFlowPanel = new FlowPanel
            {
                FlowDirection    = ControlFlowDirection.SingleLeftToRight,
                BackgroundColor  = new Color(Color.Black, 0.5f),
                Width            = 400,
                HeightSizingMode = SizingMode.AutoSize,
                Parent           = entriesFlowPanel
            };

            var checkBoxContainer = ControlFactory.CreateAdjustableChildLocationContainer(entryFlowPanel);

            var isVisibleCheckbox = new Checkbox
            {
                Checked          = entry.IsVisible,
                BasicTooltipText = "Show in UI. Values for hidden stats are still tracked.",
                Size             = new Point(16, 16),
                Location         = new Point(5, 5),
                Parent           = checkBoxContainer
            };

            _visibilityCheckBoxes.Add(isVisibleCheckbox);

            var moveEntryUpwardsButton = new GlowButton
            {
                Icon             = _textureService.MoveUpTexture,
                ActiveIcon       = _textureService.MoveUpActiveTexture,
                BasicTooltipText = "Move up",
                Size             = new Point(25, 25),
                Parent           = entryFlowPanel
            };

            var moveEntryDownwardsButton = new GlowButton()
            {
                Icon             = _textureService.MoveDownTexture,
                ActiveIcon       = _textureService.MoveDownActiveTexture,
                BasicTooltipText = "Move down",
                Size             = new Point(25, 25),
                Parent           = entryFlowPanel
            };

            if (entry.HasIcon)
            {
                var iconContainer = ControlFactory.CreateAdjustableChildLocationContainer(entryFlowPanel);

                var asyncTexture2D = _textureService.EntryTextureByEntryId[entry.Id];
                new Image(asyncTexture2D)
                {
                    BasicTooltipText = entry.LabelTooltip,
                    Size             = new Point(24),
                    Location         = new Point(20, 0),
                    Parent           = iconContainer,
                };
            }

            var labelContainer = ControlFactory.CreateAdjustableChildLocationContainer(entryFlowPanel);

            new Label
            {
                Text             = entry.LabelText,
                BasicTooltipText = entry.LabelTooltip,
                AutoSizeWidth    = true,
                AutoSizeHeight   = true,
                Location         = new Point(5, 3),
                Parent           = labelContainer
            };

            isVisibleCheckbox.CheckedChanged += (s, e) =>
            {
                entry.IsVisible         = e.Checked;
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

        private void UpdateEntryRows()
        {
            var scrollDistance = _scrollbar.ScrollDistance;
            _entriesFlowPanel.ClearChildren();
            ShowEntryRows(_entriesFlowPanel);
            _model.UiHasToBeUpdated = true;

            Task.Run(async () =>
            {
                await Task.Delay(50);
                _scrollbar.ScrollDistance = scrollDistance;
            });
        }

        private readonly Model _model;
        private readonly EntriesContainer _entriesContainer;
        private readonly SettingService _settingService;
        private readonly TextureService _textureService;
        private Scrollbar _scrollbar;
        private FlowPanel _rootFlowPanel;
        private FlowPanel _entriesFlowPanel;
        private readonly List<Checkbox> _visibilityCheckBoxes = new List<Checkbox>();
    }
}