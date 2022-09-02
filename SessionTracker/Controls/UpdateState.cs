using System;
using Blish_HUD;
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

        public void UpdateElapsedTime(double totalMilliseconds) => _elapsedTimeInMilliseconds += totalMilliseconds;
        public void ResetElapsedTime() => _elapsedTimeInMilliseconds = 0;

        public void SetToUninitializedAndSetToInstantInitializeInterval()
        {
            IsInitialized                          = false;
            _initializeStatsIntervalInMilliseconds = INSTANT_INITIALIZE_INTERVAL_IN_MILLISECONDS;
        }

        public void SetToRetryInitializeInterval()
        {
            _initializeStatsIntervalInMilliseconds = RETRY_INITIALIZE_INTERVAL_IN_MILLISECONDS;
        }

        public bool ShouldCheckForApiTokenAfterModuleStartup()
        {
            if (IsWaitingForApiTokenAfterModuleStartup == false)
                return false;

            var intervalForNextCheckEnded = _elapsedTimeInMilliseconds >= CHECK_FOR_API_TOKEN_AFTER_MODULE_STARTUP_INTERVAL_IN_MILLISECONDS;
            return intervalForNextCheckEnded;
        }

        public void CheckIfWaitedLongEnoughAndApiKeyIsProbablyMissing()
        {
            _timeWaitedForApiTokenInMilliseconds += _elapsedTimeInMilliseconds;

            if (_timeWaitedForApiTokenInMilliseconds >= MAX_WAIT_TIME_FOR_API_TOKEN_AFTER_MODULE_STARTUP_IN_MILLISECONDS)
                IsWaitingForApiTokenAfterModuleStartup = false;
        }

        public bool ShouldInitOrUpdate()
        {
            var shouldInit   = !IsInitialized && _elapsedTimeInMilliseconds >= _initializeStatsIntervalInMilliseconds;
            var shouldUpdate = IsInitialized && _elapsedTimeInMilliseconds >= _updateStatsIntervalInMilliseconds;

            return shouldInit || shouldUpdate;
        }

        public bool IsInitialized { get; set; }
        public bool IsWaitingForApiResponse { get; set; }
        public bool IsWaitingForApiTokenAfterModuleStartup { get; set; } = true;

        private void UpdateApiInterval(object sender, EventArgs e)
        {
            _updateStatsIntervalInMilliseconds = _settingService.DebugApiIntervalEnabledSetting.Value
                ? _settingService.DebugApiIntervalValueSetting.Value
                : REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS;
        }

        private readonly SettingService _settingService;
        private double _elapsedTimeInMilliseconds;
        private double _timeWaitedForApiTokenInMilliseconds = 0;
        private double _initializeStatsIntervalInMilliseconds = INSTANT_INITIALIZE_INTERVAL_IN_MILLISECONDS;
        private double _updateStatsIntervalInMilliseconds = REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS;
        private const double INSTANT_INITIALIZE_INTERVAL_IN_MILLISECONDS = 0;
        private const double REGULAR_UPDATE_INTERVAL_IN_MILLISECONDS = 5 * 60 * 1000;
        private const double RETRY_INITIALIZE_INTERVAL_IN_MILLISECONDS = 5 * 1000;
        private const double MAX_WAIT_TIME_FOR_API_TOKEN_AFTER_MODULE_STARTUP_IN_MILLISECONDS = 15 * 1000;
        private const double CHECK_FOR_API_TOKEN_AFTER_MODULE_STARTUP_INTERVAL_IN_MILLISECONDS = 200;
    }
}