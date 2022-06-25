using Blish_HUD;
using Gw2Sharp.Models;
using SessionTracker.Settings;

namespace SessionTracker.Services
{
    public class VisibilityService
    {
        public static bool WindowIsVisible(SettingService settingsService, MapType mapType)
        {
            if (settingsService.UiIsVisible.Value == false)
                return false;

            if (settingsService.WindowIsVisibleEverywhere.Value)
                return true;

            if (GameService.GameIntegration.Gw2Instance.IsInGame == false)
                return false;

            if (GameService.Gw2Mumble.UI.IsMapOpen)
                return false;

            if (settingsService.WindowIsOnlyVisibleInWvwSetting.Value) 
                return IsWorldVsWorldMap(mapType);
            
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
    }
}