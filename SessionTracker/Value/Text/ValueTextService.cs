using System;
using System.Globalization;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Value.Text
{
    public class ValueTextService
    {
        public static string CreateKillsDeathsRatioText(int kills, int deaths)
        {
            var killsDeathsRatio = deaths == 0
                ? 0.00
                : (double)kills / deaths;

            return killsDeathsRatio.To2DecimalPlacesCulturedString();
        }

        public static string CreateSessionAndTotalValueText(string sessionValueText, string totalValueText, bool sessionValuesAreVisible, bool totalValuesAreVisible)
        {
            var text = string.Empty;

            if (sessionValuesAreVisible)
                text += sessionValueText;

            if (sessionValuesAreVisible && totalValuesAreVisible)
                text += " | ";

            if (totalValuesAreVisible)
                text += totalValueText;

            return text;
        }

        public static string CreateCoinValueText(int valueInCopper, CoinDisplayFormat coinDisplayFormat)
        {
            var sign = valueInCopper < 0 ? "-" : "";
            var unsignedValueInCopper = Math.Abs(valueInCopper);

            var gold = unsignedValueInCopper / 10000;
            var silver = (unsignedValueInCopper - gold * 10000) / 100;
            var copper = unsignedValueInCopper % 100;

            var goldText = gold.To0DecimalPlacesCulturedString();
            var silverInTens = silver / 10;
            var silver2DigitText = silver.ToString("00", CultureInfo.CurrentUICulture);
            var allInCopperText = unsignedValueInCopper.To0DecimalPlacesCulturedString();

            switch (coinDisplayFormat)
            {
                case CoinDisplayFormat.Xc:
                    return $"{sign}{allInCopperText}c";
                case CoinDisplayFormat.Xg:
                    return gold == 0
                        ? "0g"
                        : $"{sign}{goldText}g";
                case CoinDisplayFormat.XgX:
                    return gold == 0 && silverInTens == 0
                        ? "0g"
                        : $"{sign}{goldText}g{silverInTens}";
                case CoinDisplayFormat.XgXX:
                    return gold == 0 && silver == 0
                        ? "0g"
                        : $"{sign}{goldText}g{silver2DigitText}";
                case CoinDisplayFormat.XgXsXc:
                default:
                    return gold == 0 && silver == 0 && copper == 0
                        ? "0g\u20090s\u20090c"
                        : $"{sign}{goldText}g\u2009{silver}s\u2009{copper}c";
            }
        }
    }
}