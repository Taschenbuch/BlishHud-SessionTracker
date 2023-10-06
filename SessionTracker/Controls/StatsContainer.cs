using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SessionTracker.Api;
using SessionTracker.AutomaticReset;
using SessionTracker.Controls.Hint;
using SessionTracker.Models;
using SessionTracker.Reset;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.Settings;
using SessionTracker.Settings.Window;
using SessionTracker.Value.Text;
using SessionTracker.Value.Tooltip;

namespace SessionTracker.Controls
{
    public class StatsContainer : RelativePositionAndMouseDraggableContainer
    {
        public StatsContainer(Model model,
                              Gw2ApiManager gw2ApiManager,
                              TextureService textureService,
                              FileService fileService,
                              UpdateLoop updateLoop,
                              SettingsWindowService settingsWindowService,
                              SettingService settingService,
                              Logger logger)
            : base(settingService)
        {
            _model          = model;
            _gw2ApiManager  = gw2ApiManager;
            _textureService = textureService;
            _fileService    = fileService;
            _updateLoop    = updateLoop;
            _settingService = settingService;
            _logger         = logger;

            _resetService = new ResetService(model, settingService.AutomaticSessionResetSetting, settingService.MinutesUntilResetAfterModuleShutdownSetting);
            _model.SessionDuration.StartMeasuring();
            CreateUi(settingsWindowService);

            _valueLabelTextService = new ValueLabelTextService(_valueLabelByStatId, _model, settingService, logger);
            _statTooltipService    = new StatTooltipService(_titleFlowPanelByStatId, _valueLabelByStatId, model, _settingService);

            settingService.HideStatsWithValueZeroSetting.SettingChanged  += OnHideStatsWithValueZeroSettingChanged;
            settingService.FontSizeIndexSetting.SettingChanged           += OnFontSizeIndexSettingChanged;
            settingService.BackgroundOpacitySetting.SettingChanged       += OnBackgroundSettingChanged;
            settingService.BackgroundColorSetting.SettingChanged         += OnBackgroundSettingChanged;
            settingService.ValueLabelColorSetting.SettingChanged         += OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                        += OnUserChangedLanguageInBlishSettings;
        }

        protected override void DisposeControl()
        {
            _settingService.HideStatsWithValueZeroSetting.SettingChanged -= OnHideStatsWithValueZeroSettingChanged;
            _settingService.FontSizeIndexSetting.SettingChanged          -= OnFontSizeIndexSettingChanged;
            _settingService.BackgroundOpacitySetting.SettingChanged      -= OnBackgroundSettingChanged;
            _settingService.BackgroundColorSetting.SettingChanged        -= OnBackgroundSettingChanged;
            _settingService.ValueLabelColorSetting.SettingChanged        -= OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                        -= OnUserChangedLanguageInBlishSettings;

            _visibilityService?.Dispose();
            _updateLoop.Dispose();
            _resetService.Dispose();
            _valueLabelTextService.Dispose();
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
            _updateLoop.State = UpdateLoopState.StartingNewSession;
        }

