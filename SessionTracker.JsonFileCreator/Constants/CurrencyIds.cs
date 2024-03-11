using SessionTracker.Constants;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SessionTracker.JsonFileCreator.Constants
{
    public class CurrencyIds
    {
        private const int KARMA = 2;

        public static readonly IReadOnlyList<int> Pvp = new List<int>
        {
            30, // PVP_LEAGUE_TICKET
            33, // PVP_ASCENDED_SHARDS_OF_GLORY
            46, // PVP_TOURNAMENT_VOUCHER
            CurrencyId.COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly IReadOnlyList<int> Wvw = new List<int>
        {
            26, // WVW_SKIRMISH_CLAIM_TICKET
            15, // WVW_BADGE_OF_HONOR
            KARMA,
            CurrencyId.COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly IReadOnlyList<int> Fractal = new List<int>
        {
            7,  // FRACTAL_RELIC
            24, // PRISTINE_FRACTAL_RELIC
            59, // UNSTABLE_FRACTAL_ESSENCE
            CurrencyId.COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly IReadOnlyList<int> Strike = new List<int>
        {
            53, // GREEN_PROPHET_SHARD
            54, // BLUE_PROPHET_CRYSTAL
            57, // PROPHET_SHARD
            CurrencyId.COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly IReadOnlyList<int> Raid = new List<int>
        {
            28, // MAGNETITE_SHARD
            70, // LEGENDARY_INSIGHT
            CurrencyId.COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly IReadOnlyList<int> OpenWorld = new List<int>
        {
            KARMA,
            23, // SPIRIT_SHARD
            32, // UNBOUND_MAGIC
            45, // VOLATILE_MAGIC
            CurrencyId.COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly IReadOnlyList<int> ObsoleteIds = new List<int>
        {
            // Those are currencyIds and NOT itemIds. No idea why currencies both have a currency and an item id.
            74, // "Astral Claim" without name and without description, was replaced by other Astral Claim (id 74): https://api.guildwars2.com/v2/currencies?ids=63,74
            56, // "Red Prophet Crystal", replaced by Blue Prophet Crystal
            52, // "Red Prophet Shard", replaced by BLue Prophet Shard
            39, // "Gaeting Crystal", replaced by Magnetite Shards
            5,  // "Ascalonian Tear", replaced by Tales Of Dungeon Devling
            9,  // "Seal of Beetletun", replaced by Tales Of Dungeon Devling
            11, // "Deadly Bloom", replaced by Tales Of Dungeon Devling
            10, // "Manifesto of the Moletariate", replaced by Tales Of Dungeon Devling
            13, // "Flame Legion Charr Carving", replaced by Tales Of Dungeon Devling
            12, // "Symbol of Koda", replaced by Tales Of Dungeon Devling
            14, // "Knowledge Crystal", replaced by Tales Of Dungeon Devling
            6,  // "Shard of Zhaitan" replaced by Tales Of Dungeon Devling
        }.AsReadOnly();
    }
}