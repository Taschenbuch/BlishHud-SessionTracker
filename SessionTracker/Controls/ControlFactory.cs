using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Blish_HUD.Settings.UI.Views;
using Microsoft.Xna.Framework;

namespace SessionTracker.Controls
{
    public class ControlFactory
    {
        public static Label CreateHintLabel(Container parent, string text)
        {
            return new Label
            {
                Text           = text,
                AutoSizeHeight = true,
                AutoSizeWidth  = true,
                Parent         = parent
            };
        }

        public static LocationContainer CreateAdjustableChildLocationContainer(Container parent)
        {
            return new LocationContainer()
            {
                WidthSizingMode  = SizingMode.AutoSize,
                HeightSizingMode = SizingMode.AutoSize,
                Parent           = parent
            };
        }

        public static FlowPanel CreateSettingsRootFlowPanel(Container parent)
        {
            return new FlowPanel
            {
                FlowDirection       = ControlFlowDirection.SingleTopToBottom,
                CanScroll           = true,
                OuterControlPadding = new Vector2(10, 0),
                ControlPadding      = new Vector2(0, 10),
                Width               = 910, // fixed width to not cutoff scrollbar
                HeightSizingMode    = SizingMode.Fill,
                Parent              = parent
            };
        }

        public static FlowPanel CreateSettingsGroupFlowPanel(string title, Container parent)
        {
            return new FlowPanel
            {
                Title               = title,
                FlowDirection       = ControlFlowDirection.SingleTopToBottom,
                OuterControlPadding = new Vector2(15, 10),
                ShowBorder          = true,
                Width               = parent.Width - 30,
                HeightSizingMode    = SizingMode.AutoSize,
                Parent              = parent
            };
        }

        public static ViewContainer CreateSetting(Container parent, SettingEntry settingEntry)
        {
            return CreateSetting(parent, parent.Width, settingEntry);
        }

        public static ViewContainer CreateSetting(Container parent, int width, SettingEntry settingEntry)
        {
            var viewContainer = new ViewContainer { Parent = parent };
            viewContainer.Show(SettingView.FromType(settingEntry, width));
            return viewContainer;
        }
    }
}