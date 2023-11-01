using System.Collections.Generic;
using Blish_HUD;
using Gw2Sharp.WebApi;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class LocalizedText
    {
        public Dictionary<Locale, string> LocalizedTextByLocale { get; set; } = new Dictionary<Locale, string>();

        [JsonIgnore]
        public string English
        {
            set => SetLocalizedText(value, Locale.English);
            get => GetLocalizedText(Locale.English);
        }

        [JsonIgnore] public string Localized => GetLocalizedText(GameService.Overlay.UserLocale.Value);

        private string GetLocalizedText(Locale locale)
        {
            var localizationExists = LocalizedTextByLocale.ContainsKey(locale) && !string.IsNullOrWhiteSpace(LocalizedTextByLocale[locale]);
            if (localizationExists)
                return LocalizedTextByLocale[locale];

            var englishLocalizationCanBeUsedAsFallback = LocalizedTextByLocale.ContainsKey(Locale.English) && !string.IsNullOrWhiteSpace(LocalizedTextByLocale[Locale.English]);
            return englishLocalizationCanBeUsedAsFallback
                ? LocalizedTextByLocale[Locale.English]
                : string.Empty;
        }

        public void SetLocalizedText(string localizedText, Locale locale)
        {
            LocalizedTextByLocale[locale] = localizedText;
        }
    }
}