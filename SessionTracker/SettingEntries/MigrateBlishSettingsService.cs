using Blish_HUD.Settings;

namespace SessionTracker.SettingEntries
{
    public class MigrateBlishSettingsService
    {
        // Changelog:
        // version 2
        // - use enum setting instead of 2 bool settings for value display format
        public static void MigrateSettings(SettingCollection settings, SettingEntry<int> settingsVersionSetting, SettingEntry<ValueDisplayFormat> valueDisplayFormatSetting)
        {
            var hasToMigrateOldSettings = settingsVersionSetting.Value == 1;
            if (hasToMigrateOldSettings)
            {
                valueDisplayFormatSetting.Value = GetValueDisplayFormat(settings);
                settings.UndefineSetting(SESSION_VALUES_ARE_VISIBLE_SETTING_KEY);
                settings.UndefineSetting(TOTAL_VALUES_ARE_VISIBLE_SETTING_KEY);
                settingsVersionSetting.Value = 2;
            }
        }

        private static ValueDisplayFormat GetValueDisplayFormat(SettingCollection settings)
        {
            var sessionValuesAreVisibleSetting = settings[SESSION_VALUES_ARE_VISIBLE_SETTING_KEY] as SettingEntry<bool>;
            var totalValuesAreVisibleSettings = settings[TOTAL_VALUES_ARE_VISIBLE_SETTING_KEY] as SettingEntry<bool>;

            if (sessionValuesAreVisibleSetting == null || totalValuesAreVisibleSettings == null) // should not happen
                return ValueDisplayFormat.SessionValue;

            var sessionValuesAreVisible = sessionValuesAreVisibleSetting.Value;
            var totalValuesAreVisible = totalValuesAreVisibleSettings.Value;

            if (sessionValuesAreVisible && totalValuesAreVisible)
                return ValueDisplayFormat.SessionAndTotalValue;
            else if (!sessionValuesAreVisible && totalValuesAreVisible)
                return ValueDisplayFormat.TotalValue;
            else if (sessionValuesAreVisible && !totalValuesAreVisible)
                return ValueDisplayFormat.SessionValue;
            else // should never happen: !sessionValuesAreVisible && !totalValuesAreVisible 
                return ValueDisplayFormat.SessionValue;
        }

        private const string SESSION_VALUES_ARE_VISIBLE_SETTING_KEY = "show session values";
        private const string TOTAL_VALUES_ARE_VISIBLE_SETTING_KEY = "show total values";
    }
}
