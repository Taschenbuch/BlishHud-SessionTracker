using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator
{
    public class ItemService
    {
        public static async Task<List<Entry>> CreateItemStats()
        {
            var entries = await CreateStats();
            return await AddLocalizedTexts(entries);
        }

        private static async Task<List<Entry>> CreateStats()
        {
            var entries = new List<Entry>();

            using (var client = new Gw2Client(new Connection(Locale.English)))
            {
                var items = await client.WebApi.V2.Items.ManyAsync(ITEM_IDS);

                foreach (var item in items)
                {
                    var entry = new Entry
                    {
                        Id        = $"item{item.Id}",
                        ItemId    = item.Id,
                        IconUrl   = item.Icon.Url.ToString(),
                        IsVisible = false
                    };

                    entries.Add(entry);
                }
            }

            return entries;
        }

        private static async Task<List<Entry>> AddLocalizedTexts(List<Entry> entries)
        {
            var locales = new List<Locale>() { Locale.English, Locale.French, Locale.German, Locale.Spanish, Locale.Chinese, Locale.Korean };

            foreach (var local in locales)
            {
                using (var client = new Gw2Client(new Connection(local)))
                {
                    var items = await client.WebApi.V2.Items.ManyAsync(ITEM_IDS);

                    foreach (var item in items)
                        UpdateTexts(item, entries, local);
                }
            }

            return entries;
        }

        private static void UpdateTexts(Item item, List<Entry> entries, Locale local)
        {
            var entryForItemExists = entries.Any(e => e.ItemId == item.Id);
            if (entryForItemExists)
            {
                var matchingEntry = entries.Single(e => e.ItemId == item.Id);
                matchingEntry.LabelText.SetLocalizedText(item.Name, local);
                matchingEntry.LabelTooltip.SetLocalizedText(item.Description, local);
            }
        }

        public static readonly List<int> ITEM_IDS = new List<int>
        {
            ItemIds.MEMORY_OF_BATTLE,
            ItemIds.HEAVY_LOOT_BAG,
            83103, // "Eye of Kormir" // todo weg
        };
    }
}