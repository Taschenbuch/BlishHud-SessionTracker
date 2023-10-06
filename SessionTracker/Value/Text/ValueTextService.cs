﻿using System;
using System.Globalization;
using Blish_HUD;
using SessionTracker.SettingEntries;

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

        public static string CreateSessionAndTotalValueText(string sessionValueText, string totalValueText, ValueDisplayFormat valueDisplayFormat, Logger logger)
        {
            switch (valueDisplayFormat)
            {
                case ValueDisplayFormat.SessionValue:
                    return sessionValueText;
                case ValueDisplayFormat.TotalValue:
                    return totalValueText;
                case ValueDisplayFormat.SessionAndTotalValue:
                    return $"{sessionValueText} | {totalValueText}";
                default:
                    logger.Error($"Missing ValueDisplayFormat case for {valueDisplayFormat}");
                    return sessionValueText;
            }
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

            return coinDisplayFormat switch
            {
                CoinDisplayFormat.Xc => $"{sign}{allInCopperText}c",
                CoinDisplayFormat.Xg => gold == 0
                                        ? "0g"
                                        : $"{sign}{goldText}g",
                CoinDisplayFormat.XgX => gold == 0 && silverInTens == 0
                                        ? "0g"
                                        : $"{sign}{goldText}g{silverInTens}",
                CoinDisplayFormat.XgXX => gold == 0 && silver == 0
                                        ? "0g"
                                        : $"{sign}{goldText}g{silver2DigitText}",
                _ => gold == 0 && silver == 0 && copper == 0
                                        ? "0g\u20090s\u20090c"
                                        : $"{sign}{goldText}g\u2009{silver}s\u2009{copper}c",
            };
        }
    }
}