namespace SessionTracker.StatsWindow
{
    public enum UpdateLoopState
    {
        WaitingForApiTokenAfterModuleStart,
        PauseBetweenStartNewSessionRetries,
        PauseBeforeUpdatingSession,
        WaitingForApiResponse,
        StartingNewSession,
        UpdatingSession,
        Error
    }
}