using System.ComponentModel;

namespace SessionTracker.Settings.SettingEntries
{
    public enum CoinDisplayFormat
    {
        [Description("12g")]
        Xg,
        [Description("12g3")]
        XgX,
        [Description("12g34")]
        XgXX,
        [Description("12g34s56c")]
        XgXsXc,
        [Description("123456c")]
        Xc
    }
}