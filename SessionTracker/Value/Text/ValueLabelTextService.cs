using System;
using System.Collections.Generic;
using Blish_HUD;
using Blish_HUD.Controls;
using SessionTracker.Models;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Value.Text
{
    public class ValueLabelTextService : IDisposable
    {
        public ValueLabelTextService(Dictionary<string, Label> valueLabelByEntryId,
                                     Model model,
                                     SettingService settingService,
                                     Logger logger)
        {
            _valueLabelByEntryId = valueLabelByEntryId;
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
            foreach (var entry in _model.Entries)
            {
                if (entry.CurrencyId == CurrencyIds.COIN_IN_COPPER)
                {
                    var sessionCoinText = ValueTextService.CreateCoinValueText(entry.Value.Session, _settingService.CoinDisplayFormatSetting.Value);
                    var totalCoinText   = ValueTextService.CreateCoinValueText(entry.Value.Total, _settingService.CoinDisplayFormatSetting.Value);

                    _valueLabelByEntryId[entry.Id].Text = ValueTextService.CreateSessionAndTotalValueText(
                        sessionCoinText,
                        totalCoinText,
                        _settingService.SessionValuesAreVisibleSetting.Value,
                        _settingService.TotalValuesAreVisibleSetting.Value);
                }
                else if (entry.Id == EntryId.WVW_KDR)
                {
                    // only calculate session ratio. total ratio would be very incorrect because total deaths come from all game modes over the account life time.
                    var wvwKills = _model.GetEntry(EntryId.WVW_KILLS).Value.Session;
                    var deaths   = _model.GetEntry(EntryId.DEATHS).Value.Session;
                    _valueLabelByEntryId[EntryId.WVW_KDR].Text = ValueTextService.CreateKillsDeathsRatioText(wvwKills, deaths);
                }
                else if (entry.Id == EntryId.PVP_KDR)
                {
                    // only calculate session ratio. total ratio would be very incorrect because total deaths come from all game modes over the account life time.
                    var pvpKills = _model.GetEntry(EntryId.PVP_KILLS).Value.Session;
                    var deaths   = _model.GetEntry(EntryId.DEATHS).Value.Session;
                    _valueLabelByEntryId[EntryId.PVP_KDR].Text = ValueTextService.CreateKillsDeathsRatioText(pvpKills, deaths);
                }
                else
                {
                    var sessionValueText = entry.Value.Session.To0DecimalPlacesCulturedString();
                    var totalValueText   = entry.Value.Total.To0DecimalPlacesCulturedString();

                    _valueLabelByEntryId[entry.Id].Text = ValueTextService.CreateSessionAndTotalValueText(
                        sessionValueText,
                        totalValueText,
                        _settingService.SessionValuesAreVisibleSetting.Value,
                        _settingService.TotalValuesAreVisibleSetting.Value);
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
        }

        private readonly Dictionary<string, Label> _valueLabelByEntryId;
        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly Logger _logger;
    }
}