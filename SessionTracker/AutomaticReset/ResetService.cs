using Blish_HUD;
using Blish_HUD.Settings;
using SessionTracker.AutomaticReset;
using SessionTracker.DateTimeUtcNow;
using SessionTracker.Models;
using SessionTracker.OtherServices;
using System;

namespace SessionTracker.Reset
{
    public class ResetService : IDisposable
    {
        public ResetService(Services services)
        {
            _model = services.Model;
            _automaticSessionResetSetting = services.SettingService.AutomaticSessionResetSetting;
            _minutesUntilResetAfterModuleShutdownSetting = services.SettingService.MinutesUntilResetAfterModuleShutdownSetting;
            
            services.SettingService.AutomaticSessionResetSetting.SettingChanged += AutomaticSessionResetSettingChanged;
        }

        public void Dispose()
        {
            _automaticSessionResetSetting.SettingChanged -= AutomaticSessionResetSettingChanged;
        }

        // init requried when lastResetTime hasnt existed in model.json before (module update) or model.json is freshly created (new module installation or file was deleted)
        public void InitializeNextResetDateTimeIfNecessary()
        {
            _isInitalisationRequiredForNextResetDateTime = _model.NextResetDateTimeUtc == Model.UNDEFINED_RESET_DATE_TIME;
            if (_isInitalisationRequiredForNextResetDateTime)
                _model.NextResetDateTimeUtc = GetNextResetDateTimeUtc(_automaticSessionResetSetting.Value, DateTimeService.UtcNow, _minutesUntilResetAfterModuleShutdownSetting.Value);
        }

        public bool HasToAutomaticallyResetSession(ResetCheckLocation resetCheckLocation)
        {
            if (_isInitalisationRequiredForNextResetDateTime && resetCheckLocation == ResetCheckLocation.ModuleStartup)
                return true;

            var isPastResetDate = _model.NextResetDateTimeUtc < DateTimeService.UtcNow;
            var hasToReset = _automaticSessionResetSetting.Value switch
            {
                AutomaticSessionReset.Never => false,
                AutomaticSessionReset.OnModuleStart => resetCheckLocation == ResetCheckLocation.ModuleStartup,
                AutomaticSessionReset.MinutesAfterModuleShutdown => resetCheckLocation == ResetCheckLocation.ModuleStartup && isPastResetDate,
                _ => isPastResetDate,
            };

            if (hasToReset)
                Module.Logger.Info($"Automatic reset required because past rest DateTime. " +
                                   $"ResetWhere: {resetCheckLocation}; " +
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

        private void AutomaticSessionResetSettingChanged(object sender, ValueChangedEventArgs<AutomaticSessionReset> e)
        {
            UpdateNextResetDateTime();
        }

        private readonly Model _model;
        private readonly SettingEntry<AutomaticSessionReset> _automaticSessionResetSetting;
        private readonly SettingEntry<int> _minutesUntilResetAfterModuleShutdownSetting;
        private bool _isInitalisationRequiredForNextResetDateTime; 
    }
}