        // Update2() because Update() already exists in base class. Update() is not always called but Update2() is!
        public void Update2(GameTime gameTime)
        {
            _visibilityService ??= new VisibilityService(this, _settingService);  // this cannot be done in StatsContainer ctor because hiding window on startup would not work.

            if (_model.UiHasToBeUpdated)
            {
                _model.UiHasToBeUpdated = false;
                ShowOrHideStats();
                _rootFlowPanel.HideScrollbarIfExists();
                _hintFlowPanel.ShowHintWhenAllStatsAreHidden(_updateLoop.State);
            }

            _updateLoop.AddToElapsedTime(gameTime.ElapsedGameTime.TotalMilliseconds);

            switch (_updateLoop.State)
            {
                case UpdateLoopState.WaitingForApiTokenAfterModuleStart: // prevent showing an api key error message right after the module started
                    _updateLoop.AddToTimeWaitedForApiToken(gameTime.ElapsedGameTime.TotalMilliseconds);

                    if (!_updateLoop.IsTimeForNextApiTokenCheck())
                        return;

                    _updateLoop.ResetElapsedTime();

                    var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _gw2ApiManager);
                    var canUpdateOrStartNewSession = apiTokenService.CanAccessApi || _updateLoop.WaitedLongEnoughForApiTokenEitherApiKeyIsMissingOrUserHasNotLoggedIntoACharacter();
                    if(!canUpdateOrStartNewSession)
                    {
                        var title = apiTokenService.ApiTokenState == ApiTokenState.hasNotLoggedIntoCharacterSinceStartingGw2
                            ? "Error: Log into a character!"
                            : "Loading...";

                        SetValueTextAndTooltip(title, "Waiting for user to log into a character or API token from blish.", _valueLabelByStatId.Values);
                        return;
                    }

                    // waited too long: provokes api key error message for user even in character select when waited long enough
                    _resetService.InitializeNextResetDateTimeIfNecessary();
                    var hasToStartNewSession = _resetService.HasToAutomaticallyResetSession(ResetCheckLocation.ModuleStartup);
                    _hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit = !hasToStartNewSession;

                    _updateLoop.State = hasToStartNewSession
                        ? UpdateLoopState.StartingNewSession
                        : UpdateLoopState.UpdatingSession;

                    return;
                case UpdateLoopState.PauseBetweenStartNewSessionRetries:
                    if (!_updateLoop.IsTimeForNextStartNewSessionRetry())
                        return;
                    
                    _updateLoop.ResetElapsedTime(); 
                    _updateLoop.State = UpdateLoopState.StartingNewSession;
                    return;

                case UpdateLoopState.PauseBeforeUpdatingSession:
                    if (!_updateLoop.IsTimeForSessionUpdate())
                        return;

                    _updateLoop.ResetElapsedTime();

                    // automatic reset doesnt have to be instant, it is okay if it is delayed by up to 5 minutes. better than spamming the check in the update loop
                    if (_resetService.HasToAutomaticallyResetSession(ResetCheckLocation.BeforeSessionUpdate))
                    {
                        _updateLoop.State = UpdateLoopState.StartingNewSession;
                        return;
                    }

                    _updateLoop.State = UpdateLoopState.UpdatingSession;
                    return;
                case UpdateLoopState.StartingNewSession:
                    _updateLoop.ResetElapsedTime();
                    _updateLoop.State = UpdateLoopState.WaitingForApiResponse;
                    Task.Run(StartNewSession);
                    return;
                case UpdateLoopState.UpdatingSession:
                    _updateLoop.ResetElapsedTime();
                    _updateLoop.State = UpdateLoopState.WaitingForApiResponse;
                    Task.Run(UpdateSession);
                    return;
                case UpdateLoopState.WaitingForApiResponse:
                    // this case is used to wait for the Task.Run(..) to finish. They will update the state to leave this state, too.
                    // Because of that the state must not be set here directly. It would cause state update racing conditions with the Task.Runs
                    _updateLoop.ResetElapsedTime();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void StartNewSession()
        {
            try
            {
                var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _gw2ApiManager);
                if (!apiTokenService.CanAccessApi)
                {
                    SetValuesToApiKeyErrorTextAndTooltip(apiTokenService);
                    _updateLoop.State = UpdateLoopState.PauseBetweenStartNewSessionRetries;
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _resetService.UpdateNextResetDateTime();
                _model.StartNewSession();
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.ResetSummaryTooltip(_model);
                _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
            }
            catch (LogWarnException e)
            {
                var tooltip = $"Error: API call failed while initializing. :-( \n{RETRY_IN_X_SECONDS_MESSAGE}";
                SetValueTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByStatId.Values);
                _logger.Warn(e, "Error when initializing values: API failed to respond.");
                _updateLoop.State = UpdateLoopState.PauseBetweenStartNewSessionRetries;
            }
            catch (Exception e)
            {
                var tooltip = $"Error: Bug in module code. :-( \n{RETRY_IN_X_SECONDS_MESSAGE}";
                SetValueTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByStatId.Values);
                _logger.Error(e, "Error when initializing values: bug in module code.");
                _updateLoop.State = UpdateLoopState.PauseBetweenStartNewSessionRetries; // todo module error = module should stop? error updateState einbauen?
            }
        }

