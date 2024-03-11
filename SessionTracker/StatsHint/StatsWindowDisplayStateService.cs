using Blish_HUD.Controls;
using SessionTracker.Api;
using SessionTracker.OtherServices;
using SessionTracker.StatsWindow;
using System;
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
            Services services, 
            Container parent)
        {
            _userHasToSelectStatsFlowPanel = userHasToSelectStatsFlowPanel;
            _errorLabel = errorLabel;
            _allStatsHiddenByZeroValuesSettingImage = allStatsHiddenByZeroValuesSettingImage;
            _statsRootFlowPanel = statsRootFlowPanel;
            _services = services;
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

        public void ShowLoadingHint()
        {
            _errorLabel.SetTextAndTooltip("Loading...", "Waiting for first API response.");
            ShowUpdatedDisplayState();
        }

        public void ShowReadTooltipErrorWithRetryInfo(string tooltip)
        {
            _errorLabel.SetTextAndTooltip("Error: read tooltip!", $"{tooltip}\n{UpdateLoop.RETRY_IN_X_SECONDS_MESSAGE}");
            ShowUpdatedDisplayState();
        }

        public void ShowModuleError()
        {
            _errorLabel.SetTextAndTooltip("session-tracker had an unexpected error :-(. Please report in Blish HUD Discord");
            ShowUpdatedDisplayState();
        }

        public void ShowUpdatedDisplayState()
        {
            var statsWindowDisplayState = DetermineStatsWindowDisplayState(_services, _errorLabel.Text);
            Show(statsWindowDisplayState);
        }

        private void Show(StatsWindowDisplayState statsWindowDisplayState)
        {
            _allStatsHiddenByZeroValuesSettingImage.Parent = statsWindowDisplayState == StatsWindowDisplayState.AllStatsHiddenByZeroValuesAreHiddenSetting ? _parent : null;
            _userHasToSelectStatsFlowPanel.Parent = statsWindowDisplayState == StatsWindowDisplayState.UserHasToSelectStatsHint ? _parent : null;
            _statsRootFlowPanel.SetParentAndHandleScrollbar(statsWindowDisplayState == StatsWindowDisplayState.Stats ? _parent : null);
            _errorLabel.Parent = statsWindowDisplayState == StatsWindowDisplayState.Error ? _parent : null;
        }

        private static StatsWindowDisplayState DetermineStatsWindowDisplayState(Services services, string errorText)
        {
            var noStatsAreSelectedByUser = !services.Model.Stats.Any(e => e.IsSelectedByUser);
            if (noStatsAreSelectedByUser)
                return StatsWindowDisplayState.UserHasToSelectStatsHint;

            // error is not priorized on purpose because the error would override UserHasToSelectStatsHint because of retry intervals 
            if (errorText != string.Empty)
                return StatsWindowDisplayState.Error; 

            var allStatsHiddenBecauseOfZeroValuesAreHiddenSetting = services.Model.Stats.Any(e => e.IsSelectedByUser && e.HasNonZeroSessionValue) == false;
            var hasModuleInitializedStatValues = services.UpdateLoop.State != UpdateLoopState.WaitingForApiTokenAfterModuleStart;
            if (services.SettingService.StatsWithZeroValueAreHiddenSetting.Value && allStatsHiddenBecauseOfZeroValuesAreHiddenSetting && hasModuleInitializedStatValues)
                return StatsWindowDisplayState.AllStatsHiddenByZeroValuesAreHiddenSetting;

            return StatsWindowDisplayState.Stats;
        }   

        private readonly Container _parent;
        private readonly ErrorLabel _errorLabel;
        private readonly StatsRootFlowPanel _statsRootFlowPanel;
        private readonly Services _services;
        private readonly Image _allStatsHiddenByZeroValuesSettingImage;
        private readonly UserHasToSelectStatsFlowPanel _userHasToSelectStatsFlowPanel;
    }
}
