using Blish_HUD;
using Blish_HUD.Settings;
using Newtonsoft.Json;
using System;

namespace SessionTracker.Other
{
    public class DebugLogService
    {
        public static void LogVersionAndSettings(SemVer.Version version, SettingCollection settings)
        {
            try // must not crash define settings
            {
                Module.Logger.Debug($"Module Version {version}");
                Module.Logger.Debug($"Settings {JsonConvert.SerializeObject(settings)}");
                Module.Logger.Debug($"Interface size: GW2 ({GameService.Gw2Mumble.UI.UISize}), Blish ({GameService.Graphics.UIScalingMethod})");
                Module.Logger.Debug($"DPI Scaling: GW2 ({GameService.Gw2Mumble.UI}), Blish ({GameService.Graphics.UIScalingMethod})");
            }
            catch (Exception e)
            {
                // warn instead of debug because otherwise it wouldnt be noticed until it is needed. that would suck.
                Module.Logger.Warn(e, "Failed to create debug log message for settings");
            }
        }

        public static void LogSettingChange<T>(object sender, ValueChangedEventArgs<T> eventArgs)
        {
            try
            {
                var settingEntry = sender as SettingEntry;
                Module.Logger.Debug($"{settingEntry.EntryKey}: {eventArgs.PreviousValue} -> {eventArgs.NewValue}");
            }
            catch (Exception e)
            {
                Module.Logger.Warn(e, "Failed to create debug log message on setting change");
            }
        }
    }
}
