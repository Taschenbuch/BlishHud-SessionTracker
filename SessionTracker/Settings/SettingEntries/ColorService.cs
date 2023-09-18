using Microsoft.Xna.Framework;

namespace SessionTracker.Settings.SettingEntries
{
    public class ColorService
    {
        public static Color CreateBackgroundColor(SettingService settingService)
        {
            return settingService.BackgroundColorSetting.Value.GetColor() * (settingService.BackgroundOpacitySetting.Value / 255f);
        }
    }
}
