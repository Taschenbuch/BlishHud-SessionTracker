﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SessionTracker.Controls.Hint;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Services.Api;
using SessionTracker.Settings.SettingEntries;
using SessionTracker.Settings.Window;
using SessionTracker.Value.Text;
using SessionTracker.Value.Tooltip;
using Color = Microsoft.Xna.Framework.Color;

namespace SessionTracker.Controls
{
    public class EntriesContainer : RelativePositionAndMouseDraggableContainer
    {
        public EntriesContainer(Model model,
                                Gw2ApiManager gw2ApiManager,
                                TextureService textureService,
                                SettingsWindowService settingsWindowService,
                                SettingService settingService,
                                Logger logger)
            : base(settingService)
        {
            _model          = model;
            _gw2ApiManager  = gw2ApiManager;
            _textureService = textureService;
            _settingService = settingService;
            _logger         = logger;

            _updateState = new UpdateState(settingService);
            CreateUi(settingsWindowService);

            _valueLabelTextService = new ValueLabelTextService(_valueLabelByEntryId, _model, settingService, logger);
            _statTooltipService    = new StatTooltipService(_titleFlowPanelByEntryId, _valueLabelByEntryId, model, _settingService);

            settingService.HideStatsWithValueZeroSetting.SettingChanged  += OnHideStatsWithValueZeroSettingChanged;
            settingService.FontSizeIndexSetting.SettingChanged           += OnFontSizeIndexSettingChanged;
            settingService.BackgroundOpacitySetting.SettingChanged       += OnBackgroundOpacitySettingChanged;
            settingService.ValueLabelColorSetting.SettingChanged         += OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                        += OnUserChangedLanguageInBlishSettings;
        }
        
        protected override void DisposeControl()
        {
            _updateState.Dispose();
            _valueLabelTextService.Dispose();
            _settingService.HideStatsWithValueZeroSetting.SettingChanged  -= OnHideStatsWithValueZeroSettingChanged;
            _settingService.FontSizeIndexSetting.SettingChanged           -= OnFontSizeIndexSettingChanged;
            _settingService.BackgroundOpacitySetting.SettingChanged       -= OnBackgroundOpacitySettingChanged;
            _settingService.ValueLabelColorSetting.SettingChanged         -= OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                         -= OnUserChangedLanguageInBlishSettings;

            base.DisposeControl();
        }

        public override Control TriggerMouseInput(MouseEventType mouseEventType, MouseState ms)
        {
            var windowCanBeClickedThrough = !_settingService.DragWindowWithMouseIsEnabledSetting.Value
                                          && _settingService.WindowCanBeClickedThroughSetting.Value
                                          && GameService.Input.Keyboard.ActiveModifiers != ModifierKeys.Alt;

            return windowCanBeClickedThrough
                ? null
                : base.TriggerMouseInput(mouseEventType, ms);
        }

        public void ToggleVisibility()
        {
            _settingService.UiIsVisibleSetting.Value = !_settingService.UiIsVisibleSetting.Value;
        }

        public void ResetSession()
        {
            _updateState.State = State.ResetAndInitStats;
        }

