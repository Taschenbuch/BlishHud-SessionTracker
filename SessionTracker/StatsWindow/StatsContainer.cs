using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SessionTracker.Api;
using SessionTracker.AutomaticReset;
using SessionTracker.Models;
using SessionTracker.Other;
using SessionTracker.RelativePositionWindow;
using SessionTracker.Reset;
using SessionTracker.OtherServices;
using SessionTracker.SettingEntries;
using SessionTracker.StatsHint;
using SessionTracker.StatTooltip;
using SessionTracker.Text;

namespace SessionTracker.StatsWindow
{
    public class StatsContainer : RelativePositionAndMouseDraggableContainer
    {

        public StatsContainer(Services services)
            : base(services.SettingService)
        {
            _services = services;
            _model = services.Model;
            _updateLoop = services.UpdateLoop;

            HeightSizingMode = SizingMode.AutoSize;
            WidthSizingMode = SizingMode.AutoSize;
            BackgroundColor = ColorService.CreateBackgroundColor(services.SettingService);
            Visible = services.SettingService.UiIsVisibleSetting.Value;
            Parent = GameService.Graphics.SpriteScreen;

            _resetService = new ResetService(services);
            services.Model.SessionDuration.StartMeasuring();
            CreateUi(services);
            _valueLabelTextService = new ValueLabelTextService(_valueLabelByStatId, services);
            _summaryTooltipService = new SummaryTooltipService(_titleFlowPanelByStatId, _valueLabelByStatId, services);

            _statsWindowDisplayStateService = new StatsWindowDisplayStateService(
                _userHasToSelectStatsFlowPanel,
                _errorLabel,
                _allStatsHiddenByZeroValuesSettingImage,
                _statsRootFlowPanel,
                services,
                this);

            _statsWindowDisplayStateService.ShowUpdatedDisplayState();

            services.SettingService.StatsWithZeroValueAreHiddenSetting.SettingChanged  += OnStatsWithZeroValueAreHiddenSettingChanged;
            services.SettingService.FontSizeIndexSetting.SettingChanged                += OnFontSizeIndexSettingChanged;
            services.SettingService.BackgroundOpacitySetting.SettingChanged            += OnBackgroundSettingChanged;
            services.SettingService.BackgroundColorSetting.SettingChanged              += OnBackgroundSettingChanged;
            services.SettingService.ValueLabelColorSetting.SettingChanged              += OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                                      += OnUserChangedLanguageInBlishSettings;

            OnFontSizeIndexSettingChanged();
        } 

        protected override void DisposeControl()
        {
            _services.SettingService.StatsWithZeroValueAreHiddenSetting.SettingChanged -= OnStatsWithZeroValueAreHiddenSettingChanged;
            _services.SettingService.FontSizeIndexSetting.SettingChanged               -= OnFontSizeIndexSettingChanged;
            _services.SettingService.BackgroundOpacitySetting.SettingChanged           -= OnBackgroundSettingChanged;
            _services.SettingService.BackgroundColorSetting.SettingChanged             -= OnBackgroundSettingChanged;
            _services.SettingService.ValueLabelColorSetting.SettingChanged             -= OnValueLabelColorSettingChanged;
            GameService.Overlay.UserLocaleChanged                                     -= OnUserChangedLanguageInBlishSettings;

            _visibilityService?.Dispose();
            _updateLoop.Dispose();
            _resetService.Dispose();
            _valueLabelTextService.Dispose();
            _statsWindowDisplayStateService.Dispose();
            base.DisposeControl();
        }

        public override Control TriggerMouseInput(MouseEventType mouseEventType, MouseState ms)
        {
            var windowCanBeClickedThrough = !_services.SettingService.DragWindowWithMouseIsEnabledSetting.Value
                                          && _services.SettingService.WindowCanBeClickedThroughSetting.Value
                                          && GameService.Input.Keyboard.ActiveModifiers != ModifierKeys.Alt;

            return windowCanBeClickedThrough
                ? null
                : base.TriggerMouseInput(mouseEventType, ms);
        }

        public void ToggleVisibility()
        {
            _services.SettingService.UiIsVisibleSetting.Value = !_services.SettingService.UiIsVisibleSetting.Value;
        }

        public void ResetSession()
        {
            _updateLoop.State = UpdateLoopState.StartingNewSession;
        }

        // Update2() because Update() already exists in base class. Update() is not always called but Update2() is!
        public void Update2(GameTime gameTime)
        {
            _visibilityService ??= new VisibilityService(this, _services.SettingService);  // this cannot be done in StatsContainer ctor because hiding window on startup would not work.

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

                    var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _services.Gw2ApiManager);
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
                case UpdateLoopState.Error:
                    // NOOP
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void StartNewSession()
        {
            try
            {
                var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _services.Gw2ApiManager);
                if (!apiTokenService.CanAccessApi)
                {
                    _statsWindowDisplayStateService.ShowApiTokenIssue(apiTokenService);
                    _updateLoop.State = UpdateLoopState.PauseBetweenStartNewSessionRetries;
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_services);
                _resetService.UpdateNextResetDateTime();
                _model.ResetDurationAndStats();
                _valueLabelTextService.UpdateValueLabelTexts();
                _summaryTooltipService.ResetSummaryTooltip();
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
                Module.Logger.Error(e, "Error when initializing values: bug in module code.");
                _statsWindowDisplayStateService.ShowModuleError();
                _updateLoop.State = UpdateLoopState.Error;
            }
        }

