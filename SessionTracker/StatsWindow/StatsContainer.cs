using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SessionTracker.Api;
using SessionTracker.AutomaticReset;
using SessionTracker.Files;
using SessionTracker.Models;
using SessionTracker.Other;
using SessionTracker.RelativePositionWindow;
using SessionTracker.Reset;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.SettingsWindow;
using SessionTracker.StatsHint;
using SessionTracker.Text;
using SessionTracker.Tooltip;

namespace SessionTracker.StatsWindow
{
    public class StatsContainer : RelativePositionAndMouseDraggableContainer
    {
        public StatsContainer(Model model,
                              Gw2ApiManager gw2ApiManager,
                              TextureService textureService,
                              FileService fileService,
                              UpdateLoop updateLoop,
                              SettingsWindowService settingsWindowService,
                              SettingService settingService)
            : base(settingService)
        {
            _model          = model;
            _gw2ApiManager  = gw2ApiManager;
            _textureService = textureService;
            _fileService    = fileService;
            _updateLoop     = updateLoop;
            _settingService = settingService;

            _resetService = new ResetService(model, settingService.AutomaticSessionResetSetting, settingService.MinutesUntilResetAfterModuleShutdownSetting);
            _model.SessionDuration.StartMeasuring();
            CreateUi(model, settingsWindowService, textureService, settingService);
            // todo x START
            _statsWindowDisplayStateService = new StatsWindowDisplayStateService(
                _userHasToSelectStatsFlowPanel,
                _errorLabel,
                _allStatsHiddenByZeroValuesSettingImage,
                _statsRootFlowPanel,
                _updateLoop,
                model,
                settingService.StatsWithZeroValueAreHiddenSetting,
                this);

            _statsWindowDisplayStateService.ShowUpdatedDisplayState();
            // todo x END

            _valueLabelTextService = new ValueLabelTextService(_valueLabelByStatId, _model, settingService);
            _statTooltipService    = new StatTooltipService(_titleFlowPanelByStatId, _valueLabelByStatId, model, _settingService);

            settingService.StatsWithZeroValueAreHiddenSetting.SettingChanged  += OnStatsWithZeroValueAreHiddenSettingChanged;
            settingService.FontSizeIndexSetting.SettingChanged                += OnFontSizeIndexSettingChanged;
            settingService.BackgroundOpacitySetting.SettingChanged            += OnBackgroundSettingChanged;
            settingService.BackgroundColorSetting.SettingChanged              += OnBackgroundSettingChanged;
            settingService.ValueLabelColorSetting.SettingChanged              += OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                             += OnUserChangedLanguageInBlishSettings;

            OnFontSizeIndexSettingChanged();
        }

        protected override void DisposeControl()
        {
            _settingService.StatsWithZeroValueAreHiddenSetting.SettingChanged -= OnStatsWithZeroValueAreHiddenSettingChanged;
            _settingService.FontSizeIndexSetting.SettingChanged               -= OnFontSizeIndexSettingChanged;
            _settingService.BackgroundOpacitySetting.SettingChanged           -= OnBackgroundSettingChanged;
            _settingService.BackgroundColorSetting.SettingChanged             -= OnBackgroundSettingChanged;
            _settingService.ValueLabelColorSetting.SettingChanged             -= OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                             -= OnUserChangedLanguageInBlishSettings;

            _visibilityService?.Dispose();
            _updateLoop.Dispose();
            _resetService.Dispose();
            _valueLabelTextService.Dispose();
            _statsWindowDisplayStateService.Dispose();
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
                _statsWindowDisplayStateService.ShowUpdatedDisplayState();
            }

            _updateLoop.AddToElapsedTime(gameTime.ElapsedGameTime.TotalMilliseconds);

