using Blish_HUD;
using Blish_HUD.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SessionTracker.Other
{
    public class DebugLogService : IDisposable
    {
        public DebugLogService(SemVer.Version moduleVersion, SettingCollection settings)
        {
            _settingEntries = GetSettingEntries(settings).ToList();

            try // must not crash define settings
            {
                Module.Logger.Debug(
                    $"Module Version {moduleVersion} | " +
                    $"Interface size: GW2 ({GameService.Gw2Mumble.UI.UISize}), Blish ({GameService.Graphics.UIScalingMethod}) | " +
                    $"DPI Scaling: GW2 ({GameService.GameIntegration.GfxSettings.DpiScaling}), Blish ({GameService.Graphics.DpiScalingMethod}) | " +
                    $"Settings: {JsonConvert.SerializeObject(settings, JSON_SERIALIZER_SETTINGS)}");

                LogOnSettingsChanged(_settingEntries);
            }
            catch (Exception e)
            {
                // warn instead of debug because otherwise it wouldnt be noticed until it is needed.
                Module.Logger.Warn(e, "Failed to create DebugLogService");
            }
        }

        public void Dispose()
        {
            try
            {
                StopLoggingOnSettingChanged(_settingEntries);
            }
            catch (Exception e)
            {
                Module.Logger.Warn(e, "Failed to dispose DebugLogService");
            }
        }

        private List<SettingEntry> GetSettingEntries(SettingCollection settings)
        {
            var settingEntries = new List<SettingEntry>();

            foreach (var settingEntry in settings)
            {
                if (settingEntry is SettingEntry<SettingCollection> collectionSettingEntry)
                {
                    var subSettingEntries = GetSettingEntries(collectionSettingEntry.Value);
                    settingEntries.AddRange(subSettingEntries);
                }
                else
                    settingEntries.Add(settingEntry);
            }

            return settingEntries;
        }

        private void LogOnSettingsChanged(List<SettingEntry> settings)
        {
            var handlerMethodInfo = typeof(DebugLogService).GetMethod(nameof(OnSettingChanged), BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var settingEntry in settings)
            {
                if (_loggingDelegates.ContainsKey(settingEntry.EntryKey)) 
                    continue;
                
                var eventInfo = settingEntry.GetType().GetEvent(nameof(SettingEntry<object>.SettingChanged));
                var handlerDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, handlerMethodInfo.MakeGenericMethod(settingEntry.SettingType));
                eventInfo.AddEventHandler(settingEntry, handlerDelegate);
                _loggingDelegates.AddOrUpdate(settingEntry.EntryKey, handlerDelegate, (key, old) => handlerDelegate);
            }
        }

        private void StopLoggingOnSettingChanged(List<SettingEntry> settings)
        {
            foreach (var settingEntry in settings)
            {
                if (!_loggingDelegates.TryRemove(settingEntry.EntryKey, out var handlerDelegate))
                    continue;
     
                var eventInfo = settingEntry.GetType().GetEvent(nameof(SettingEntry<object>.SettingChanged));
                eventInfo.RemoveEventHandler(settingEntry, handlerDelegate);
            }
        }

        private void OnSettingChanged<T>(object sender, ValueChangedEventArgs<T> e)
        {
            var settingEntry = (SettingEntry<T>) sender;
            var oldValue = e.PreviousValue is string
                ? e.PreviousValue.ToString() 
                : JsonConvert.SerializeObject(e.PreviousValue, JSON_SERIALIZER_SETTINGS);

            var newValue = e.NewValue is string 
                ? e.NewValue.ToString() 
                : JsonConvert.SerializeObject(e.NewValue, JSON_SERIALIZER_SETTINGS);

            Module.Logger.Debug($"{settingEntry.EntryKey}: {oldValue} -> {newValue}");
        }

        private readonly ConcurrentDictionary<string, Delegate> _loggingDelegates = new ConcurrentDictionary<string, Delegate>();
        private readonly List<SettingEntry> _settingEntries = new List<SettingEntry>();
        private readonly JsonSerializerSettings JSON_SERIALIZER_SETTINGS = new JsonSerializerSettings 
        {
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };
    }
}