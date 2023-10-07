using Blish_HUD.Input;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework.Input;
using SessionTracker.AutomaticReset;

namespace SessionTracker.SettingEntries
{
    public class SettingService // do not mix up with blish setting"S"service 
    {
        public SettingService(SettingCollection settings)
        {
            AutomaticSessionResetSetting = settings.DefineSetting(
                "automatic session reset",
                AutomaticSessionReset.OnModuleStart,
                () => "automatic session reset",
                () => "Change when all session values are automatically reset to 0");

            MinutesUntilResetAfterModuleShutdownSetting = settings.DefineSetting(
                "number of minutes until reset after module shutdown",
                30,
                () => "minutes until session values reset after module shutdown",
                () => "Change number of minutes the module will wait after module shutdown before resetting all session values to 0. " +
                      "This setting can be usefull to not lose session values due to PC / gw2 / blish crashes, " +
                      $"but still have automatic resets similar to 'On module start' option. Or if you prefer to play in short sessions spread throughout the day");

            BackgroundOpacitySetting = settings.DefineSetting(
                "window background opacity",
                125,
                () => "background opacity",
                () => "Change window background opacity");

            BackgroundOpacitySetting.SetRange(0, 255);

            FontSizeIndexSetting = FontService.CreateFontSizeIndexSetting(settings);

            StatTitlePaddingSetting = settings.DefineSetting(
                "stat title padding",
                2,
                () => "padding",
                () => "Change padding between icon, title and value of a stat");

            StatTitlePaddingSetting.SetRange(0, 5);

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

            BackgroundColorSetting = settings.DefineSetting(
                "window background color",
                ColorType.Black,
                () => "background color",
                () => "Change window background color");

            ValueDisplayFormatSetting = settings.DefineSetting(
               "value display format",
               ValueDisplayFormat.SessionValue,
               () => "value display format",
               () => "Show current session values and/or total values for the whole account.");

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
                () => ON_WORLD_MAP_SETTING_DISPLAY_NAME,
                () => "show window on world map.");

            WindowIsVisibleOutsideOfWvwAndSpvpSetting = settings.DefineSetting(
                "show UI outside of wvw and spvp",
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
                () => DRAG_WITH_MOUSE_LABEL_TEXT,
                () => "Allow dragging the window by moving the mouse when left mouse button is pressed inside window");

            WindowCanBeClickedThroughSetting = settings.DefineSetting(
                "window is not capturing mouse clicks",
                false,
                () => "mouse clickthrough (read tooltip)",
                () => "This allows clicking with the mouse through the window to interact with Guild Wars 2 behind the window. " +
                      "Mouse clicks while ALT key is held down, will disable this feature temporarily. E.g. for scrolling the stats window etc. " +
                     $"WARNING: Feature is disabled when '{DRAG_WITH_MOUSE_LABEL_TEXT}' is checked. Feature makes scrolling impossible when fixed height setting is set.");

            CornerIconIsVisibleSetting = settings.DefineSetting(
                "cornerIcon is visible",
                true,
                () => "menu icon",
                () => "Show a menu icon at the top left of GW2 next to other menu icons." +
                      "Icon can be clicked to show/hide the stats UI.");

            LabelTypeSetting = settings.DefineSetting(
                "label type",
                LabelType.IconAndText,
                () => "label type",
                () => "The label in front of the value in the UI can be text or icon");

            CoinDisplayFormatSetting = settings.DefineSetting(
                "coin display format",
                CoinDisplayFormat.XgXsXc,
                () => "gold format",
                () => "Display format of the gold/coin stat. Dropdown shows examples of how 123456 copper are displayed in the different formats.");

            UiVisibilityKeyBindingSetting = settings.DefineSetting(
                "ui visibility key binding",
                new KeyBinding(Keys.None),
                () => "show/hide window",
                () => "Double-click to change the key binding. Will show or hide the session-tracker window. " +
                      "Whether window is really shown depends on other visibility settings. " +
                      "e.g. when 'on world map' is unchecked, using the key binding will still not show the window on the world map.");

            UiHeightIsFixedSetting = settings.DefineSetting(
                "ui height is fixed",
                false,
                () => "fixed height",
                () => "CHECKED: height is fixed and can be adjusted with the window height slider.\n" +
                      "Stats can be scrolled in the window via mouse wheel or by dragging the scrollbar. Dragging the scrollbar only works when 'drag with mouse' setting is disabled.\n" +
                      "UNCHECKED: height adjusts automatically to the number of stats shown.\n" +
                      "BUG: There is a not fixable bug, that the scrollbar is visible when the mouse is not " +
                      "over the window after adding/removing stats or loading the module. Just move the mouse one time over the window to hide the scrollbar again.");