        private async void UpdateSession()
        {
            try
            {
                _resetService.UpdateNextResetDateTimeForMinutesAfterShutdownReset();

                var apiTokenService = new ApiTokenService(ApiService.API_TOKEN_PERMISSIONS_REQUIRED_BY_MODULE, _services.Gw2ApiManager);
                if (!apiTokenService.CanAccessApi)
                {
                    _statsWindowDisplayStateService.ShowApiTokenIssue(apiTokenService);
                    Module.Logger.Warn($"Error when updating values: {apiTokenService.CreateApiErrorText()}");
                    _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_services);
                _hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit = false;
                _valueLabelTextService.UpdateValueLabelTexts();
                _summaryTooltipService.UpdateSummaryTooltip();
                _updateLoop.UseRegularUpdateSessionInterval();
                ShowOrHideStats();
                _statsWindowDisplayStateService.RemoveErrorAndShowUpdatedDisplayState();
                await _services.FileService.SaveModelToFileAsync(_model);
                _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
            }
            catch (LogWarnException e)
            {
                _updateLoop.UseShortRetryUpdateSessionInterval();
                Module.Logger.Warn(e, "Error when updating values: API failed to respond");

                if (_hasToShowApiErrorInfoBecauseIsFirstUpdateWithoutInit)
                    _statsWindowDisplayStateService.ShowReadTooltipErrorWithRetryInfo($"Error: API call failed while updating session. :-(");

                _updateLoop.State = UpdateLoopState.PauseBeforeUpdatingSession;
                // intentionally no error handling on regular updates!
                // when api server does not respond (error code 500, 502) or times out (RequestCanceledException)
                // the app will just return the previous stat values and hope that on the end of the next interval
                // the api server will answer correctly again.
            }
            catch (Exception e)
            {
                Module.Logger.Error(e, "Error when updating values: bug in module code.");
                _statsWindowDisplayStateService.ShowModuleError();
                _updateLoop.State = UpdateLoopState.Error;
            }
        }

        private void ShowOrHideStats() 
        {
            _statTitlesFlowPanel.ClearChildren();
            _statValuesFlowPanel.ClearChildren();
            var visibleStats = _model.Stats.Where(e => e.IsVisible);

            if (_services.SettingService.StatsWithZeroValueAreHiddenSetting.Value)
                visibleStats = visibleStats.Where(e => e.HasNonZeroSessionValue);

            foreach (var stat in visibleStats)
            {
                _titleFlowPanelByStatId[stat.Id].Parent = _statTitlesFlowPanel;
                _valueLabelByStatId[stat.Id].Parent = _statValuesFlowPanel;
            }
        }

        private void CreateUi(Services services)
        {
            _errorLabel = new ErrorLabel();
            _userHasToSelectStatsFlowPanel = new UserHasToSelectStatsFlowPanel(services.SettingsWindowService);            
            _allStatsHiddenByZeroValuesSettingImage = new Image(_services.TextureService.AllStatsHiddenByZeroValuesSettingTexture)
            {
                Size = new Point(30),
                BasicTooltipText = "All stats are hidden because their current session values are 0.\n" +
                                   "Stats will be visible again when their session value is not 0 anymore.\n" +
                                   "This hide-zero-value-stats-feature can be turned off in the session tracker module settings."
            };

            _statsRootFlowPanel = new StatsRootFlowPanel(_services.SettingService);

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

            new RightWindowMarginForScrollbar(_statsRootFlowPanel, _services.SettingService); // is automatically disposed with _statsRootFlowPanel

            foreach (var stat in _services.Model.Stats)
            {
                _valueLabelByStatId[stat.Id] = new Label()
                {
                    Text           = "-",
                    TextColor      = _services.SettingService.ValueLabelColorSetting.Value.GetColor(),
                    ShowShadow     = true,
                    AutoSizeHeight = true,
                    AutoSizeWidth  = true,
                    Tooltip        = new SummaryTooltip(),
                    Parent         = null
                };

                _titleFlowPanelByStatId[stat.Id] = new StatTitleFlowPanel(stat, _statTitlesFlowPanel, _services);
            }
        }

        private void OnUserChangedLanguageInBlishSettings(object sender, ValueEventArgs<System.Globalization.CultureInfo> e)
        {
            _summaryTooltipService.UpdateSummaryTooltip();
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
            var font = FontService.Fonts[_services.SettingService.FontSizeIndexSetting.Value];
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
            BackgroundColor = ColorService.CreateBackgroundColor(_services.SettingService);
        }

        private void OnStatsWithZeroValueAreHiddenSettingChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            _model.UiHasToBeUpdated = true;
        }

        private VisibilityService _visibilityService;
        private readonly Interval _apiTokenAvailableCheckInterval = new Interval(TimeSpan.FromMilliseconds(200));
        private readonly Interval _waitedLongEnoughForApiTokenInterval = new Interval(TimeSpan.FromSeconds(20));
        private readonly Model _model;
        private readonly SummaryTooltipService _summaryTooltipService;
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
        private readonly Services _services;
    }
}