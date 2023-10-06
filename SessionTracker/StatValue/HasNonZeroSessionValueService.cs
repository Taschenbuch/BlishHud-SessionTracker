using Blish_HUD;
using SessionTracker.SettingEntries;

namespace SessionTracker.StatValue
{
    public class HasNonZeroSessionValueService
    {
        public static bool DetermineForKdr(int kills, int deaths)
        {
            if(deaths == 0 || kills == 0)
                return false;

            // factor 200 because 1/200 would be 0.005 which is rounded to 0.01 in UI. So it has to be 200 * kills for integer division to result in "0" for "0.00"
            var kdrAsInteger = (200 * kills) / deaths; // is not really kdr as integer!

            return kdrAsInteger != 0;
        }

        public static bool DetermineForCoin(int sessionValue, CoinDisplayFormat coinDisplayFormat, Logger logger)
        {
            var divisor = GetCoinDivisor(coinDisplayFormat, logger);
            return (sessionValue / divisor) != 0;
        }

        private static int GetCoinDivisor(CoinDisplayFormat coinDisplayFormat, Logger logger)
        {
            switch (coinDisplayFormat)
            {
                case CoinDisplayFormat.Xg:
                    return 10000;
                case CoinDisplayFormat.XgX:
                    return 1000;
                case CoinDisplayFormat.XgXX:
                    return 100;
                case CoinDisplayFormat.XgXsXc:
                case CoinDisplayFormat.Xc:
                    return 1;
                default:
                    logger.Error($"Missing CoinDisplayFormat case for {coinDisplayFormat}");
                    return 1;
            }
        }
    }
}
