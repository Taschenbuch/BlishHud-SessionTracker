using Blish_HUD;
using Gw2Sharp.Models;
using SessionTracker.Controls;
using SessionTracker.SettingEntries;
using System;

namespace SessionTracker.Services
{
    public class VisibilityService: IDisposable
    {
        public VisibilityService(StatsContainer statsContainer, SettingService settingService)
        {
            _statsContainer = statsContainer;
            _settingService = settingService;

            GameService.Gw2Mumble.UI.IsMapOpenChanged                      += OnWindowVisibilityUpdateRequired;
            GameService.GameIntegration.Gw2Instance.IsInGameChanged        += OnWindowVisibilityUpdateRequired;
            GameService.Gw2Mumble.CurrentMap.MapChanged                    += OnWindowVisibilityUpdateRequired;
            settingService.UiIsVisibleSetting.SettingChanged               += OnWindowVisibilityUpdateRequired;
            settingService.WindowIsVisibleOnWorldMapSetting.SettingChanged += OnWindowVisibilityUpdateRequired;
            settingService.WindowIsVisibleInWvwSetting.SettingChanged      += OnWindowVisibilityUpdateRequired;
            settingService.WindowIsVisibleInSpvpSetting.SettingChanged     += OnWindowVisibilityUpdateRequired;
            settingService.WindowIsVisibleOutsideOfWvwAndSpvpSetting.SettingChanged += OnWindowVisibilityUpdateRequired;
            settingService.WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting.SettingChanged += OnWindowVisibilityUpdateRequired;

            UpdateWindowVisibility();
        }

        public void Dispose()
        {
            GameService.Gw2Mumble.UI.IsMapOpenChanged                       -= OnWindowVisibilityUpdateRequired;
            GameService.GameIntegration.Gw2Instance.IsInGameChanged         -= OnWindowVisibilityUpdateRequired;
            GameService.Gw2Mumble.CurrentMap.MapChanged                     -= OnWindowVisibilityUpdateRequired;
            _settingService.UiIsVisibleSetting.SettingChanged               -= OnWindowVisibilityUpdateRequired;
            _settingService.WindowIsVisibleOnWorldMapSetting.SettingChanged -= OnWindowVisibilityUpdateRequired;
            _settingService.WindowIsVisibleInWvwSetting.SettingChanged      -= OnWindowVisibilityUpdateRequired;
            _settingService.WindowIsVisibleInSpvpSetting.SettingChanged     -= OnWindowVisibilityUpdateRequired;
            _settingService.WindowIsVisibleOutsideOfWvwAndSpvpSetting.SettingChanged -= OnWindowVisibilityUpdateRequired;
            _settingService.WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting.SettingChanged -= OnWindowVisibilityUpdateRequired;
        }

        public void UpdateWindowVisibility()
        {
            _statsContainer.Visible = IsWindowVisible(_settingService);
        }

        private void OnWindowVisibilityUpdateRequired(object sender, EventArgs e)
        {
            UpdateWindowVisibility();
        }

        private static bool IsWindowVisible(SettingService settingService)
        {
            var show                                   = settingService.UiIsVisibleSetting.Value;
            var showOnMap                              = settingService.WindowIsVisibleOnWorldMapSetting.Value;
            var showOnCharSelectLoadingScreenCutScenes = settingService.WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting.Value;
            var showOutsideOfWvwAndSpvp                = settingService.WindowIsVisibleOutsideOfWvwAndSpvpSetting.Value;
            var showInWvw                              = settingService.WindowIsVisibleInWvwSetting.Value;
            var showInSpvp                             = settingService.WindowIsVisibleInSpvpSetting.Value;

            var mapType               = GameService.Gw2Mumble.CurrentMap.Type;
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
        private readonly StatsContainer _statsContainer;
        private readonly SettingService _settingService;
    }
}