            UiHeightSetting = settings.DefineSetting(
                "ui height",
                200,
                () => "height",
                () => "window height when fixed height setting is checked.");

            UiHeightSetting.SetRange(5, 2000);

            ScrollbarFixDelay = settings.DefineSetting(
                "scrollbar fix delay",
                50,
                () => "scrollbar fix (read tooltip)",
                () => "The scrollbar is a bit buggy :(. It jumps to the top after reordering stats or pressing buttons that affect the stats list " +
                      "in the settings window. " +
                      "A fix for that is implemented. But this fix does not work reliable for everybody. " +
                      "If the scrollbar keeps jumping to the top, try moving the slider to the right until this issue does not happen anymore. " +
                      "You will still notice that the scrollbar jumps to the top, especially when the slider is far to the right. But it should jump back " +
                      "to the correct position after a very short time.");

            ScrollbarFixDelay.SetRange(50, 500);

            DebugApiIntervalEnabledSetting = settings.DefineSetting(
                "debug api interval enabled",
                false,
                () => "use debug api interval",
                () => "Use debug api interval instead of normal api interval.");

            DebugApiIntervalValueSetting = settings.DefineSetting(
                "debug api interval value",
                5 * 1000,
                () => "debug api interval",
                () => "Increases polling rate beyond api cache time limit.");

            DebugApiIntervalValueSetting.SetRange(1000, 20 * 1000);

            UiIsVisibleSetting = settings.DefineSetting(
                "ui is visible",
                true,
                () => "window visible (read tooltip)",
                () => $"Show or hide sessions tracker UI. Has the same effect as clicking the menu icon or using the key binding. " +
                      $"Whether the window is really shown depends on further settings like '{ON_WORLD_MAP_SETTING_DISPLAY_NAME}'.");

            HideStatsWithValueZeroSetting = settings.DefineSetting(
                "hide stats with value zero",
                false,
                () => "hide stats with value = 0",
                () => "Stats with a session value of 0 are hidden until the session value changes to a non-zero value. " +
                      "At the start of a session all values will be 0 so the whole window is hidden.");

            var internalSettings = settings.AddSubCollection("internal settings (not visible in UI)");
            SettingsVersionSetting = internalSettings.DefineSetting("settings version", 2);
            XMainWindowRelativeLocationSetting = internalSettings.DefineSetting("window relative location x", 0.2f);
            YMainWindowRelativeLocationSetting = internalSettings.DefineSetting("window relative location y", 0.2f);
            MigrateBlishSettingsService.MigrateSettings(settings, SettingsVersionSetting, ValueDisplayFormatSetting);
        }

        public SettingEntry<int> DebugApiIntervalValueSetting { get; }
        public SettingEntry<bool> HideStatsWithValueZeroSetting { get; }
        public SettingEntry<int> ScrollbarFixDelay { get; }
        public SettingEntry<CoinDisplayFormat> CoinDisplayFormatSetting { get; }
        public SettingEntry<bool> DebugApiIntervalEnabledSetting { get; }
        public SettingEntry<int> StatTitlePaddingSetting { get; }
        public SettingEntry<int> UiHeightSetting { get; }
        public SettingEntry<bool> UiHeightIsFixedSetting { get; }
        public SettingEntry<ColorType> ValueLabelColorSetting { get; }
        public SettingEntry<ColorType> TitleLabelColorSetting { get; }
        public SettingEntry<ColorType> BackgroundColorSetting { get; }
        public SettingEntry<float> XMainWindowRelativeLocationSetting { get; }
        public SettingEntry<float> YMainWindowRelativeLocationSetting { get; }
        public SettingEntry<bool> UiIsVisibleSetting { get; }
        public SettingEntry<AutomaticSessionReset> AutomaticSessionResetSetting { get; }
        public SettingEntry<int> MinutesUntilResetAfterModuleShutdownSetting { get; }
        public SettingEntry<int> BackgroundOpacitySetting { get; }
        public SettingEntry<int> FontSizeIndexSetting { get; }
        public SettingEntry<ValueDisplayFormat> ValueDisplayFormatSetting { get; }
        public SettingEntry<bool> WindowCanBeClickedThroughSetting { get; }
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

        private const string ON_WORLD_MAP_SETTING_DISPLAY_NAME = "on world map";
        private const string DRAG_WITH_MOUSE_LABEL_TEXT = "drag with mouse";
    }
}