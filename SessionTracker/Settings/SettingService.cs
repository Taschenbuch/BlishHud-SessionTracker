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
                () => "show SESSION values",
                () => "Show values of the current session. " +
                      "Total or session values can not be both hidden.");

            TotalValuesAreVisibleSetting = settings.DefineSetting(
                "show total values",
                false,
                () => "show TOTAL values",
                () => "Show total values for the whole account. " +
                      "Total or session values can not be both hidden.");

            WindowIsVisibleEverywhere = settings.DefineSetting(
                "show window everywhere",
                true,
                () => "show everywhere",
                () => "show window everywhere even on world map or character select screen. " +
                      "But it will not work on character select right after logging into Guild Wars 2 because Blish does not know " +
                      "which API key it should use at this point.");

            WindowIsOnlyVisibleInWvwSetting = settings.DefineSetting(
                "show window only in WvW",
                true,
                () => "show only in WvW",
                () => "show window only when you are on a world vs world map");

            DragWindowWithMouseIsEnabledSetting = settings.DefineSetting(
                "dragging window is allowed",
                true,
                () => "move window by dragging with mouse",
                () => "Allow dragging the window by moving the mouse when left mouse button is pressed inside window");

            CornerIconIsVisible = settings.DefineSetting(
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
                () => "Double-click to change it. Will show or hide the UI of this module.");

            var internalSettings = settings.AddSubCollection("internal settings (not visible in UI)");
            SettingsVersionSetting             = internalSettings.DefineSetting("settings version", 1);
            XMainWindowRelativeLocationSetting = internalSettings.DefineSetting("window relative location x", 0.5f);
            YMainWindowRelativeLocationSetting = internalSettings.DefineSetting("window relative location y", 0.5f);
            UiIsVisible                        = internalSettings.DefineSetting("ui is visible", true);
        }

        public SettingEntry<ColorType> ValueLabelColorSetting { get; }
        public SettingEntry<ColorType> TitleLabelColorSetting { get; }
        public SettingEntry<float> XMainWindowRelativeLocationSetting { get; }
        public SettingEntry<float> YMainWindowRelativeLocationSetting { get; }
        public SettingEntry<bool> UiIsVisible { get; }
        public SettingEntry<int> BackgroundOpacitySetting { get; }
        public SettingEntry<int> FontSizeIndexSetting { get; }
        public SettingEntry<bool> SessionValuesAreVisibleSetting { get; }
        public SettingEntry<bool> TotalValuesAreVisibleSetting { get; }
        public SettingEntry<bool> WindowIsVisibleEverywhere { get; }
        public SettingEntry<bool> WindowIsOnlyVisibleInWvwSetting { get; }
        public SettingEntry<bool> DragWindowWithMouseIsEnabledSetting { get; }
        public SettingEntry<bool> CornerIconIsVisible { get; }
        public SettingEntry<KeyBinding> UiVisibilityKeyBindingSetting { get; }
        public SettingEntry<LabelType> LabelTypeSetting { get; }
        public SettingEntry<int> SettingsVersionSetting { get; }
    }
}