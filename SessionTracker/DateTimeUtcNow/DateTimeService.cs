﻿using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using System;
using System.Diagnostics;

namespace SessionTracker.DateTimeUtcNow
{
    public class DateTimeService : IDisposable
    {
        // static part
        public static DateTime UtcNow
        {
            get
            {
                return _debugEnabled
                    ? _utcNowDebug + _stopWatch.Elapsed
                    : DateTime.UtcNow;
            }
            set
            {
                _utcNowDebug = value;
                _stopWatch.Restart();
            }
        }

        private static DateTime _utcNowDebug = DateTime.UtcNow;
        private static bool _debugEnabled;
        private static readonly Stopwatch _stopWatch = new Stopwatch(); // no disposing necessary

        // non static part (but updating static props
        public DateTimeService()
        {
            _stopWatch.Start();
            _utcNowDebug = DateTime.UtcNow; // because it is static. It would userwise survive module unload/load with old value
        }

        // must be called AFTER DefineSettings()
        public void CreateDateTimeDebugPanel(Container parent)
        {
            DateTimeDebugPanelService.CreateDateTimeDebugPanel(parent, _debugDateTimeEnabledSetting, _debugDateTimeValueSetting);
        }

        public void DefineSettings(SettingCollection settings)
        {
            _debugDateTimeEnabledSetting = settings.DefineSetting(
                "debug date time enabled",
                false,
                () => "use debug dateTime",
                () => "Use debug dateTime instead of system time.");

            _debugDateTimeValueSetting = settings.DefineSetting(
                "debug date time value",
                DateTime.UtcNow.ToString(),
                () => "use debug dateTime",
                () => "Use debug dateTime instead of system time.");

            _debugEnabled = _debugDateTimeEnabledSetting.Value;
            _debugDateTimeEnabledSetting.SettingChanged += DebugApiIntervalEnabledSettingChanged;
            DebugApiIntervalEnabledSettingChanged();
        }

        public void Dispose()
        {
            _debugDateTimeEnabledSetting.SettingChanged -= DebugApiIntervalEnabledSettingChanged;
        }

        private void DebugApiIntervalEnabledSettingChanged(object sender = null, ValueChangedEventArgs<bool> e = null)
        {
            _debugEnabled = _debugDateTimeEnabledSetting.Value;
        }

        private SettingEntry<bool> _debugDateTimeEnabledSetting;
        private SettingEntry<string> _debugDateTimeValueSetting;
    }
}
