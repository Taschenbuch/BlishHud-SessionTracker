﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using SessionTracker.Models;
using SessionTracker.Settings;

namespace SessionTracker.Services
{
    public class ValueLabelTextService
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
        }

        public void UpdateValueLabelTexts()
        {
            foreach (var entry in _model.Entries)
            {
                if (entry.CurrencyId == CurrencyIds.GOLD_IN_COPPER)
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
                    var sessionValueText = entry.Value.Session.ToString("N0", CultureInfo.CurrentUICulture);
                    var totalValueText   = entry.Value.Total.ToString("N0", CultureInfo.CurrentUICulture);

                    _valueLabelByEntryId[entry.Id].Text = ValueTextService.CreateSessionAndTotalValueText(
                        sessionValueText,
                        totalValueText,
                        _settingService.SessionValuesAreVisibleSetting.Value,
                        _settingService.TotalValuesAreVisibleSetting.Value);
                }
            }
        }

        private readonly Dictionary<string, Label> _valueLabelByEntryId;
        private readonly Model _model;
        private readonly SettingService _settingService;
        private readonly Logger _logger;
    }
}