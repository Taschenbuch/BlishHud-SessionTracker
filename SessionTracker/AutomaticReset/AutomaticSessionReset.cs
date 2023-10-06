namespace SessionTracker.AutomaticReset
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
        OnWeeklyMapBonusRewardsReset,
        MinutesAfterModuleShutdown
        //OnFixedTimeDaily
    }
}