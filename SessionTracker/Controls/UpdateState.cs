using System;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Controls
{
    public class UpdateState : IDisposable
    {
        public UpdateState(SettingService settingService)
        {
            _settingService = settingService;

            UpdateApiInterval();
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

        public bool IsTimeForNextApiTokenCheck()
        {
            return _elapsedTimeTotalInMilliseconds >= 200;
        }

        public bool IsTimeForNextTryToInitStats()
        {
            return _elapsedTimeTotalInMilliseconds >= RETRY_INIT_STATS_INTERVAL_IN_SECONDS * 1000;
        }

        public bool IsTimeForNextStatsUpdate()
        {
            return _elapsedTimeTotalInMilliseconds >= _updateStatsIntervalInMilliseconds;
        }

        private void UpdateApiInterval(object sender = null, EventArgs e = null)
        {
            _updateStatsIntervalInMilliseconds = _settingService.DebugApiIntervalEnabledSetting.Value
                ? _settingService.DebugApiIntervalValueSetting.Value
                : REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS;
        }

        public const int RETRY_INIT_STATS_INTERVAL_IN_SECONDS = 5; // seconds instead of milliseconds because value is displayed in UI
        private const double REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS = 5 * 60 * 1000; // 5 minutes
        private double _updateStatsIntervalInMilliseconds = REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS;
        private double _elapsedTimeTotalInMilliseconds;
        private double _timeWaitedForApiTokenInMilliseconds;
        private readonly SettingService _settingService;
    }
}