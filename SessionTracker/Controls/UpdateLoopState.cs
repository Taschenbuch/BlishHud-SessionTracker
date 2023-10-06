namespace SessionTracker.Controls
{
    public enum UpdateLoopState
    {
        WaitForApiTokenAfterModuleStart,
        WaitBeforeStartSession,
        WaitBeforeUpdateSession,
        WaitForApiResponse,
        StartSession,
        UpdateSession,
    }
}