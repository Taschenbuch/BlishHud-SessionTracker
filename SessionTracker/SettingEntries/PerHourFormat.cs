using System.ComponentModel;

namespace SessionTracker.SettingEntries
{
    public enum PerHourFormat
    {
        [Description("12/hour")]
        XPerHour,
        [Description("12/h")]
        XPerH,
        [Description("12h")]
        XH,
        [Description("12")]
        X,
    }
}