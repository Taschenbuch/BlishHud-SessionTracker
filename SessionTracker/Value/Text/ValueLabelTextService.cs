using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using SessionTracker.Models;
using SessionTracker.Models.Constants;
using SessionTracker.Services;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Value.Text
{
    public class ValueLabelTextService : IDisposable
    {
        public ValueLabelTextService(Dictionary<string, Label> valueLabelByStatId,
                                     Model model,
                                     SettingService settingService,
                                     Logger logger)
        {
            _valueLabelByStatId = valueLabelByStatId;
            _model               = model;
            _settingService      = settingService;
            _logger              = logger;

            settingService.SessionValuesAreVisibleSetting.SettingChanged += OnSessionValueVisibilitySettingChanged;
            settingService.TotalValuesAreVisibleSetting.SettingChanged   += OnTotalValueVisibilitySettingChanged;
            settingService.CoinDisplayFormatSetting.SettingChanged       += OnCoinDisplayFormatSettingChanged;
        }

        public void Dispose()
        {
            _settingService.SessionValuesAreVisibleSetting.SettingChanged -= OnSessionValueVisibilitySettingChanged;
            _settingService.TotalValuesAreVisibleSetting.SettingChanged   -= OnTotalValueVisibilitySettingChanged;
            _settingService.CoinDisplayFormatSetting.SettingChanged       -= OnCoinDisplayFormatSettingChanged;
        }

        public void UpdateValueLabelTexts()
        {
            foreach (var stat in _model.Stats)
            {
                if (stat.ApiId == CurrencyIds.COIN_IN_COPPER)
                {
                    var sessionCoinText = ValueTextService.CreateCoinValueText(stat.Value.Session, _settingService.CoinDisplayFormatSetting.Value);
                    var totalCoinText = ValueTextService.CreateCoinValueText(stat.Value.Total, _settingService.CoinDisplayFormatSetting.Value);

                    _valueLabelByStatId[stat.Id].Text = ValueTextService.CreateSessionAndTotalValueText(
                        sessionCoinText,
                        totalCoinText,
                        _settingService.SessionValuesAreVisibleSetting.Value,
                        _settingService.TotalValuesAreVisibleSetting.Value);

                    stat.HasNonZeroSessionValue = HasNonZeroSessionValueService.DetermineForCoin(stat.Value.Session, _settingService.CoinDisplayFormatSetting.Value, _logger);
                }
                else if (stat.Id == StatId.WVW_KDR)
                {
                    // only calculate session ratio. total ratio would be very incorrect because total deaths come from all game modes over the account life time.
                    var kills = _model.GetStat(StatId.WVW_KILLS).Value.Session;
                    var deaths = _model.GetStat(StatId.DEATHS).Value.Session;
                    _valueLabelByStatId[StatId.WVW_KDR].Text = ValueTextService.CreateKillsDeathsRatioText(kills, deaths);
                    stat.HasNonZeroSessionValue = HasNonZeroSessionValueService.DetermineForKdr(kills, deaths);
                }
                else if (stat.Id == StatId.PVP_KDR)
                {
                    // only calculate session ratio. total ratio would be very incorrect because total deaths come from all game modes over the account life time.
                    var kills = _model.GetStat(StatId.PVP_KILLS).Value.Session;
                    var deaths = _model.GetStat(StatId.DEATHS).Value.Session;
                    _valueLabelByStatId[StatId.PVP_KDR].Text = ValueTextService.CreateKillsDeathsRatioText(kills, deaths);
                    stat.HasNonZeroSessionValue = HasNonZeroSessionValueService.DetermineForKdr(kills, deaths);
                }
                else
                {
                    var sessionValueText = stat.Value.Session.To0DecimalPlacesCulturedString();
                    var totalValueText   = stat.Value.Total.To0DecimalPlacesCulturedString();

                    _valueLabelByStatId[stat.Id].Text = ValueTextService.CreateSessionAndTotalValueText(
                        sessionValueText,
                        totalValueText,
                        _settingService.SessionValuesAreVisibleSetting.Value,
                        _settingService.TotalValuesAreVisibleSetting.Value);

                    stat.HasNonZeroSessionValue = stat.Value.Session != 0;
                }
            }
        }        

        private void OnTotalValueVisibilitySettingChanged(object sender, ValueChangedEventArgs<bool> valueChangedEventArgs)
        {
            if (_settingService.SessionValuesAreVisibleSetting.Value == false && _settingService.TotalValuesAreVisibleSetting.Value == false)
                _settingService.SessionValuesAreVisibleSetting.Value = true;

            UpdateValueLabelTexts();
        }

        private void OnSessionValueVisibilitySettingChanged(object sender, ValueChangedEventArgs<bool> valueChangedEventArgs)
        {
            if (_settingService.SessionValuesAreVisibleSetting.Value == false && _settingService.TotalValuesAreVisibleSetting.Value == false)
                _settingService.TotalValuesAreVisibleSetting.Value = true;

            UpdateValueLabelTexts();
        }

        private void OnCoinDisplayFormatSettingChanged(object sender, ValueChangedEventArgs<CoinDisplayFormat> e)
        {
            UpdateValueLabelTexts();
            _model.UiHasToBeUpdated = true; // because depending on the coin format value has to be hidden by hideZeroValueSetting
        }

        private readonly Dictionary<string, Label> _valueLabelByStatId;
        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly Logger _logger;
    }
}