        private async void UpdateSession()
        {
            try
            {
                _resetService.UpdateNextResetDateTimeForMinutesAfterShutdownReset();

                var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _gw2ApiManager);
                if (!apiTokenService.CanAccessApi)
                {
                    SetValuesToApiKeyErrorTextAndTooltip(apiTokenService);
                    _logger.Warn("Error when updating values: api token is missing permissions. " +
                                 "Possible reasons: api key got removed or new api key is missing permissions.");
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.UpdateSummaryTooltip(_model);
                _hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit = false;
                _updateLoop.UseRegularUpdateSessionInterval();
                await _fileService.SaveModelToFileAsync(_model);
            }
            catch (LogWarnException e)
            {
                _updateLoop.UseShortRetryUpdateSessionInterval();
                _logger.Warn(e, "Error when updating values: API failed to respond");

                if (_hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit)
                {
                    var tooltip = $"Error: API call failed while updating. :-( \n{RETRY_IN_X_SECONDS_MESSAGE}";
                    SetValueTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByStatId.Values);
                }
                // intentionally no error handling on regular updates!
                // when api server does not respond (error code 500, 502) or times out (RequestCanceledException)
                // the app will just return the previous stat values and hope that on the end of the next interval
                // the api server will answer correctly again.
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when updating values: bug in module code.");  // todo module error = module should stop? error updateState einbauen?
            }
            finally
            {
                // even in error case an init makes no sense. It is better to wait for the user to fix the api key to continue to update the old values.
                // this can only cause issues if in the future blish supports swapping gw2 accounts without doing an unload+load of a module.
                _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
            }
        }

        private void SetValuesToApiKeyErrorTextAndTooltip(ApiTokenService apiTokenService)
        {
            var apiErrorText = apiTokenService.CreateApiErrorText();
            var tooltip = $"{apiErrorText}\n{RETRY_IN_X_SECONDS_MESSAGE}";
            SetValueTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByStatId.Values);
        }

        private static void SetValueTextAndTooltip(string text, string tooltip, Dictionary<string, Label>.ValueCollection valueLabelByStatId)
        {
            foreach (var valueLabel in valueLabelByStatId)
            {
                valueLabel.Text             = text;
                valueLabel.BasicTooltipText = tooltip;
            }
        }

        private void ShowOrHideStats()
        {
            _titlesFlowPanel.ClearChildren();
            _valuesFlowPanel.ClearChildren();
            var visibleStats = _model.Stats.WhereUserSetToBeVisible();

            if (_settingService.HideStatsWithValueZeroSetting.Value)
                visibleStats = visibleStats.WhereSessionValueIsNonZero();

            foreach (var stat in visibleStats)
            {
                _titleFlowPanelByStatId[stat.Id].Show();
                _valueLabelByStatId[stat.Id].Parent = _valuesFlowPanel;
            }
        }

        private void CreateUi(SettingsWindowService settingsWindowService)
        {

            _hintFlowPanel = new HintFlowPanel(_model.Stats, settingsWindowService, _textureService, _settingService, this)
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
            };

            _hintFlowPanel.ShowHintWhenAllStatsAreHidden(_updateLoop.State);

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

            foreach (var stat in _model.Stats)
            {
                _valueLabelByStatId[stat.Id] = new Label()
                {
                    Text           = "-",
                    TextColor      = _settingService.ValueLabelColorSetting.Value.GetColor(),
                    Font           = font,
                    ShowShadow     = true,
                    AutoSizeHeight = true,
                    AutoSizeWidth  = true,
                    Parent         = stat.IsVisible ? _valuesFlowPanel : null
                };

                _titleFlowPanelByStatId[stat.Id] = new StatTitleFlowPanel(stat, font, _titlesFlowPanel, _textureService, _settingService);
            }

            _rootFlowPanel.HideScrollbarIfExists();
        }

        private void OnUserChangedLanguageInBlishSettings(object sender, ValueEventArgs<System.Globalization.CultureInfo> e)
        {
            foreach (var titleFlowPanel in _titleFlowPanelByStatId)
                titleFlowPanel.Value.UpdateLabelText();
        }

        private void OnValueLabelColorSettingChanged(object sender, ValueChangedEventArgs<ColorType> e)
        {
            foreach (var valueLabel in _valueLabelByStatId.Values)
                valueLabel.TextColor = e.NewValue.GetColor();
        }

        private void OnFontSizeIndexSettingChanged(object sender, ValueChangedEventArgs<int> valueChangedEventArgs)
        {
            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];

            foreach (var label in _valueLabelByStatId.Values)
                label.Font = font;
        }
        
        private void OnBackgroundSettingChanged(object sender, EventArgs e)
        {
            BackgroundColor = ColorService.CreateBackgroundColor(_settingService);
        }

        private void OnHideStatsWithValueZeroSettingChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            _model.UiHasToBeUpdated = true;
        }

        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly TextureService _textureService;
        private readonly FileService _fileService;
        private VisibilityService _visibilityService;
        private readonly Logger _logger;
        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly StatTooltipService _statTooltipService;
        private readonly ValueLabelTextService _valueLabelTextService;
        private readonly Dictionary<string, StatTitleFlowPanel> _titleFlowPanelByStatId = new Dictionary<string, StatTitleFlowPanel>();
        private readonly Dictionary<string, Label> _valueLabelByStatId = new Dictionary<string, Label>();
        private readonly UpdateLoop _updateLoop;
        private readonly ResetService _resetService;
        private FlowPanel _titlesFlowPanel;
        private FlowPanel _valuesFlowPanel;
        private HintFlowPanel _hintFlowPanel;
        private RootFlowPanel _rootFlowPanel;
        private static readonly string RETRY_IN_X_SECONDS_MESSAGE = $"Retry in {UpdateLoop.RETRY_INTERVAL_IN_SECONDS}s…";
        private bool _hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit;
    }
}