        // Update2() because Update() already exists in base class. Update() is not always called but Update2() is!
        public void Update2(GameTime gameTime)
        {
            Visible = VisibilityService.WindowIsVisible(_settingService);

            if (_model.UiHasToBeUpdated)
            {
                _model.UiHasToBeUpdated = false;
                ShowOrHideEntries();
                _rootFlowPanel.HideScrollbarIfExists();
                _hintFlowPanel.ShowHintWhenAllEntriesAreHidden();
            }

            _updateState.AddToElapsedTime(gameTime.ElapsedGameTime.TotalMilliseconds);

            switch (_updateState.State)
            {
                case State.WaitForApiTokenAfterModuleStart: // prevent showing an api key error message right after the module started
                    _updateState.AddToTimeWaitedForApiToken(gameTime.ElapsedGameTime.TotalMilliseconds);

                    if (!_updateState.IsTimeForNextApiTokenCheck())
                        return;

                    _updateState.ResetElapsedTime();
                    
                    if (ApiService.ModuleHasApiToken(_gw2ApiManager))
                    {
                        _updateState.State = State.ResetAndInitStats;
                        return;
                    }

                    if (_updateState.WaitedLongEnoughForApiTokenEitherApiKeyIsMissingOrUserHasNotLoggedIntoACharacter())
                    {
                        // to provoke api key error message for user even in character select.
                        _updateState.State = State.ResetAndInitStats; 
                        return;
                    }
                    
                    SetValueTextAndTooltip("Loading...", "Waiting for user to log into a character or API token from blish.", _valueLabelByEntryId.Values);
                    return;
                case State.WaitBeforeResetAndInitStats:
                    if (!_updateState.IsTimeForNextTryToInitStats())
                        return;
                    
                    _updateState.ResetElapsedTime(); 
                    _updateState.State = State.ResetAndInitStats;
                    return;
                case State.ResetAndInitStats:
                    _updateState.ResetElapsedTime();
                    _updateState.State = State.WaitForApiResponse;
                    Task.Run(ResetAndInitStatValues);
                    return;
                case State.UpdateStats:
                    if (!_updateState.IsTimeForNextStatsUpdate())
                        return;

                    _updateState.ResetElapsedTime();
                    _updateState.State = State.WaitForApiResponse;
                    Task.Run(UpdateStatValues);
                    return;
                case State.WaitForApiResponse:
                    // this case is used to wait for the Task.Run(..) to finish. They will update the state to leave this state, too.
                    // Because of that the state must not be set here directly. It would cause state update racing conditions with the Task.Runs
                    _updateState.ResetElapsedTime();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void ResetAndInitStatValues()
        {
            try
            {
                if (ApiService.ApiKeyIsMissingPermissions(_gw2ApiManager))
                {
                    SetValuesToApiKeyErrorTextAndTooltip();
                    _updateState.State = State.WaitBeforeResetAndInitStats;
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _model.StartSession();
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.ResetSummaryTooltip(_model);
                _updateState.State = State.UpdateStats;
            }
            catch (Exception e)
            {
                var tooltip = $"Error: API call failed or bug in module code. :( \n{RETRY_IN_X_SECONDS_MESSAGE}";
                SetValueTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByEntryId.Values);
                _logger.Warn(e, "Error when initializing values: API failed to respond or bug in module code.");
                _updateState.State = State.WaitBeforeResetAndInitStats;
            }
        }

        private async void UpdateStatValues()
        {
            try
            {
                if (ApiService.ApiKeyIsMissingPermissions(_gw2ApiManager))
                {
                    SetValuesToApiKeyErrorTextAndTooltip();
                    _logger.Warn("Error when updating values: api token is missing permissions. " +
                                 "Possible reasons: api key got removed or new api key is missing permissions.");
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.UpdateSummaryTooltip(_model);
            }
            catch (Exception e)
            {
                // intentionally no error handling!
                // when api server does not respond (error code 500, 502) or times out (RequestCanceledException)
                // the app will just return the previous stat values and hope that on the end of the next interval
                // the api server will answer correctly again.
                _logger.Warn(e, "Error when updating values: API failed to respond or bug in module code.");
            }
            finally
            {
                // even in error case an init makes no sense. It is better to wait for the user to fix the api key to continue to update the old values.
                // this can only cause issues if in the future blish supports swapping gw2 accounts without doing an unload+load of a module.
                _updateState.State = State.UpdateStats;
            }
        }

        private void SetValuesToApiKeyErrorTextAndTooltip()
        {
            var tooltip = "Error: Not logged into a character or API key is missing or missing permissions. :(\n" +
                          $"Required permissions: {string.Join(", ", ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE)}\n" +
                          RETRY_IN_X_SECONDS_MESSAGE;

            SetValueTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByEntryId.Values);
        }

        private static void SetValueTextAndTooltip(string text, string tooltip, Dictionary<string, Label>.ValueCollection valueLabelByEntryId)
        {
            foreach (var valueLabel in valueLabelByEntryId)
            {
                valueLabel.Text             = text;
                valueLabel.BasicTooltipText = tooltip;
            }
        }

        private void ShowOrHideEntries()
        {
            _titlesFlowPanel.ClearChildren();
            _valuesFlowPanel.ClearChildren();
            var visibleEntries = _model.Entries.WhereUserSetToBeVisible();

            if (_settingService.HideStatsWithValueZeroSetting.Value)
                visibleEntries = visibleEntries.WhereSessionValueIsNonZero();

            foreach (var entry in visibleEntries)
            {
                _titleFlowPanelByEntryId[entry.Id].Show();
                _valueLabelByEntryId[entry.Id].Parent = _valuesFlowPanel;
            }
        }

        private void CreateUi(SettingsWindowService settingsWindowService)
        {

            _hintFlowPanel = new HintFlowPanel(_model.Entries, settingsWindowService, _textureService, _settingService, this)
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
            };

            _hintFlowPanel.ShowHintWhenAllEntriesAreHidden();

            _rootFlowPanel = new RootFlowPanel(this, _settingService);

            _titlesFlowPanel = new FlowPanel()
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                Parent           = _rootFlowPanel
            };

            _valuesFlowPanel = new FlowPanel()
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                Parent           = _rootFlowPanel
            };

            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];

