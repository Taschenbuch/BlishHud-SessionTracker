namespace SessionTracker.Controls
{
    public enum UpdateLoopState
    {
        WaitingForApiTokenAfterModuleStart,
        PauseBetweenStartNewSessionRetries,
        PauseBeforeUpdatingSession,
        WaitingForApiResponse,
        StartingNewSession,
        UpdatingSession,
    }
}