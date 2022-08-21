using System.Globalization;

namespace SessionTracker.Services
{
    public static class ValueToStringExtensions
    {
        public static string To0DecimalPlacesCulturedString(this int value)
        {
            return value.ToString("N0", CultureInfo.CurrentUICulture);
        }

        public static string To0DecimalPlacesCulturedString(this double value)
        {
            return value.ToString("N0", CultureInfo.CurrentUICulture);
        }

        public static string To2DecimalPlacesCulturedString(this double value)
        {
            return value.ToString("N2", CultureInfo.CurrentUICulture);
        }
    }
}