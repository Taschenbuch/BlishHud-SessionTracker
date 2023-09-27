using System.ComponentModel;

namespace SessionTracker.Settings.SettingEntries
{
    public enum AutomaticSessionReset
    {
        // WARNING: only add to bottom. DO NOT REORDER. Enums are stored in settings.json as NUMBER and not as STRING!
        [Description("Never")]
        Never,
        [Description("module start")]
        OnModuleStart
        //[Description("daily server reset (02:00 UTC+2)")]
        //OnDailyReset,
        //[Description("24h after last reset")]
        //On24hAfterLastReset,
        //[Description("weekly reset (Monday 09:30 UTC+2)")]
        //OnWeeklyReset,
        //[Description("weekly NA WvW reset (Saturday 04:00 UTC+2)")]
        //OnWeeklyNaWvwReset,
        //[Description("weekly EU WvW reset (Friday 20:00 UTC+2)")]
        //OnWeeklyEuWvwReset,
        //[Description("weekly Map bonus rewards reset (Thursday 22:00 UTC+2 )")]
        //OnWeeklyMapBonusRewardsReset
        //OnWeekly_welche_noch? // https://wiki.guildwars2.com/wiki/Server_reset#Weekly_reset
        //Custom // at which time and after how many days
    }
}