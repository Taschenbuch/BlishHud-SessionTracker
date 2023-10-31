using System;
using System.Globalization;
using SessionTracker.Constants;
using SessionTracker.Models;
using SessionTracker.SettingEntries;
using SessionTracker.Value;

namespace SessionTracker.Text
{
    public class StatValueTextService
    {
        public static string CreateKillsDeathsRatioText(int kills, int deaths)
        {
            var killsDeathsRatio = deaths == 0
                ? 0.00
                : (double)kills / deaths;

            return killsDeathsRatio.To2DecimalPlacesCulturedString();
        }

        public static string CreateValueTextForDisplayFormat(
            string sessionValueText,
            string sessionValuePerHourText,
            string totalValueText, 
            ValueDisplayFormat valueDisplayFormat,
            PerHourFormat perHourFormat)
        {
            var perHourText = GetPerHourText(perHourFormat);
            var sessionValuePerHourTextWithUnit = $"{sessionValuePerHourText}{perHourText}";

            switch (valueDisplayFormat)
            {
                case ValueDisplayFormat.SessionValue:
                    return sessionValueText;
                case ValueDisplayFormat.TotalValue:
                    return totalValueText;
                case ValueDisplayFormat.SessionValuePerHour:
                    return sessionValuePerHourTextWithUnit;
                case ValueDisplayFormat.SessionValue_TotalValue:
                    return $"{sessionValueText} | {totalValueText}";
                case ValueDisplayFormat.SessionValue_SessionValuePerHour:
                    return $"{sessionValueText} | {sessionValuePerHourTextWithUnit}";
                case ValueDisplayFormat.SessionValue_SessionValuePerHour_TotalValue:
                    return $"{sessionValueText} | {sessionValuePerHourTextWithUnit} | {totalValueText}";
                case ValueDisplayFormat.SessionValuePerHour_TotalValue:
                    return $"{sessionValuePerHourTextWithUnit} | {totalValueText}";
                default:
                    Module.Logger.Error($"Switch case missing or should not be be handled here: {nameof(ValueDisplayFormat)}.{valueDisplayFormat}.");
                    return sessionValueText;
            }
        }

        private static string GetPerHourText(PerHourFormat perHourFormat)
        {
            switch (perHourFormat)
            {
                case PerHourFormat.XPerHour:
                    return "/hour";
                case PerHourFormat.XPerH:
                    return "/h";
                case PerHourFormat.XH:
                    return "h";
                case PerHourFormat.X:
                    return "";
                default:
                    Module.Logger.Error($"Switch case missing or should not be be handled here: {nameof(PerHourFormat)}.{perHourFormat}.");
                    return "/hour";
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

        public static string CreateValuePerHourText(Stat stat, TimeSpan sessionDuration, CoinDisplayFormat coinDisplayFormat)
        {
            var sessionValuePerHour = GetValuePerHourAsInteger(stat.Value.Session, sessionDuration);
            return CreateValueText(sessionValuePerHour, stat, coinDisplayFormat); // do NOT add "/h" here. That messes up the tooltip text
        }

        public static string CreateValueText(int value, Stat stat, CoinDisplayFormat coinDisplayFormat)
        {
            return stat.IsCurrency && stat.ApiId == CurrencyIds.COIN_IN_COPPER
                ? CreateCoinValueText(value, coinDisplayFormat)
                : value.To0DecimalPlacesCulturedString();
        }

        private static int GetValuePerHourAsInteger(int value, TimeSpan sessionDuration)
        {
            if (value == 0)
                return 0;

            // - the value == 0 guard should be actually enough to catch the session start/reset case. Because on session start all values are 0.
            // - this prevents division by 0 and cases where a value is divided by a very small session duration which would create unrealistic high values
            var sessionJustStarted = sessionDuration.TotalMinutes < 1;
            if (sessionJustStarted)
                return 0;

            var valuePerHour = value / sessionDuration.TotalHours;
            if (valuePerHour >= int.MaxValue)
                return int.MaxValue;

            // int: because everything that has less than 1/h is not interesting to track anyway.
            // showing decimals would make the more common >>1/h values harder to read. (e.g. 12345 vs 12345.67)
            return (int)valuePerHour;
        }
    }
}