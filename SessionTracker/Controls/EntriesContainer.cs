using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework;
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

        public void ToggleVisibility()
        {
            _settingService.UiIsVisibleSetting.Value = !_settingService.UiIsVisibleSetting.Value;
        }

        public void ResetSession()
        {
            _updateState.SetToUninitializedAndSetToInstantInitializeInterval();
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

            _updateState.UpdateElapsedTime(gameTime.ElapsedGameTime.TotalMilliseconds);

            // to prevent showing an api key error message right after the module start
            if (_updateState.ShouldCheckForApiTokenAfterModuleStartup())
            {
                _updateState.CheckIfWaitedLongEnoughAndApiKeyIsProbablyMissing();
                _updateState.ResetElapsedTime();

                if (_gw2ApiManager.HasPermissions(ApiService.ACCOUNT_API_TOKEN_PERMISSION))
                    _updateState.IsWaitingForApiTokenAfterModuleStartup = false;

                return;
            }

            if (_updateState.ShouldInitOrUpdate())
            {
                _updateState.ResetElapsedTime(); // reset time before IsWaitingForApiResponse-check prevents possible elapsedTime overflow

                if (_updateState.IsWaitingForApiResponse) 
                    return;

                _updateState.IsWaitingForApiResponse = true;

                if (_updateState.IsInitialized)
                    Task.Run(UpdateValuesAsync);
                else
                {
                    _updateState.SetToRetryInitializeInterval();
                    Task.Run(InitializeValuesAsync);
                }
            }
        }

        private async void InitializeValuesAsync()
        {
            try
            {
                if (_gw2ApiManager.HasPermissions(ApiService.NECESSARY_API_TOKEN_PERMISSIONS) == false)
                {
                    var tooltip = "Error: Not logged into character or API key missing or API permissions missing. :(\n" +
                                  $"Required permissions: {string.Join(", ", ApiService.NECESSARY_API_TOKEN_PERMISSIONS)}\n" +
                                  "Retry in 5s…";

                    SetTextAndTooltip("Error: read tooltip.", tooltip, _valueLabelByEntryId.Values);
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);
                _model.StartSession();
                
                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.ResetSummaryTooltip(_model);
                
                _updateState.IsInitialized = true;
            }
            catch (Exception e)
            {
                SetTextAndTooltip("Error: read tooltip.", "Error: API call failed.\nRetry in 30s…", _valueLabelByEntryId.Values);
                _logger.Error(e, "Error when initializing: API failed to respond or bug in module code.");
            }
            finally
            {
                _updateState.IsWaitingForApiResponse = false;
            }
        }

        private static void SetTextAndTooltip(string text, string tooltip, Dictionary<string, Label>.ValueCollection valueLabelByEntryId)
        {
            foreach (var valueLabel in valueLabelByEntryId)
            {
                valueLabel.Text             = text;
                valueLabel.BasicTooltipText = tooltip;
            }
        }

        private async void UpdateValuesAsync()
        {
            try
            {
                if (_gw2ApiManager.HasPermissions(ApiService.NECESSARY_API_TOKEN_PERMISSIONS) == false)
                {
                    _updateState.SetToUninitializedAndSetToInstantInitializeInterval();
                    _logger.Warn("Error when fetching values: api token is missing permissions. " +
                                 "Possible reasons: api key got removed or new api key is missing permissions.");
                    return;
                }

                await ApiService.UpdateTotalValuesInModel(_model, _gw2ApiManager);

                _valueLabelTextService.UpdateValueLabelTexts();
                _statTooltipService.UpdateSummaryTooltip(_model);

            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when fetching values: API failed to respond or bug in module code.");
                // intentionally no error handling!
                // when api server does not respond (error code 500, 502) or times out (RequestCanceledException)
                // the app will just return the previous kill/death values and hope that on the end of the next interval
                // the api server will answer correctly again.
            }
            finally
            {
                _updateState.IsWaitingForApiResponse = false;
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
                    Text           = "Loading...",
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
        
    }
}