            foreach (var entry in _model.Entries)
            {
                _valueLabelByEntryId[entry.Id] = new Label()
                {
                    Text           = "-",
                    TextColor      = _settingService.ValueLabelColorSetting.Value.GetColor(),
                    Font           = font,
                    ShowShadow     = true,
                    AutoSizeHeight = true,
                    AutoSizeWidth  = true,
                    Parent         = entry.IsVisible ? _valuesFlowPanel : null
                };

                _titleFlowPanelByEntryId[entry.Id] = new EntryTitleFlowPanel(entry, font, _titlesFlowPanel, _textureService, _settingService);
            }

            _rootFlowPanel.HideScrollbarIfExists();
        }

        private void OnUserChangedLanguageInBlishSettings(object sender, ValueEventArgs<System.Globalization.CultureInfo> e)
        {
            foreach (var titleFlowPanel in _titleFlowPanelByEntryId)
                titleFlowPanel.Value.UpdateLabelText();
        }

        private void OnValueLabelColorSettingChanged(object sender, ValueChangedEventArgs<ColorType> e)
        {
            foreach (var valueLabel in _valueLabelByEntryId.Values)
                valueLabel.TextColor = e.NewValue.GetColor();
        }

        private void OnFontSizeIndexSettingChanged(object sender, ValueChangedEventArgs<int> valueChangedEventArgs)
        {
            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];

            foreach (var label in _valueLabelByEntryId.Values)
                label.Font = font;
        }
        
        private void OnBackgroundOpacitySettingChanged(object sender, ValueChangedEventArgs<int> valueChangedEventArgs)
        {
            BackgroundColor = new Color(Color.Black, _settingService.BackgroundOpacitySetting.Value);
        }

        private void OnHideStatsWithValueZeroSettingChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            _model.UiHasToBeUpdated = true;
        }

        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly TextureService _textureService;
        private readonly Logger _logger;
        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly StatTooltipService _statTooltipService;
        private readonly ValueLabelTextService _valueLabelTextService;
        private readonly Dictionary<string, EntryTitleFlowPanel> _titleFlowPanelByEntryId = new Dictionary<string, EntryTitleFlowPanel>();
        private readonly Dictionary<string, Label> _valueLabelByEntryId = new Dictionary<string, Label>();
        private readonly UpdateState _updateState;
        private FlowPanel _titlesFlowPanel;
        private FlowPanel _valuesFlowPanel;
        private HintFlowPanel _hintFlowPanel;
        private RootFlowPanel _rootFlowPanel;
        private static readonly string RETRY_IN_X_SECONDS_MESSAGE = $"Retry in {UpdateState.RETRY_INIT_STATS_INTERVAL_IN_SECONDS}s…";
    }
}