using System;
using SessionTracker.SettingEntries;

namespace SessionTracker.StatsWindow
{
    public class UpdateLoop : IDisposable
    {
        public UpdateLoop(SettingService settingService)
        {
            _settingService = settingService;

            OnDebugApiIntervalSettingsChanged();
            settingService.DebugApiIntervalValueSetting.SettingChanged   += OnDebugApiIntervalSettingsChanged;
            settingService.DebugApiIntervalEnabledSetting.SettingChanged += OnDebugApiIntervalSettingsChanged;
        }

        public void Dispose()
        {
            _settingService.DebugApiIntervalEnabledSetting.SettingChanged -= OnDebugApiIntervalSettingsChanged;
            _settingService.DebugApiIntervalValueSetting.SettingChanged   -= OnDebugApiIntervalSettingsChanged;
        }

        public UpdateLoopState State 
        {
            get => _state; 
            set
            {
                if (_state == value)
                    return;

                _state = value;
                StateChanged?.Invoke(null, null);
            }
        }

        public event EventHandler StateChanged;

        public void AddToElapsedTime(double elapsedTimeSinceLastUpdateInMilliseconds)
        {
            _elapsedTimeInMilliseconds += elapsedTimeSinceLastUpdateInMilliseconds;
        }

        public void ResetElapsedTime()
        {
            _elapsedTimeInMilliseconds = 0;
        }

        public bool IsTimeForNextStartNewSessionRetry()
        {
            return _elapsedTimeInMilliseconds >= GetRetryIntervalInMilliseconds();
        }

        public bool IsTimeForSessionUpdate()
        {
            return _elapsedTimeInMilliseconds >= _updateSessionIntervalInMilliseconds;
        }

        public void UseShortRetryUpdateSessionInterval()
        {
            _updateSessionIntervalInMilliseconds = GetRetryIntervalInMilliseconds();
        }

        public void UseRegularUpdateSessionInterval()
        {
            OnDebugApiIntervalSettingsChanged();
        }

        private void OnDebugApiIntervalSettingsChanged(object sender = null, EventArgs e = null)
        {
            _updateSessionIntervalInMilliseconds = _settingService.DebugApiIntervalEnabledSetting.Value
                ? _settingService.DebugApiIntervalValueSetting.Value
                : REGULAR_UPDATE_SESSION_INTERVAL_IN_MILLISECONDS;
        }

        private static int GetRetryIntervalInMilliseconds()
        {
            return RETRY_INTERVAL_IN_SECONDS * 1000;
        }

        public const int RETRY_INTERVAL_IN_SECONDS = 5; // seconds instead of milliseconds because value is displayed in UI
        private const double REGULAR_UPDATE_SESSION_INTERVAL_IN_MILLISECONDS = 5 * 60 * 1000; // 5 minutes  
        private double _updateSessionIntervalInMilliseconds = REGULAR_UPDATE_SESSION_INTERVAL_IN_MILLISECONDS;
        private double _elapsedTimeInMilliseconds;
        private UpdateLoopState _state = UpdateLoopState.WaitingForApiTokenAfterModuleStart;
        private readonly SettingService _settingService;
    }
}