using System;
using SessionTracker.SettingEntries;

namespace SessionTracker.Controls
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

        public bool IsTimeForNextStartNewSessionRetry()
        {
            return _elapsedTimeTotalInMilliseconds >= GetRetryIntervalInMilliseconds();
        }

        public bool IsTimeForSessionUpdate()
        {
            return _elapsedTimeTotalInMilliseconds >= _updateSessionIntervalInMilliseconds;
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
        private double _elapsedTimeTotalInMilliseconds;
        private double _timeWaitedForApiTokenInMilliseconds;
        private UpdateLoopState _state = UpdateLoopState.WaitingForApiTokenAfterModuleStart;
        private readonly SettingService _settingService;
    }
}