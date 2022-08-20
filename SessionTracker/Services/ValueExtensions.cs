using System.Globalization;

namespace SessionTracker.Services
{
    public static class ValueExtensions
    {
        public static string IntToCulturedString(this int value)
        {
            return value.ToString("N0", CultureInfo.CurrentUICulture);
        }

        public static string DoubleToCulturedStringWith2DecimalPlaces(this double value)
        {
            return value.ToString("N2", CultureInfo.CurrentUICulture);
        }

        public static string DoubleToCulturedStringWith1DecimalPlace(this double value)
        {
            return value.ToString("N1", CultureInfo.CurrentUICulture);
        }
    }
}