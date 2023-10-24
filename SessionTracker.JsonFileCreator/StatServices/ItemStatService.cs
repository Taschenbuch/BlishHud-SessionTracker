using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Gw2Sharp.WebApi.V2.Models;
using SessionTracker.Constants;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator.StatServices
{
    public class ItemStatService
    {
        public static async Task<List<Stat>> CreateItemStats()
        {
            var stats = await CreateStats();
            return await AddLocalizedTexts(stats);
        }

        private static async Task<List<Stat>> CreateStats()
        {
            var stats = new List<Stat>();
            using var client = new Gw2Client(new Connection(Locale.English));
            var items = await client.WebApi.V2.Items.ManyAsync(ITEM_IDS);

            foreach (var item in items)
            {
                var stat = new Stat
                {
                    Id          = $"item{item.Id}",
                    ApiId       = item.Id,
                    ApiIdType   = ApiIdType.Item,
                    IconAssetId = AssetService.GetIconAssetIdFromIconUrl(item.Icon.Url.ToString()),
                    IsVisible   = false
                };

                stats.Add(stat);
            }

            return stats;
        }

        private static async Task<List<Stat>> AddLocalizedTexts(List<Stat> stats)
        {
            var locales = new List<Locale>() { Locale.English, Locale.French, Locale.German, Locale.Spanish, Locale.Chinese, Locale.Korean };

            foreach (var local in locales)
            {
                using var client = new Gw2Client(new Connection(local));
                var items = await client.WebApi.V2.Items.ManyAsync(ITEM_IDS);

                foreach (var item in items)
                    UpdateTexts(item, stats, local);
            }

            return stats;
        }

        private static void UpdateTexts(Item item, List<Stat> stats, Locale local)
        {
            var statForItemExists = stats.Any(e => e.ApiId == item.Id);
            if (statForItemExists)
            {
                var matchingStat = stats.Single(e => e.ApiId == item.Id);
                matchingStat.Name.SetLocalizedText(item.Name, local);
                matchingStat.Description.SetLocalizedText(item.Description, local);
            }
        }

        public static readonly List<int> ITEM_IDS = new List<int>
        {
            ItemIds.MEMORY_OF_BATTLE,
            ItemIds.HEAVY_LOOT_BAG,
            ItemIds.TRICK_OR_TREAT_BAG,
            ItemIds.PIECE_OF_CANDY_CORN, // remove when material storage is added!
            ItemIds.MYSTIC_COIN, // remove when material storage is added!
            ItemIds.FRACTAL_ENCRYPTION, // remove when material storage is added!
        };
    }
}