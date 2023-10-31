using Blish_HUD.Settings;
using SessionTracker.RelativePositionWindow;

namespace SessionTracker.SettingEntries
{
    public class MigrateBlishSettingsService
    {
        // Changelog:
        // version 3
        // - use FloatPoint setting instead of 2 float settings for window relative location
        //
        // version 2
        // - use enum setting instead of 2 bool settings for value display format
        public static void MigrateSettings(
            SettingCollection settings,
            SettingCollection internalSettings,
            SettingEntry<int> settingsVersionSetting, 
            SettingEntry<ValueDisplayFormat> valueDisplayFormatSetting, 
            SettingEntry<FloatPoint> windowRelativeLocationSetting)
        {
            var hasToMigrateFrom1To2 = settingsVersionSetting.Value == 1;
            if (hasToMigrateFrom1To2)
            {
                valueDisplayFormatSetting.Value = GetValueDisplayFormat(settings, valueDisplayFormatSetting.Value);
                settings.UndefineSetting(SESSION_VALUES_ARE_VISIBLE_SETTING_KEY);
                settings.UndefineSetting(TOTAL_VALUES_ARE_VISIBLE_SETTING_KEY);
                settingsVersionSetting.Value = 2;
                Module.Logger.Info($"Migrated Blish settings version 1 to 2: valueDisplayFormatSetting: {valueDisplayFormatSetting.Value}");
            }

            var hasToMigrateFrom2To3 = settingsVersionSetting.Value == 2;
            if (hasToMigrateFrom2To3)
            {
                windowRelativeLocationSetting.Value = GetWindowRelativeLocation(internalSettings, windowRelativeLocationSetting.Value);
                internalSettings.UndefineSetting(X_WINDOW_RELATIVE_LOCATION_SETTING_KEY);
                internalSettings.UndefineSetting(Y_WINDOW_RELATIVE_LOCATION_SETTING_KEY);
                settingsVersionSetting.Value = 3;
                Module.Logger.Info($"Migrated Blish settings version 2 to 3: windowRelativeLocationSetting: {windowRelativeLocationSetting.Value}");
            }
        }

        private static FloatPoint GetWindowRelativeLocation(SettingCollection internalSettings, FloatPoint defaultWindowRelativeLocation)
        {
            var xWindowRelativeLocationSetting = internalSettings[X_WINDOW_RELATIVE_LOCATION_SETTING_KEY] as SettingEntry<float>;
            var yWindowRelativeLocationSetting = internalSettings[Y_WINDOW_RELATIVE_LOCATION_SETTING_KEY] as SettingEntry<float>;

            var oldSettingsAreMissing = xWindowRelativeLocationSetting == null || yWindowRelativeLocationSetting == null;
            if (oldSettingsAreMissing)
                return defaultWindowRelativeLocation;

            return new FloatPoint(xWindowRelativeLocationSetting.Value, yWindowRelativeLocationSetting.Value);
        }

        private static ValueDisplayFormat GetValueDisplayFormat(SettingCollection settings, ValueDisplayFormat defaultValueDisplayFormat)
        {
            var sessionValuesAreVisibleSetting = settings[SESSION_VALUES_ARE_VISIBLE_SETTING_KEY] as SettingEntry<bool>;
            var totalValuesAreVisibleSetting = settings[TOTAL_VALUES_ARE_VISIBLE_SETTING_KEY] as SettingEntry<bool>;

            var oldSettingsAreMissing = sessionValuesAreVisibleSetting == null || totalValuesAreVisibleSetting == null;
            if (oldSettingsAreMissing)
                return defaultValueDisplayFormat;

            var sessionValuesAreVisible = sessionValuesAreVisibleSetting.Value;
            var totalValuesAreVisible = totalValuesAreVisibleSetting.Value;

            if (sessionValuesAreVisible && totalValuesAreVisible)
                return ValueDisplayFormat.SessionValue_TotalValue;
            else if (!sessionValuesAreVisible && totalValuesAreVisible)
                return ValueDisplayFormat.TotalValue;
            else if (sessionValuesAreVisible && !totalValuesAreVisible)
                return ValueDisplayFormat.SessionValue;
            else // should never happen: !sessionValuesAreVisible && !totalValuesAreVisible 
                return ValueDisplayFormat.SessionValue;
        }

        private const string SESSION_VALUES_ARE_VISIBLE_SETTING_KEY = "show session values";
        private const string TOTAL_VALUES_ARE_VISIBLE_SETTING_KEY = "show total values";
        private const string X_WINDOW_RELATIVE_LOCATION_SETTING_KEY = "window relative location x";
        private const string Y_WINDOW_RELATIVE_LOCATION_SETTING_KEY = "window relative location y";
    }
}
