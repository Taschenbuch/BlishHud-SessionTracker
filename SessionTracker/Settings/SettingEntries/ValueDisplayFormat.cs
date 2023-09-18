using System.ComponentModel;

namespace SessionTracker.Settings.SettingEntries
{
    public enum ValueDisplayFormat
    {
        [Description("Session value")]
        SessionValue,
        [Description("Total value")]
        TotalValue,
        [Description("Session and total value")]
        SessionAndTotalValue,
    }
}