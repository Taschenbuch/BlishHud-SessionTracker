using System;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Controls
{
    public class UpdateState : IDisposable
    {
        public UpdateState(SettingService settingService)
        {
            _settingService = settingService;

            UpdateApiInterval(null, null);
            settingService.DebugApiIntervalValueSetting.SettingChanged   += UpdateApiInterval;
            settingService.DebugApiIntervalEnabledSetting.SettingChanged += UpdateApiInterval;
        }

        public void Dispose()
        {
            _settingService.DebugApiIntervalEnabledSetting.SettingChanged -= UpdateApiInterval;
            _settingService.DebugApiIntervalValueSetting.SettingChanged   -= UpdateApiInterval;
        }
        
        public State State { get; set; } = State.WaitForApiTokenAfterModuleStart;

        public void AddToElapsedTime(double elapsedTimeSinceLastUpdateInMilliseconds)
        {
            _elapsedTimeTotalInMilliseconds += elapsedTimeSinceLastUpdateInMilliseconds;
        }

        public void ResetElapsedTime()
        {
            _elapsedTimeTotalInMilliseconds = 0;
        }

        public void AddToTimeWaitedForApiToken(double elapsedTimeSinceLastUpdateInMilliseconds)
        {
            _timeWaitedForApiTokenInMilliseconds += elapsedTimeSinceLastUpdateInMilliseconds;
        }

        public bool WaitedLongEnoughForApiTokenEitherApiKeyIsMissingOrUserHasNotLoggedIntoACharacter()
        {
            return _timeWaitedForApiTokenInMilliseconds >= 20 * 1000;
        }

        public bool IntervalEndedBetweenApiTokenExistsChecks()
        {
            return _elapsedTimeTotalInMilliseconds >= 200;
        }

        public bool IntervalEndedBetweenInitStatsRetries()
        {
            return _elapsedTimeTotalInMilliseconds >= RETRY_INIT_STATS_INTERVAL_IN_SECONDS * 1000;
        }

        public const int RETRY_INIT_STATS_INTERVAL_IN_SECONDS = 5;

        public bool IntervalEndedBetweenStatsUpdates()
        {
            return _elapsedTimeTotalInMilliseconds >= _updateStatsIntervalInMilliseconds;
        }

        private void UpdateApiInterval(object sender, EventArgs e)
        {
            _updateStatsIntervalInMilliseconds = _settingService.DebugApiIntervalEnabledSetting.Value
                ? _settingService.DebugApiIntervalValueSetting.Value
                : REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS;
        }

        private const double REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS = 5 * 60 * 1000;
        private double _updateStatsIntervalInMilliseconds = REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS;
        private double _elapsedTimeTotalInMilliseconds;
        private double _timeWaitedForApiTokenInMilliseconds;
        private readonly SettingService _settingService;
    }
}