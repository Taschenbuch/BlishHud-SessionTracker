namespace SessionTracker.Controls
{
    public enum State
    {
        WaitForApiTokenAfterModuleStart,
        WaitBeforeResetAndInitStats,
        ResetAndInitStats,
        WaitBeforeUpdateStats,
        UpdateStats,
        WaitForApiResponse,
    }
}