namespace SessionTracker.Controls
{
    public enum State
    {
        WaitForApiTokenAfterModuleStart,
        WaitBeforeResetAndInitStats,
        ResetAndInitStats,
        UpdateStats,
        WaitForApiResponse,
    }
}