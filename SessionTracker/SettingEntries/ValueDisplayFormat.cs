using System.ComponentModel;

namespace SessionTracker.SettingEntries
{
    public enum ValueDisplayFormat
    {
        [Description("session")]
        SessionValue,
        [Description("total")]
        TotalValue,
        [Description("session | total")]
        SessionValue_TotalValue,
        [Description("session | session/hour")]
        SessionValue_SessionValuePerHour,
        [Description("session | session/hour | total")]
        SessionValue_SessionValuePerHour_TotalValue,
        [Description("session/hour | total")]
        SessionValuePerHour_TotalValue,
        [Description("session/hour")]
        SessionValuePerHour,
    }
}