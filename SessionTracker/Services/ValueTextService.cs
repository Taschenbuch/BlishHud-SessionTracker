using System;
using System.Globalization;
using Blish_HUD;
using SessionTracker.Settings;

namespace SessionTracker.Services
{
    public class ValueTextService
    {
        public static string CreateKillsDeathsRatioText(int kills, int deaths)
        {
            var killsDeathsRatio = deaths == 0
                ? 0.00
                : (double)kills / deaths;

            return killsDeathsRatio.ToString("N2", CultureInfo.CurrentUICulture);
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
            var sign                  = valueInCopper < 0 ? "-" : "";
            var unsignedValueInCopper = Math.Abs(valueInCopper);

            var gold   = unsignedValueInCopper / 10000;
            var silver = (unsignedValueInCopper - gold * 10000) / 100;
            var copper = unsignedValueInCopper % 100;

            var goldText         = gold.ToString("N0", CultureInfo.CurrentUICulture);
            var silverInTens     = silver / 10;
            var silver2DigitText = silver.ToString("00", CultureInfo.CurrentUICulture);
            var allInCopperText  = unsignedValueInCopper.ToString("N0", CultureInfo.CurrentUICulture);

            switch (coinDisplayFormat)
            {
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
                    return gold == 0 && silver == 0 && copper == 0
                        ? "0g\u20090s\u20090c"
                        : $"{sign}{goldText}g\u2009{silver}s\u2009{copper}c";
                case CoinDisplayFormat.Xc:
                    return $"{sign}{allInCopperText}c";
                default:
                    //logger.Error($"Error: unknown coinDisplayFormat: {coinDisplayFormat}. Use XgXsXc format as fallback");
                    return gold == 0 && silver == 0 && copper == 0
                        ? "0g\u20090s\u20090c"
                        : $"{sign}{goldText}g\u2009{silver}s\u2009{copper}c";
            }
        }
    }
}