using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework.Input;

namespace SessionTracker.Settings
{
    public class SettingService // do not mix up with blish setting"S"service 
    {
        public SettingService(SettingCollection settings)
        {
            BackgroundOpacitySetting = settings.DefineSetting(
                "window background opacity",
                0,
                () => "window background opacity",
                () => "Change window background opacity");

            BackgroundOpacitySetting.SetRange(0, 255);

            FontSizeIndexSetting = FontService.CreateFontSizeIndexSetting(settings);

            TitleLabelColorSetting = settings.DefineSetting(
                "title label color",
                ColorType.White,
                () => "label color",
                () => "Change color of the stats label. e.g. for 'PvP kills'.");

            ValueLabelColorSetting = settings.DefineSetting(
                "value label color",
                ColorType.LightGreen,
                () => "value color",
                () => "Change color of the stats value. e.g. for '0 | 10'");

            SessionValuesAreVisibleSetting = settings.DefineSetting(
                "show session values",
                true,
                () => "SESSION values",
                () => "Show values of the current session. " +
                      "Total or session values can not be both hidden.");

            TotalValuesAreVisibleSetting = settings.DefineSetting(
                "show total values",
                false,
                () => "TOTAL values",
                () => "Show total values for the whole account. " +
                      "Total or session values can not be both hidden.");

            WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting = settings.DefineSetting(
                "show window on cutScenes and characterSelection and loadingScreens",
                true,
                () => "on character selection / loading screens / cut scenes",
                () => "show window on character selection, loading screens and cut scences. " +
                      "It will not show values on character selection screen right after starting Guild Wars 2 because " +
                      "at that point Blish does not know " +
                      "which API key it should use. You have to log into a character first.");

            WindowIsVisibleOnWorldMapSetting = settings.DefineSetting(
                "show window on world map",
                true,
                () => "on world map",
                () => "show window on world map.");

            WindowIsVisibleOutsideOfWvwAndSpvpSetting = settings.DefineSetting(
                "show window outside of wvw and spvp",
                true,
                () => "outside of WvW and sPvP",
                () => "show window outside of wvw and spvp. e.g. on open world maps");

            WindowIsVisibleInSpvpSetting = settings.DefineSetting(
                "show window in spvp",
                true,
                () => "in sPvP",
                () => "show window on structured PvP maps.");

            WindowIsVisibleInWvwSetting = settings.DefineSetting(
                "show window in wvw",
                true,
                () => "in WvW",
                () => "show window on world vs world maps.");

            DragWindowWithMouseIsEnabledSetting = settings.DefineSetting(
                "dragging window is allowed",
                true,
                () => "drag with mouse",
                () => "Allow dragging the window by moving the mouse when left mouse button is pressed inside window");

            CornerIconIsVisibleSetting = settings.DefineSetting(
                "cornerIcon is visible",
                false,
                () => "corner icon",
                () => "Show an icon at the top left of GW2 next to other menu icons. " +
                      "Icon can be clicked to show/hide the stats UI.");

            LabelTypeSetting = settings.DefineSetting(
                "label type",
                LabelType.IconAndText,
                () => "label type",
                () => "The label in front of the value in the UI can be text or icon");

            UiVisibilityKeyBindingSetting = settings.DefineSetting(
                "ui visibility key binding",
                new KeyBinding(Keys.None),
                () => "show/hide UI",
                () => "Double-click to change it. Will show or hide the session-tracker UI. " +
                      "Whether UI is really shown depends on other visibility settings. " +
                      "e.g. when 'on world map' is unchecked, using the key binding will still not show the UI on the world map.");

            UiHeightIsFixedSetting = settings.DefineSetting(
                "ui height is fixed",
                false,
                () => "fixed height",
                () => "CHECKED: height is fixed and can be adjusted with the ui height slider.\n" +
                      "Stats can be scrolled in the UI via mouse wheel or by dragging the scrollbar. Dragging the scrollbar only works when 'drag with mouse' setting is disabled.\n" +
                      "UNCHECKED: height adjusts automatically to the number of stats shown.\n" +
                      "BUG: There is a not fixable bug, that the scrollbar is visible when the mouse is not " +
                      "over the UI after adding/removing stats or loading the module. Just move the mouse one time over the UI to hide the scrollbar again.");

            UiHeightSetting = settings.DefineSetting(
                "ui height",
                200,
                () => "height",
                () => "");

            UiHeightSetting.SetRange(5, 2000);

            var internalSettings = settings.AddSubCollection("internal settings (not visible in UI)");
            SettingsVersionSetting             = internalSettings.DefineSetting("settings version", 1);
            XMainWindowRelativeLocationSetting = internalSettings.DefineSetting("window relative location x", 0.5f);
            YMainWindowRelativeLocationSetting = internalSettings.DefineSetting("window relative location y", 0.5f);
            UiIsVisibleSetting                 = internalSettings.DefineSetting("ui is visible", true);
        }

        public SettingEntry<int> UiHeightSetting { get; }
        public SettingEntry<bool> UiHeightIsFixedSetting { get; }
        public SettingEntry<ColorType> ValueLabelColorSetting { get; }
        public SettingEntry<ColorType> TitleLabelColorSetting { get; }
        public SettingEntry<float> XMainWindowRelativeLocationSetting { get; }
        public SettingEntry<float> YMainWindowRelativeLocationSetting { get; }
        public SettingEntry<bool> UiIsVisibleSetting { get; }
        public SettingEntry<int> BackgroundOpacitySetting { get; }
        public SettingEntry<int> FontSizeIndexSetting { get; }
        public SettingEntry<bool> SessionValuesAreVisibleSetting { get; }
        public SettingEntry<bool> TotalValuesAreVisibleSetting { get; }
        public SettingEntry<bool> WindowIsVisibleOnCharacterSelectAndLoadingScreensAndCutScenesSetting { get; }
        public SettingEntry<bool> WindowIsVisibleOnWorldMapSetting { get; }
        public SettingEntry<bool> WindowIsVisibleOutsideOfWvwAndSpvpSetting { get; }
        public SettingEntry<bool> WindowIsVisibleInSpvpSetting { get; }
        public SettingEntry<bool> WindowIsVisibleInWvwSetting { get; }
        public SettingEntry<bool> DragWindowWithMouseIsEnabledSetting { get; }
        public SettingEntry<bool> CornerIconIsVisibleSetting { get; }
        public SettingEntry<KeyBinding> UiVisibilityKeyBindingSetting { get; }
        public SettingEntry<LabelType> LabelTypeSetting { get; }
        public SettingEntry<int> SettingsVersionSetting { get; }
    }
}