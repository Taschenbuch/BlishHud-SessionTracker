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
            _valueLabelByStatId  = valueLabelByStatId;
            _model               = model;
            _settingService      = settingService;

            settingService.ValuesSeparatorSetting.SettingChanged    += DoUpdateValueLabelTexts;
            settingService.PerHourUnitText.SettingChanged           += DoUpdateValueLabelTexts;
            settingService.ValueDisplayFormatSetting.SettingChanged += DoUpdateValueLabelTexts;
            settingService.CoinDisplayFormatSetting.SettingChanged  += OnCoinDisplayFormatSettingChanged;
        }

        public void Dispose()
        {
            _settingService.ValuesSeparatorSetting.SettingChanged    -= DoUpdateValueLabelTexts;
            _settingService.PerHourUnitText.SettingChanged           -= DoUpdateValueLabelTexts;
            _settingService.ValueDisplayFormatSetting.SettingChanged -= DoUpdateValueLabelTexts;
            _settingService.CoinDisplayFormatSetting.SettingChanged  -= OnCoinDisplayFormatSettingChanged;
        }

        public void UpdateValueLabelTexts()
        {
            foreach (var stat in _model.Stats)
            {
                if (stat.IsKdr)
                {
                    // only calculate session ratio. total ratio would be very incorrect because total deaths come from all game modes over the account life time.
                    var killsStatId = stat.Id == StatId.PVP_KDR 
                        ? StatId.PVP_KILLS 
                        : StatId.WVW_KILLS;

                    var kills  = _model.GetStat(killsStatId).Value.Session;
                    var deaths = _model.GetStat(StatId.DEATHS).Value.Session;
                    stat.HasNonZeroSessionValue = HasNonZeroSessionValueService.DetermineForKdr(kills, deaths);
                    _valueLabelByStatId[stat.Id].Text = StatValueTextService.CreateKillsDeathsRatioText(kills, deaths);
                }
                else
                {
                    var sessionValueText        = StatValueTextService.CreateValueText(stat.Value.Session, stat, _settingService.CoinDisplayFormatSetting.Value);
                    var totalValueText          = StatValueTextService.CreateValueText(stat.Value.Total, stat, _settingService.CoinDisplayFormatSetting.Value);
                    var sessionValuePerHourText = StatValueTextService.CreateValuePerHourText(stat, _model.SessionDuration.Value, _settingService.CoinDisplayFormatSetting.Value);

                    stat.HasNonZeroSessionValue = stat.IsCoin
                        ? HasNonZeroSessionValueService.DetermineForCoin(stat.Value.Session, _settingService.CoinDisplayFormatSetting.Value)
                        : stat.Value.Session != 0;

                    _valueLabelByStatId[stat.Id].Text = StatValueTextService.CreateValueTextForDisplayFormat(
                        sessionValueText,
                        sessionValuePerHourText,
                        totalValueText,
                        _settingService.ValueDisplayFormatSetting.Value,
                        _settingService.PerHourUnitText.Value,
                        _settingService.ValuesSeparatorSetting.Value);
                }
            }
        }

        private void DoUpdateValueLabelTexts<T>(object sender, ValueChangedEventArgs<T> e)
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