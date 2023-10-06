namespace SessionTracker.Api
{
    public enum ApiTokenState
    {
        hasNotLoggedIntoCharacterSinceStartingGw2,
        ApiTokenMissing,
        RequiredPermissionsMissing,
        CanAccessApi,
    }
}
