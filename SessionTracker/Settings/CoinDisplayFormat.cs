using System.ComponentModel;

namespace SessionTracker.Settings
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
        XgXXsXXc,
        [Description("123456c")]
        Xc
    }
}