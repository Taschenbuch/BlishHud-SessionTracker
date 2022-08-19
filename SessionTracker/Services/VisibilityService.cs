using Blish_HUD;
using Gw2Sharp.Models;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Services
{
    public class VisibilityService
    {
        public static bool WindowIsVisible(SettingService settingsService)
        {
            var show                                   = settingsService.UiIsVisibleSetting.Value;
            var showOnMap                              = settingsService.WindowIsVisibleOnWorldMapSetting.Value;
            var showOnCharSelectLoadingScreenCutScenes = settingsService.WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting.Value;
            var showOutsideOfWvwAndSpvp                = settingsService.WindowIsVisibleOutsideOfWvwAndSpvpSetting.Value;
            var showInWvw                              = settingsService.WindowIsVisibleInWvwSetting.Value;
            var showInSpvp                             = settingsService.WindowIsVisibleInSpvpSetting.Value;

            var mapType = GameService.Gw2Mumble.CurrentMap.Type;

            var mapIsClosed           = GameService.Gw2Mumble.UI.IsMapOpen == false;
            var isInGame              = GameService.GameIntegration.Gw2Instance.IsInGame;
            var isWvwMap              = IsWorldVsWorldMap(mapType);
            var isSpvpMap             = IsSpvpMap(mapType, GameService.Gw2Mumble.CurrentMap.Id);
            var isOutsideOfWvwAndSpvp = IsOutsideOfWvwAndSpvp(mapType, isSpvpMap, isWvwMap);

            return show
                   && (showOnMap || mapIsClosed)
                   && (showOnCharSelectLoadingScreenCutScenes || isInGame)
                   && (showOutsideOfWvwAndSpvp || isWvwMap || isSpvpMap || isInGame == false)
                   && (showInWvw || isOutsideOfWvwAndSpvp || isSpvpMap || isInGame == false)
                   && (showInSpvp || isOutsideOfWvwAndSpvp || isWvwMap || isInGame == false);
        }

        private static bool IsOutsideOfWvwAndSpvp(MapType mapType, bool isSpvpMap, bool isWvwMap)
        {
            if (isWvwMap)
                return false;

            if (isSpvpMap)
                return false;

            if (mapType == MapType.Redirect)
                return false;

            return true;
        }

        private static bool IsWorldVsWorldMap(MapType mapType)
        {
            switch (mapType)
            {
                case MapType.Gvg:
                case MapType.EternalBattlegrounds:
                case MapType.BlueBorderlands:
                case MapType.GreenBorderlands:
                case MapType.RedBorderlands:
                case MapType.ObsidianSanctum:
                case MapType.EdgeOfTheMists:
                case MapType.WvwLounge: // Armistice Bastion Lounge
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsSpvpMap(MapType mapType, int mapId)
        {
            if (mapId == PVP_LOBBY_MAP_ID)
                return true;

            switch (mapType)
            {
                case MapType.Pvp:
                case MapType.Tournament:
                case MapType.UserTournament:
                    return true;
                default:
                    return false;
            }
        }

        private const int PVP_LOBBY_MAP_ID = 350;
    }
}