            switch (_updateLoop.State)
            {
                case UpdateLoopState.WaitingForApiTokenAfterModuleStart: // prevent showing an api key error message right after the module started
                    _updateLoop.ResetElapsedTime(); // reset because not used here, but for other states it should start at 0

                    if (!_apiTokenAvailableCheckInterval.HasEnded())
                        return;

                    var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _gw2ApiManager);
                    var canUpdateOrStartNewSession = apiTokenService.CanAccessApi || _waitedLongEnoughForApiTokenInterval.HasEnded();
                    if(!canUpdateOrStartNewSession)
                    {
                        _statsWindowDisplayStateService.ShowLoadingOrApiTokenIssue(apiTokenService);
                        return;
                    }

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
                    _updateLoop.State = _resetService.HasToAutomaticallyResetSession(ResetCheckLocation.BeforeSessionUpdate)
                        ? UpdateLoopState.StartingNewSession
                        : UpdateLoopState.UpdatingSession;
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
                    _statsWindowDisplayStateService.ShowApiTokenIssue(apiTokenService);
                    _updateLoop.State = UpdateLoopState.PauseBetweenStartNewSessionRetries;
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _resetService.UpdateNextResetDateTime();
                _model.ResetDurationAndStats();
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.ResetSummaryTooltip(_model);
                ShowOrHideStats();
                _statsWindowDisplayStateService.RemoveErrorAndShowUpdatedDisplayState();
                _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
            }
            catch (LogWarnException e)
            {
                _statsWindowDisplayStateService.ShowReadTooltipErrorWithRetryInfo($"Error: API call failed while starting new session. :-(");
                Module.Logger.Warn(e, "Error when initializing values: API failed to respond.");
                _updateLoop.State = UpdateLoopState.PauseBetweenStartNewSessionRetries;
            }
            catch (Exception e)
            {
                _statsWindowDisplayStateService.ShowReadTooltipErrorWithRetryInfo($"Error: Bug in module code. :-(");
                Module.Logger.Error(e, "Error when initializing values: bug in module code.");
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
                    _statsWindowDisplayStateService.ShowApiTokenIssue(apiTokenService);
                    Module.Logger.Warn($"Error when updating values: {apiTokenService.CreateApiErrorText()}");
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit = false;
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.UpdateSummaryTooltip(_model);
                _updateLoop.UseRegularUpdateSessionInterval();
                ShowOrHideStats();
                _statsWindowDisplayStateService.RemoveErrorAndShowUpdatedDisplayState();
                await _fileService.SaveModelToFileAsync(_model);
            }
            catch (LogWarnException e)
            {
                _updateLoop.UseShortRetryUpdateSessionInterval();
                Module.Logger.Warn(e, "Error when updating values: API failed to respond");

                if (_hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit)
                    _statsWindowDisplayStateService.ShowReadTooltipErrorWithRetryInfo($"Error: API call failed while updating session. :-(");
                // intentionally no error handling on regular updates!
                // when api server does not respond (error code 500, 502) or times out (RequestCanceledException)
                // the app will just return the previous stat values and hope that on the end of the next interval
                // the api server will answer correctly again.
            }
            catch (Exception e)
            {
                Module.Logger.Error(e, "Error when updating values: bug in module code.");  // todo module error = module should stop? error updateState einbauen?
            }
            finally
            {
                // even in error case an init makes no sense. It is better to wait for the user to fix the api key to continue to update the old values.
                // this can only cause issues if in the future blish supports swapping gw2 accounts without doing an unload+load of a module.
                _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
            }
        }

        private void ShowOrHideStats() 
        {
            _statTitlesFlowPanel.ClearChildren();
            _statValuesFlowPanel.ClearChildren();
            var visibleStats = _model.Stats.Where(e => e.IsVisible);

            if (_settingService.StatsWithZeroValueAreHiddenSetting.Value)
                visibleStats = visibleStats.Where(e => e.HasNonZeroSessionValue);

            foreach (var stat in visibleStats)
            {
                _titleFlowPanelByStatId[stat.Id].Parent = _statTitlesFlowPanel;
                _valueLabelByStatId[stat.Id].Parent = _statValuesFlowPanel;
            }
        }

        private void CreateUi(Model model, SettingsWindowService settingsWindowService, TextureService textureService, SettingService settingService)
        {
            _errorLabel = new ErrorLabel();
            _userHasToSelectStatsFlowPanel = new UserHasToSelectStatsFlowPanel(settingsWindowService);            
            _allStatsHiddenByZeroValuesSettingImage = new Image(textureService.AllStatsHiddenByZeroValuesSettingTexture)
            {
                Size = new Point(30),
                BasicTooltipText = "All stats are hidden because their current session values are 0.\n" +
                                   "Stats will be visible again when their session value is not 0 anymore.\n" +
                                   "This hide-zero-value-stats-feature can be turned off in the session tracker module settings."
            };

            _statsRootFlowPanel = new StatsRootFlowPanel(this, settingService);

            _statTitlesFlowPanel = new FlowPanel()
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                Parent           = _statsRootFlowPanel
            };

            _statValuesFlowPanel = new FlowPanel()
            {
                FlowDirection    = ControlFlowDirection.SingleTopToBottom,
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                Parent           = _statsRootFlowPanel
            };

            foreach (var stat in model.Stats)
            {
                _valueLabelByStatId[stat.Id] = new Label()
                {
                    Text           = "-",
                    TextColor      = settingService.ValueLabelColorSetting.Value.GetColor(),
                    ShowShadow     = true,
                    AutoSizeHeight = true,
                    AutoSizeWidth  = true,
                    Parent         = null
                };

                _titleFlowPanelByStatId[stat.Id] = new StatTitleFlowPanel(stat, _statTitlesFlowPanel, textureService, settingService);
            }
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

        private void OnFontSizeIndexSettingChanged(object sender = null, ValueChangedEventArgs<int> valueChangedEventArgs = null)
        {
            var font = FontService.Fonts[_settingService.FontSizeIndexSetting.Value];
            _userHasToSelectStatsFlowPanel.SetFont(font);
            _errorLabel.Font = font;
            _allStatsHiddenByZeroValuesSettingImage.Size = new Point(font.LineHeight);

            foreach (var titleFlowPanel in _titleFlowPanelByStatId)
                titleFlowPanel.Value.SetFont(font);

            foreach (var label in _valueLabelByStatId.Values)
                label.Font = font;
        }
        
        private void OnBackgroundSettingChanged(object sender, EventArgs e)
        {
            BackgroundColor = ColorService.CreateBackgroundColor(_settingService);
        }

        private void OnStatsWithZeroValueAreHiddenSettingChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            _model.UiHasToBeUpdated = true;
        }

        private readonly Gw2ApiManager _gw2ApiManager;
        private readonly TextureService _textureService;
        private readonly FileService _fileService;
        private VisibilityService _visibilityService;
        private readonly Interval _apiTokenAvailableCheckInterval = new Interval(TimeSpan.FromMilliseconds(200));
        private readonly Interval _waitedLongEnoughForApiTokenInterval = new Interval(TimeSpan.FromSeconds(20));
        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly StatTooltipService _statTooltipService;
        private readonly ValueLabelTextService _valueLabelTextService;
        private readonly Dictionary<string, StatTitleFlowPanel> _titleFlowPanelByStatId = new Dictionary<string, StatTitleFlowPanel>();
        private readonly Dictionary<string, Label> _valueLabelByStatId = new Dictionary<string, Label>();
        private readonly UpdateLoop _updateLoop;
        private readonly ResetService _resetService;
        private FlowPanel _statTitlesFlowPanel;
        private FlowPanel _statValuesFlowPanel;
        private UserHasToSelectStatsFlowPanel _userHasToSelectStatsFlowPanel;
        private StatsRootFlowPanel _statsRootFlowPanel;
        private ErrorLabel _errorLabel;
        private Image _allStatsHiddenByZeroValuesSettingImage;
        private readonly StatsWindowDisplayStateService _statsWindowDisplayStateService;
        private bool _hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit;
    }
}