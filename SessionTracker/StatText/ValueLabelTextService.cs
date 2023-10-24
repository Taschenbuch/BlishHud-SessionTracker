using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using SessionTracker.Constants;
using SessionTracker.Models;
using SessionTracker.SettingEntries;
using SessionTracker.StatValue;
using SessionTracker.Value;

namespace SessionTracker.Text
{
    public class ValueLabelTextService : IDisposable
    {
        public ValueLabelTextService(Dictionary<string, Label> valueLabelByStatId,
                                     Model model,
                                     SettingService settingService)
        {
            _valueLabelByStatId = valueLabelByStatId;
            _model               = model;
            _settingService      = settingService;

            settingService.ValueDisplayFormatSetting.SettingChanged += ValueDisplayFormatSettingChanged;
            settingService.CoinDisplayFormatSetting.SettingChanged  += OnCoinDisplayFormatSettingChanged;
        }

        public void Dispose()
        {
            _settingService.ValueDisplayFormatSetting.SettingChanged -= ValueDisplayFormatSettingChanged;
            _settingService.CoinDisplayFormatSetting.SettingChanged -= OnCoinDisplayFormatSettingChanged;
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
                        _settingService.ValueDisplayFormatSetting.Value);

                    stat.HasNonZeroSessionValue = HasNonZeroSessionValueService.DetermineForCoin(stat.Value.Session, _settingService.CoinDisplayFormatSetting.Value);
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
                        _settingService.ValueDisplayFormatSetting.Value);

                    stat.HasNonZeroSessionValue = stat.Value.Session != 0;
                }
            }
        }

        private void ValueDisplayFormatSettingChanged(object sender, ValueChangedEventArgs<ValueDisplayFormat> e)
        {
            UpdateValueLabelTexts();
        }

        private void OnCoinDisplayFormatSettingChanged(object sender, ValueChangedEventArgs<CoinDisplayFormat> e)
        {
            UpdateValueLabelTexts();
            _model.UiHasToBeUpdated = true; // because depending on the coin format value the coin stat has to be hidden by StatsWithZeroValueAreHiddenSetting
        }

        private readonly Dictionary<string, Label> _valueLabelByStatId;
        private readonly Model _model;
        private readonly SettingService _settingService;
    }
}