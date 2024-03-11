using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SessionTracker.JsonFileCreator.Constants
{
    public class ItemIds
    {
        private const int MYSTIC_COIN = 19976; // also part of material storage

        public static readonly ReadOnlyCollection<int> Fractal = new List<int>
        {
            MYSTIC_COIN,
            75919 // Fractal Encryption
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Raid = new List<int>
        {
            MYSTIC_COIN
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Wvw = new List<int>
        {
            8920,  // Heavy Loot Bag
            71581, // Memory of Battle // also part of material storage
            93075, // Emblem of the Avenger
            93146, // Emblem of the Conqueror
            87557, // Grandmaster Mark Shard
            84966, // Skirmish Chest 1
            96536, // Skirmish Chest 2 // probably EOD and POF versions of skirmish chests. I get a different total count for both.
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Misc = new List<int>
        {
            83008, // Piece of Rare Unidentified Gear (yellow)
            84731, // Piece of Unidentified Gear (green)
            85016, // Piece of Common Unidentified Gear (blue)
            45175, // Essence of Luck (fine)
            45176, // Essence of Luck (masterwork)
            45177, // Essence of Luck (rare)
            45178, // Essence of Luck (exotic)
            45179, // Essence of Luck (legendary)
        }.AsReadOnly();

        public static readonly ReadOnlyCollection<int> Festival = new List<int>
        {
            // lunar new year
            68617, // Coffer of the Dragon Ball Champion
            68645, // Little Lucky Envelope
            68646, // Divine Lucky Envelope
            68647, // Dragon Ball Champion's Divine Lucky Envelope
            94653, // Lucky Red Bag
            92659, // Token of the Celestial Champion
            94668, // Token of the Celestial Champion Fragment
            68618, // Token of the Dragon Ball Champion
            // super adventure box
            39752, // Bauble
            41886, // Bauble Bubble
            41824, // Continue Coin
            80890, // Crimson Assassin Token
            78062, // Fancy Furniture Coin
            // dragon bash
            43357, // Dragon Coffer
            // festival of the four winds
            // none: festival Token has 2 itemIds, but doesnt matter, because they can be consumed to become a currency
            // halloween
            36038, // Trick-or-Treat Bag
            // wintersday
            77604, // Wintersday Gift
        }.AsReadOnly();
    }
}