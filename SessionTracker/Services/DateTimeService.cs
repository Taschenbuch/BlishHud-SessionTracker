using Blish_HUD;
using Blish_HUD.Settings;
using System;
using System.Diagnostics;

namespace SessionTracker.Services
{
    public class DateTimeService : IDisposable
    {
        public DateTimeService(SettingEntry<bool> debugDateTimeEnabledSetting)
        {
            _debugDateTimeEnabledSetting = debugDateTimeEnabledSetting;
            _utcNowDebug = DateTime.UtcNow;
            _stopWatch.Start();
            debugDateTimeEnabledSetting.SettingChanged += DebugApiIntervalEnabledSettingChanged;
            DebugApiIntervalEnabledSettingChanged();
            debugEnabled = debugDateTimeEnabledSetting.Value;
        }

        public void Dispose()
        {
            _debugDateTimeEnabledSetting.SettingChanged -= DebugApiIntervalEnabledSettingChanged;
        }

        public static bool debugEnabled { get; set; }

        public static DateTime UtcNow
        {
            get
            {
                return debugEnabled
                    ? _utcNowDebug + _stopWatch.Elapsed
                    : DateTime.UtcNow;
            }
            set
            {
                _utcNowDebug = value;
                _stopWatch.Restart();
            }
        }

        private void DebugApiIntervalEnabledSettingChanged(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            debugEnabled = _debugDateTimeEnabledSetting.Value;
        }

        private static DateTime _utcNowDebug;
        private static Stopwatch _stopWatch = new Stopwatch(); // does not require IDisposeable
        private SettingEntry<bool> _debugDateTimeEnabledSetting;
    }
}
