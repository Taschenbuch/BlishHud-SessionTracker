﻿using System.Collections.Generic;
using System;
using SessionTracker.AutomaticReset;
using SessionTracker.DateTimeUtcNow;

namespace SessionTracker.Reset
{
    public class ResetDropDownService
    {
        public static Dictionary<AutomaticSessionReset, string> GetDropDownTextsForAutomaticSessionResetSetting()
        {
            return new Dictionary<AutomaticSessionReset, string>
            {
                [AutomaticSessionReset.Never] = "Never (right click menu icon for manual reset)",
                [AutomaticSessionReset.OnModuleStart] = "On module start",
                [AutomaticSessionReset.OnDailyReset] = $"On daily reset ({GetNextDailyResetLocalTime()})",
                [AutomaticSessionReset.OnWeeklyReset] = $"On weekly reset ({GetNextWeeklyResetInLocalTime(AutomaticSessionReset.OnWeeklyReset)})",
                [AutomaticSessionReset.OnWeeklyNaWvwReset] = $"On weekly NA WvW reset ({GetNextWeeklyResetInLocalTime(AutomaticSessionReset.OnWeeklyNaWvwReset)})",
                [AutomaticSessionReset.OnWeeklyEuWvwReset] = $"On weekly EU WvW reset ({GetNextWeeklyResetInLocalTime(AutomaticSessionReset.OnWeeklyEuWvwReset)})",
                [AutomaticSessionReset.OnWeeklyMapBonusRewardsReset] = $"On weekly map bonus rewards reset ({GetNextWeeklyResetInLocalTime(AutomaticSessionReset.OnWeeklyMapBonusRewardsReset)})",
                [AutomaticSessionReset.MinutesAfterModuleShutdown] = $"Minutes after module shutdown (change amount of minutes below) "
            };
        }

        private static string GetNextWeeklyResetInLocalTime(AutomaticSessionReset automaticSessionReset)
        {
            return ResetService.GetNextResetDateTimeUtc(automaticSessionReset, DateTimeService.UtcNow, DOES_NOT_MATTER)
                .ToLocalTime()
                .ToString("dddd HH:mm");
        }

        private static string GetNextDailyResetLocalTime()
        {
            return ResetService.GetNextResetDateTimeUtc(AutomaticSessionReset.OnDailyReset, DateTimeService.UtcNow, DOES_NOT_MATTER)
                .ToLocalTime()
                .ToString("HH:mm");
        }

        private const int DOES_NOT_MATTER = 60;
    }
}
