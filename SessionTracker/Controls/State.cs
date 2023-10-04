namespace SessionTracker.Controls
{
    public enum State
    {
        WaitForApiTokenAfterModuleStart,
        WaitBeforeStartSession,
        WaitBeforeUpdateSession,
        WaitForApiResponse,
        StartSession,
        UpdateSession,
    }
}