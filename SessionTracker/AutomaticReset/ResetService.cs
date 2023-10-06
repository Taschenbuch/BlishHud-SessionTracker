using Blish_HUD;
using Blish_HUD.Settings;
using SessionTracker.AutomaticReset;
using SessionTracker.DateTimeUtcNow;
using SessionTracker.Models;
using System;

namespace SessionTracker.Reset
{
    public class ResetService : IDisposable
    {
        public ResetService(Model model, SettingEntry<AutomaticSessionReset> automaticSessionResetSetting, SettingEntry<int> minutesUntilResetAfterModuleShutdownSetting)
        {
            _model = model;
            _automaticSessionResetSetting = automaticSessionResetSetting;
            _minutesUntilResetAfterModuleShutdownSetting = minutesUntilResetAfterModuleShutdownSetting;
            automaticSessionResetSetting.SettingChanged += AutomaticSessionResetSettingChanged;
            model.NextResetDateTimeUtc = InitializeNextResetDateTime(automaticSessionResetSetting.Value, minutesUntilResetAfterModuleShutdownSetting.Value, model.NextResetDateTimeUtc);
        }

        public void Dispose()
        {
            _automaticSessionResetSetting.SettingChanged -= AutomaticSessionResetSettingChanged;
        }

        public bool HasToAutomaticallyResetSession(ResetWhere resetWhere)
        {
            var isPastResetDate = _model.NextResetDateTimeUtc < DateTimeService.UtcNow;
            var hasToReset = _automaticSessionResetSetting.Value switch
            {
                AutomaticSessionReset.Never => false,
                AutomaticSessionReset.OnModuleStart => resetWhere == ResetWhere.ModuleStartup,
                AutomaticSessionReset.MinutesAfterModuleShutdown => resetWhere == ResetWhere.ModuleStartup && isPastResetDate,
                _ => isPastResetDate,
            };

            if (hasToReset)
                Module.Logger.Info($"Automatic reset required because past rest DateTime. " +
                                   $"ResetWhere: {resetWhere}; " +
                                   $"AutomaticSessionResetSetting: {_automaticSessionResetSetting.Value}; " +
                                   $"ResetDateTimeUtc {_model.NextResetDateTimeUtc}");

            return hasToReset;
        }

        public void UpdateNextResetDateTimeForMinutesAfterShutdownReset()
        {
            if (_automaticSessionResetSetting.Value == AutomaticSessionReset.MinutesAfterModuleShutdown)
                UpdateNextResetDateTime();
        }

        public void UpdateNextResetDateTime()
        {
           _model.NextResetDateTimeUtc = GetNextResetDateTimeUtc(_automaticSessionResetSetting.Value, DateTimeService.UtcNow, _minutesUntilResetAfterModuleShutdownSetting.Value);
        }

        public static DateTime GetNextResetDateTimeUtc(AutomaticSessionReset automaticSessionReset, DateTime dateTimeUtc, int minutesUntilResetAfterModuleShutdown)
        {
            switch (automaticSessionReset)
            {
                case AutomaticSessionReset.OnModuleStart:
                case AutomaticSessionReset.Never:
                    return Model.NEVER_OR_ON_MODULE_START_RESET_DATE_TIME;
                case AutomaticSessionReset.MinutesAfterModuleShutdown:
                    return dateTimeUtc + TimeSpan.FromMinutes(minutesUntilResetAfterModuleShutdown);
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
                    Module.Logger.Error($"Fallback: never reset. Because switch case missing or should not be be handled here: {nameof(AutomaticSessionReset)}.{automaticSessionReset}.");
                    return Model.NEVER_OR_ON_MODULE_START_RESET_DATE_TIME;
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

        private static DateTime InitializeNextResetDateTime(AutomaticSessionReset automaticSessionReset, int minutesAfterModuleShutdownUntilResetSetting, DateTime nextResetDateTimeUtc)
        {
            var initializeRequired = nextResetDateTimeUtc == Model.UNDEFINED_RESET_DATE_TIME;
            return initializeRequired
                ? GetNextResetDateTimeUtc(automaticSessionReset, DateTimeService.UtcNow, minutesAfterModuleShutdownUntilResetSetting)
                : nextResetDateTimeUtc;
        }

        private void AutomaticSessionResetSettingChanged(object sender, ValueChangedEventArgs<AutomaticSessionReset> e)
        {
            UpdateNextResetDateTime();
        }

        private readonly Model _model;
        private readonly SettingEntry<AutomaticSessionReset> _automaticSessionResetSetting;
        private readonly SettingEntry<int> _minutesUntilResetAfterModuleShutdownSetting;
    }
}
