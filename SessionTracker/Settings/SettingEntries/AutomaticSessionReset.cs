namespace SessionTracker.Settings.SettingEntries
{
    public enum AutomaticSessionReset
    {
        // WARNING: only add to bottom. DO NOT REORDER. Enums are stored in settings.json as NUMBER and not as STRING!
        Never,
        OnModuleStart,
        OnDailyReset,
        OnWeeklyReset,
        OnWeeklyNaWvwReset,
        OnWeeklyEuWvwReset,
        OnWeeklyMapBonusRewardsReset
        //On24hAfterLastReset,  // macht wenig sinn. resettet dann plötzlich mitten in session am nächsten tag oder so
        //Custom // at which time, after how many days / dayOfWeek
        //OnModuleStartIfRanLastTimeMoreThanXMinutesAgo // für gaming session die unterbrochen wird (blish crash, pc neustart, mittagspause).
    }
}