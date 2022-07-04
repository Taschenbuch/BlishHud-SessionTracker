using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Gw2Sharp;
using Gw2Sharp.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SessionTracker.Models;

namespace SessionTracker.JsonFileCreator
{
    internal class Program
    {
        static async Task Main()
        {
            var currencyEntries = await CreateCurrencyEntries();

            var model = new Model();
            model.Entries.AddRange(_manuallyCreatedEntries);
            model.Entries.AddRange(currencyEntries);

            var json = JsonConvert.SerializeObject(model, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(@"C:\gw2\session\model.json", json);
        }

        private static async Task<List<Entry>> CreateCurrencyEntries()
        {
            var entries = new List<Entry>();

            using (var client = new Gw2Client(new Connection(Locale.English)))
            {
                var currencies = await client.WebApi.V2.Currencies.AllAsync();

                foreach (var currency in currencies)
                {
                    var entry = new Entry
                    {
                        Id                   = $"currency{currency.Id}",
                        LabelText            = currency.Name,
                        LabelTooltip         = currency.Description,
                        PlaceholderInTooltip = currency.Name,
                        CurrencyId           = currency.Id,
                        IconUrl              = currency.Icon.Url.ToString(),
                        IsVisible            = false
                    };

                    entries.Add(entry);
                }
            }

            return entries;
        }

        private static readonly List<Entry> _manuallyCreatedEntries = new List<Entry>()
        {
            new Entry { Id = EntryId.DEATHS, LabelText              = "Deaths", LabelTooltip              = "Deaths", PlaceholderInTooltip                     = "deaths", IconFileName          = "death.png" },
            new Entry { Id = EntryId.PVP_KILLS, LabelText           = "PvP kills", LabelTooltip           = "PvP kills", PlaceholderInTooltip                  = "kills", AchievementId          = 239, IconFileName = "kill.png" },
            new Entry { Id = EntryId.PVP_KDR, LabelText             = "PvP k/d", LabelTooltip             = "PvP kills/deaths ratio", PlaceholderInTooltip     = "kills/death", IconFileName     = "kdr.png" },
            new Entry { Id = EntryId.PVP_TOTAL_WINS, LabelText      = "PvP total wins", LabelTooltip      = "PvP total wins", PlaceholderInTooltip             = "total wins", IconFileName      = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_TOTAL_LOSSES, LabelText    = "PvP total losses", LabelTooltip    = "PvP total losses", PlaceholderInTooltip           = "total losses", IconFileName    = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANKED_WINS, LabelText     = "PvP ranked wins", LabelTooltip     = "PvP ranked wins", PlaceholderInTooltip            = "ranked wins", IconFileName     = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_RANKED_LOSSES, LabelText   = "PvP ranked losses", LabelTooltip   = "PvP ranked losses", PlaceholderInTooltip          = "ranked losses", IconFileName   = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_WINS, LabelText   = "PvP unranked wins", LabelTooltip   = "PvP unranked wins", PlaceholderInTooltip          = "unranked wins", IconFileName   = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_UNRANKED_LOSSES, LabelText = "PvP unranked losses", LabelTooltip = "PvP unranked losses", PlaceholderInTooltip        = "unranked losses", IconFileName = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_WINS, LabelText     = "PvP custom wins", LabelTooltip     = "PvP custom wins", PlaceholderInTooltip            = "custom wins", IconFileName     = "pvpWins.png" },
            new Entry { Id = EntryId.PVP_CUSTOM_LOSSES, LabelText   = "PvP custom losses", LabelTooltip   = "PvP custom losses", PlaceholderInTooltip          = "custom losses", IconFileName   = "pvpLosses.png" },
            new Entry { Id = EntryId.PVP_RANK, LabelText            = "PvP rank", LabelTooltip            = "PvP rank", PlaceholderInTooltip                   = "ranks", IconFileName           = "pvpRank.png" },
            new Entry { Id = EntryId.PVP_RANKING_POINTS, LabelText  = "PvP ranking points", LabelTooltip  = "PvP ranking points", PlaceholderInTooltip         = "ranking points", IconFileName  = "pvpRankingPoints.png" },
            new Entry { Id = EntryId.WVW_KILLS, LabelText           = "WvW kills", LabelTooltip           = "WvW kills", PlaceholderInTooltip                  = "kills", AchievementId          = 283, IconFileName = "kill.png" },
            new Entry { Id = EntryId.WVW_KDR, LabelText             = "WvW k/d", LabelTooltip             = "WvW kills/deaths ratio", PlaceholderInTooltip     = "kills/death", IconFileName     = "kdr.png" },
            new Entry { Id = EntryId.WVW_RANK, LabelText            = "WvW rank", LabelTooltip            = "WvW rank", PlaceholderInTooltip                   = "ranks", IconFileName           = "wvwRank.png" },
            new Entry { Id = "WvwSupplySpentOnRepairs", LabelText   = "Supply (repair)", LabelTooltip     = "Supply spent on repairs", PlaceholderInTooltip    = "supplies", AchievementId       = 306, IconFileName = "supplySpend.png" },
            new Entry { Id = "WvwDolyaksKilled", LabelText          = "Dolyaks killed", LabelTooltip      = "Dolyaks killed", PlaceholderInTooltip             = "dolyaks", AchievementId        = 288, IconFileName = "dolyak.png" },
            new Entry { Id = "WvwDolyakEscorted", LabelText         = "Dolyaks escorted", LabelTooltip    = "Dolyaks escorted", PlaceholderInTooltip           = "dolyaks", AchievementId        = 285, IconFileName = "dolyakDefended.png" },
            new Entry { Id = "WvwCampsCaptured", LabelText          = "Camps captured", LabelTooltip      = "Supply camp captured", PlaceholderInTooltip       = "camps", AchievementId          = 291, IconFileName = "camp.png" },
            new Entry { Id = "WvwCampsDefended", LabelText          = "Camps defended", LabelTooltip      = "Supply camps defended", PlaceholderInTooltip      = "camps", AchievementId          = 310, IconFileName = "campDefended.png" },
            new Entry { Id = "WvwTowersCaptured", LabelText         = "Towers captured", LabelTooltip     = "Towers captured", PlaceholderInTooltip            = "towers", AchievementId         = 297, IconFileName = "tower.png" },
            new Entry { Id = "WvwTowersDefended", LabelText         = "Towers defended", LabelTooltip     = "Towers defended", PlaceholderInTooltip            = "towers", AchievementId         = 322, IconFileName = "towerDefended.png" },
            new Entry { Id = "WvwKeepsCaptured", LabelText          = "Keeps captured", LabelTooltip      = "Keeps captured", PlaceholderInTooltip             = "keeps", AchievementId          = 300, IconFileName = "keep.png" },
            new Entry { Id = "WvwKeepsDefended", LabelText          = "Keeps defended", LabelTooltip      = "Keeps defended", PlaceholderInTooltip             = "keeps", AchievementId          = 316, IconFileName = "keepDefended.png" },
            new Entry { Id = "WvwCastlesCaptured", LabelText        = "Castles captured", LabelTooltip    = "Stonemist castles captured", PlaceholderInTooltip = "castles", AchievementId        = 294, IconFileName = "castle.png" },
            new Entry { Id = "WvwCastlesDefended", LabelText        = "Castles defended", LabelTooltip    = "Stonemist castles defended", PlaceholderInTooltip = "castles", AchievementId        = 313, IconFileName = "castleDefended.png" },
            new Entry { Id = "WvwObjectivesCaptured", LabelText     = "Objectives captured", LabelTooltip = "Objectives captured", PlaceholderInTooltip        = "objectives", AchievementId     = 303, IconFileName = "objective.png" },
            new Entry { Id = "WvwObjectivesDefended", LabelText     = "Objectives defended", LabelTooltip = "Objectives defended", PlaceholderInTooltip        = "objectives", AchievementId     = 319, IconFileName = "objectiveDefended.png" },
        };
    }
}