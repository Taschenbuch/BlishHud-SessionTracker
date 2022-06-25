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

        public static LocationContainer CreateAdjustableChildLocationContainer(FlowPanel parent)
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
                OuterControlPadding = new Vector2(10, 20),
                ControlPadding      = new Vector2(0, 10),
                Size                = new Point(690, 0),
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
                OuterControlPadding = new Vector2(10, 10),
                ShowBorder          = true,
                Width               = 660,
                HeightSizingMode    = SizingMode.AutoSize,
                Parent              = parent
            };
        }

        public static void CreateSetting(Container parent, int width, SettingEntry settingEntry)
        {
            var viewContainer = new ViewContainer { Parent = parent };
            viewContainer.Show(SettingView.FromType(settingEntry, width));
        }
    }
}