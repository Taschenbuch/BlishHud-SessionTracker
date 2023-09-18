using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SessionTracker.Models.Constants
{
    public class CurrencyIds
    {
        public const int COIN_IN_COPPER = 1;
        public const int PVP_LEAGUE_TICKET = 30;
        public const int PVP_ASCENDED_SHARDS_OF_GLORY = 33;
        public const int PVP_TOURNAMENT_VOUCHER = 46;
        public const int WVW_SKIRMISH_CLAIM_TICKET = 26;
        public const int WVW_BADGE_OF_HONOR = 15;
        public const int KARMA = 2;
        public const int FRACTAL_RELIC = 7;
        public const int PRISTINE_FRACTAL_RELIC = 24;
        public const int UNSTABLE_FRACTAL_ESSENCE = 59;
        public const int GREEN_PROPHET_SHARD = 53;
        public const int BLUE_PROPHET_CRYSTAL = 54;
        public const int PROPHET_SHARD = 57;
        public const int MAGNETITE_SHARD = 28;
        public const int LEGENDARY_INSIGHT = 70;
        public const int SPIRIT_SHARD = 23;
        public const int UNBOUND_MAGIC = 32;
        public const int VOLATILE_MAGIC = 45;

        public static readonly ReadOnlyCollection<int> Pvp = new List<int>
        {
            PVP_LEAGUE_TICKET,
            PVP_ASCENDED_SHARDS_OF_GLORY,
            PVP_TOURNAMENT_VOUCHER,
            COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Wvw = new List<int>
        {
            WVW_SKIRMISH_CLAIM_TICKET,
            WVW_BADGE_OF_HONOR,
            KARMA,
            COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Fractal = new List<int>
        {
            FRACTAL_RELIC,
            PRISTINE_FRACTAL_RELIC,
            UNSTABLE_FRACTAL_ESSENCE,
            COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Strike = new List<int>
        {
            GREEN_PROPHET_SHARD,
            BLUE_PROPHET_CRYSTAL,
            PROPHET_SHARD,
            COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Raid = new List<int>
        {
            MAGNETITE_SHARD,
            LEGENDARY_INSIGHT,
            COIN_IN_COPPER,
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> OpenWorld = new List<int>
        {
            KARMA,
            SPIRIT_SHARD,
            UNBOUND_MAGIC,
            VOLATILE_MAGIC,
            COIN_IN_COPPER,
        }.AsReadOnly();
    }
}