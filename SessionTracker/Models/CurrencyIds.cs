using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SessionTracker.Models
{
    public class CurrencyIds
    {
        public static readonly ReadOnlyCollection<int> Pvp = new List<int>
        {
            30, // PvpLeagueTicket
            33, // PvpAscendedShardsOfGlory
            46, // PvpTournamentVoucher
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Wvw = new List<int>
        {
            26, // WvW Skirmish Claim Ticket
            15, // Badge of Honor
            2,  // Karma
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Fractal = new List<int>
        {
            7, // Fractal Relic 
            24, // Pristine Fractal Relic
            59, // Unstable Fractal Essence
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Strike = new List<int>
        {
            53, // Green Prophet Shard
            54, // Blue Prophet Crystal
            57, // Blue Prophet Shard
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Raid = new List<int>
        {
            28, // Magnetite Shard 
            70, // Legendary Insight 
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> OpenWorld = new List<int>
        {
            2, // Karma
            23, // Spirit Shard
            32, // Unbound Magic
            45, // Volatile Magic
        }.AsReadOnly();
    }
}