using Blish_HUD.Controls;
using Blish_HUD.Settings;
using SessionTracker.Api;
using SessionTracker.Models;
using SessionTracker.StatsWindow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.StatsHint
{
    public class StatsWindowDisplayStateService : IDisposable
    {
        public StatsWindowDisplayStateService(
            UserHasToSelectStatsFlowPanel userHasToSelectStatsFlowPanel,
            ErrorLabel errorLabel,
            Image allStatsHiddenByZeroValuesSettingImage,
            StatsRootFlowPanel statsRootFlowPanel, 
            UpdateLoop updateLoop, 
            Model model, 
            SettingEntry<bool> statsWithZeroValueAreHiddenSetting, 
            Container parent)
        {
            _userHasToSelectStatsFlowPanel = userHasToSelectStatsFlowPanel;
            _errorLabel = errorLabel;
            _allStatsHiddenByZeroValuesSettingImage = allStatsHiddenByZeroValuesSettingImage;
            _statsRootFlowPanel = statsRootFlowPanel;
            _updateLoop = updateLoop;
            _model = model;
            _statsWithZeroValueAreHiddenSetting = statsWithZeroValueAreHiddenSetting;
            _parent = parent;
        }

        public void Dispose()
        {
            // dispose them because they may not be child of statsContainer so they do not get disposed automatically when statsContainer is dispoed in module.Unload()
            _statsRootFlowPanel.Dispose();
            _allStatsHiddenByZeroValuesSettingImage.Dispose();
            _userHasToSelectStatsFlowPanel.Dispose();
            _errorLabel.Dispose();
        }

        public void RemoveErrorAndShowUpdatedDisplayState()
        {
            _errorLabel.Text = string.Empty;
            ShowUpdatedDisplayState();
        }
     
        public void ShowLoadingOrApiTokenIssue(ApiTokenService apiTokenService)
        {
            _errorLabel.SetToApiTokenIssue(apiTokenService, true);
            ShowUpdatedDisplayState();
        }

        public void ShowApiTokenIssue(ApiTokenService apiTokenService)
        {
            _errorLabel.SetToApiTokenIssue(apiTokenService, false);
            ShowUpdatedDisplayState();
        }

        // todo x retry ding überdenken siehe method oben
        public void ShowReadTooltipErrorWithRetryInfo(string tooltip)
        {
            _errorLabel.SetTextAndTooltip("Error: read tooltip!", $"{tooltip}\n{UpdateLoop.RETRY_IN_X_SECONDS_MESSAGE}");
            ShowUpdatedDisplayState();
        }

        public void ShowUpdatedDisplayState()
        {
            var statsWindowDisplayState = DetermineStatsWindowDisplayState(_model.Stats, _updateLoop.State, _statsWithZeroValueAreHiddenSetting.Value, _errorLabel.Text);
            Show(statsWindowDisplayState);
        }

        private void Show(StatsWindowDisplayState statsWindowDisplayState)
        {
            _allStatsHiddenByZeroValuesSettingImage.Parent 
                = statsWindowDisplayState == StatsWindowDisplayState.AllStatsHiddenByZeroValuesAreHiddenSetting ? _parent : null;
            
            _userHasToSelectStatsFlowPanel.Parent 
                = statsWindowDisplayState == StatsWindowDisplayState.UserHasToSelectStatsHint ? _parent : null;

            _statsRootFlowPanel.SetParentAndHandleScrollbar(statsWindowDisplayState == StatsWindowDisplayState.Stats ? _parent : null);
            
            _errorLabel.Parent 
                = statsWindowDisplayState == StatsWindowDisplayState.Error ? _parent : null;
        }

        private static StatsWindowDisplayState DetermineStatsWindowDisplayState(List<Stat> stats, UpdateLoopState updateLoopState, bool statsWithZeroValueAreHidden, string errorText)
        {
            var noStatsAreSelectedByUser = !stats.Any(e => e.IsVisible);
            if (noStatsAreSelectedByUser)
                return StatsWindowDisplayState.UserHasToSelectStatsHint;

            // error is not priorized on purpose because the error would override UserHasToSelectStatsHint because of retry intervals 
            if (errorText != string.Empty)
                return StatsWindowDisplayState.Error; 

            var allStatsHiddenBecauseOfZeroValuesAreHiddenSetting = stats.Any(e => e.IsVisible && e.HasNonZeroSessionValue) == false;
            var hasModuleInitializedStatValues = updateLoopState != UpdateLoopState.WaitingForApiTokenAfterModuleStart;
            if (statsWithZeroValueAreHidden && allStatsHiddenBecauseOfZeroValuesAreHiddenSetting && hasModuleInitializedStatValues)
                return StatsWindowDisplayState.AllStatsHiddenByZeroValuesAreHiddenSetting;

            return StatsWindowDisplayState.Stats;
        }   

        private readonly Container _parent;
        private readonly ErrorLabel _errorLabel;
        private readonly StatsRootFlowPanel _statsRootFlowPanel;
        private readonly Image _allStatsHiddenByZeroValuesSettingImage;
        private readonly UserHasToSelectStatsFlowPanel _userHasToSelectStatsFlowPanel;
        private readonly UpdateLoop _updateLoop;
        private readonly Model _model;
        private readonly SettingEntry<bool> _statsWithZeroValueAreHiddenSetting;
    }
}
