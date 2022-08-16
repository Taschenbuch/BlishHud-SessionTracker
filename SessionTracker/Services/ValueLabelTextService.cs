using System.Collections.Generic;
using System.Globalization;
using Blish_HUD.Controls;
using SessionTracker.Models;
using SessionTracker.Settings;

namespace SessionTracker.Services
{
    public class ValueLabelTextService
    {
        public ValueLabelTextService(Dictionary<string, Label> valueLabelByEntryId,
                                     Model model,
                                     SettingService settingService)
        {
            _valueLabelByEntryId = valueLabelByEntryId;
            _model               = model;
            _settingService      = settingService;
        }

        public void UpdateValueLabelTexts()
        {
            foreach (var entry in _model.Entries)
                _valueLabelByEntryId[entry.Id].Text = CreateSessionAndTotalValueText(
                    entry.Value.Session,
                    entry.Value.Total,
                    _settingService.SessionValuesAreVisibleSetting.Value,
                    _settingService.TotalValuesAreVisibleSetting.Value);

            UpdateKillsDeathsRatiosValueLabelTexts(_model, _valueLabelByEntryId);
        }

        // only calculate session ratio. total ratio would be very incorrect because total deaths come from all game modes over the account life time.
        private static void UpdateKillsDeathsRatiosValueLabelTexts(Model model, Dictionary<string, Label> valueLabelByEntryId)
        {
            var wvwKills = model.GetEntry(EntryId.WVW_KILLS).Value.Session;
            var pvpKills = model.GetEntry(EntryId.PVP_KILLS).Value.Session;
            var deaths   = model.GetEntry(EntryId.DEATHS).Value.Session;

            valueLabelByEntryId[EntryId.WVW_KDR].Text = CreateKillsDeathsRatioText(wvwKills, deaths);
            valueLabelByEntryId[EntryId.PVP_KDR].Text = CreateKillsDeathsRatioText(pvpKills, deaths);
        }

        private static string CreateKillsDeathsRatioText(int kills, int deaths)
        {
            var killsDeathsRatio = deaths == 0
                ? 0.00
                : (double)kills / deaths;

            return killsDeathsRatio.ToString("N2", CultureInfo.CurrentUICulture);
        }

        private static string CreateSessionAndTotalValueText(int sessionValue, int totalValue, bool sessionValuesAreVisible, bool totalValuesAreVisible)
        {
            var text = string.Empty;

            if (sessionValuesAreVisible)
                text += sessionValue.ToString("N0", CultureInfo.CurrentUICulture);

            if (sessionValuesAreVisible && totalValuesAreVisible)
                text += " | ";

            if (totalValuesAreVisible)
                text += totalValue.ToString("N0", CultureInfo.CurrentUICulture);

            return text;
        }

        private readonly Dictionary<string, Label> _valueLabelByEntryId;
        private readonly Model _model;
        private readonly SettingService _settingService;
    }
}