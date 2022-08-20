using System.Collections.Generic;
using Blish_HUD;
using Gw2Sharp.WebApi;
using Newtonsoft.Json;

namespace SessionTracker.Models
{
    public class LocalizedText
    {
        public Dictionary<Locale, string> LocalizedTextByLocale { get; set; } = new Dictionary<Locale, string>();
        [JsonIgnore] public string English
        {
            set => SetLocalizedText(value, Locale.English);
            get => GetLocalizedText(Locale.English);
        }

        [JsonIgnore] public string Localized => GetLocalizedText(GameService.Overlay.UserLocale.Value);

        private string GetLocalizedText(Locale locale)
        {
            return LocalizedTextByLocale.ContainsKey(locale) 
                ? LocalizedTextByLocale[locale] 
                : LocalizedTextByLocale[Locale.English];
        }

        public void SetLocalizedText(string localizedText, Locale locale)
        {
            LocalizedTextByLocale[locale] = localizedText;
        }
    }
}