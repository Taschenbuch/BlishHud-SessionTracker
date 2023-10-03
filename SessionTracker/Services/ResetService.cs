using Blish_HUD;
using Blish_HUD.Settings;
using SessionTracker.Models;
using SessionTracker.Settings.SettingEntries;
using System;

namespace SessionTracker.Services
{
    public class ResetService : IDisposable
    {
        public ResetService(Model model, SettingEntry<AutomaticSessionReset> automaticSessionResetSetting)
        {
            _model = model;
            _automaticSessionResetSetting = automaticSessionResetSetting;

            automaticSessionResetSetting.SettingChanged += AutomaticSessionResetSettingChanged;
            model.NextResetDateTimeUtc = InitializeNextResetDateTime(automaticSessionResetSetting.Value, model.NextResetDateTimeUtc);
        }

        public void Dispose()
        {
            _automaticSessionResetSetting.SettingChanged -= AutomaticSessionResetSettingChanged;
        }

        public bool HasToAutomaticallyResetSessionOnModuleStartup()
        {
            return _automaticSessionResetSetting.Value switch
            {
                AutomaticSessionReset.Never => false,
                AutomaticSessionReset.OnModuleStart => true,
                _ => _model.NextResetDateTimeUtc < DateTimeService.UtcNow,
            };
        }

        public bool HasToAutomaticallyResetSessionOnUpdate()
        {
            return _automaticSessionResetSetting.Value switch
            {
                AutomaticSessionReset.Never => false,
                AutomaticSessionReset.OnModuleStart => false,
                _ => _model.NextResetDateTimeUtc < DateTimeService.UtcNow,
            };
        }

        public void UpdateNextResetDateTimetInModel()
        {
            _model.NextResetDateTimeUtc = GetNextResetDateTimeUtc(_automaticSessionResetSetting.Value, DateTimeService.UtcNow);
        }

        public static DateTime GetNextResetDateTimeUtc(AutomaticSessionReset automaticSessionReset, DateTime dateTimeUtc)
        {
            switch (automaticSessionReset)
            {
                case AutomaticSessionReset.OnModuleStart:
                case AutomaticSessionReset.Never:
                    return Model.NEVER_OR_ON_MODULE_START_RESET_DATE_TIME;
                case AutomaticSessionReset.OnDailyReset:
                    return GetNextDailyResetDateTimeUtc(dateTimeUtc);
                case AutomaticSessionReset.OnWeeklyReset:
                    return GetNextWeeklyResetDateTimeUtc(dateTimeUtc, DayOfWeek.Monday, 7, 30);
                case AutomaticSessionReset.OnWeeklyNaWvwReset:
                    return GetNextWeeklyResetDateTimeUtc(dateTimeUtc, DayOfWeek.Saturday, 2);
                case AutomaticSessionReset.OnWeeklyEuWvwReset:
                    return GetNextWeeklyResetDateTimeUtc(dateTimeUtc, DayOfWeek.Friday, 18);
                case AutomaticSessionReset.OnWeeklyMapBonusRewardsReset:
                    return GetNextWeeklyResetDateTimeUtc(dateTimeUtc, DayOfWeek.Thursday, 20);
                default:
                {
                    Module.Logger.Error($"Fallback: never reset. Because switch case missing or should not be be handled here: {nameof(AutomaticSessionReset)}.{automaticSessionReset}.");
                    return Model.NEVER_OR_ON_MODULE_START_RESET_DATE_TIME;
                }
            }
        }

        private static DateTime GetNextWeeklyResetDateTimeUtc(DateTime dateTimeUtc, DayOfWeek resetDayOfWeekUtc, double resetHoursUtc, double resetMinutesUtc = 0)
        {
            var daysUntilWeeklyReset = (resetDayOfWeekUtc - dateTimeUtc.DayOfWeek + 7) % 7;
            var weeklyResetDateTimeUtc = dateTimeUtc.Date.AddDays(daysUntilWeeklyReset).AddHours(resetHoursUtc).AddMinutes(resetMinutesUtc);
            return dateTimeUtc < weeklyResetDateTimeUtc
                ? weeklyResetDateTimeUtc
                : weeklyResetDateTimeUtc.AddDays(7);
        }

        private static DateTime GetNextDailyResetDateTimeUtc(DateTime dateTimeUtc)
        {
            return dateTimeUtc.Date.AddDays(1); // daily reset is at 00:00 UTC
        }

        private static DateTime InitializeNextResetDateTime(AutomaticSessionReset automaticSessionReset, DateTime nextResetDateTimeUtc)
        {
            var initializeRequired = nextResetDateTimeUtc == Model.UNDEFINED_RESET_DATE_TIME;
            return initializeRequired
                ? GetNextResetDateTimeUtc(automaticSessionReset, DateTimeService.UtcNow)
                : nextResetDateTimeUtc;
        }

        private void AutomaticSessionResetSettingChanged(object sender, ValueChangedEventArgs<AutomaticSessionReset> e)
        {
            UpdateNextResetDateTimetInModel();
        }

        private readonly Model _model;
        private SettingEntry<AutomaticSessionReset> _automaticSessionResetSetting;
    }
}
