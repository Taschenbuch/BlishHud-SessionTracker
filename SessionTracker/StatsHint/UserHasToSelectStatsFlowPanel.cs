using Blish_HUD.Controls;
using MonoGame.Extended.BitmapFonts;
using SessionTracker.Settings;
using SessionTracker.SettingsWindow;

namespace SessionTracker.StatsHint
{
    public class UserHasToSelectStatsFlowPanel : FlowPanel
    {
        public UserHasToSelectStatsFlowPanel(SettingsWindowService settingsWindowService)
        {
            FlowDirection = ControlFlowDirection.SingleTopToBottom;
            HeightSizingMode = SizingMode.AutoSize;
            WidthSizingMode = SizingMode.AutoSize;

            CreateUi(settingsWindowService);
        }

        protected override void DisposeControl()
        {
            _noStatsSelectedByUserHintLabel?.Dispose();
            _openSettingsButton?.Dispose();
            base.DisposeControl();
        }

        public void SetFont(BitmapFont font)
        {
            _noStatsSelectedByUserHintLabel.Font = font;
        }

        private void CreateUi(SettingsWindowService settingsWindowService)
        {
            var noStatsSelectedByUserHintText = "No stats selected! Select the stats you want to see in the session tracker module settings. :-)";
            _noStatsSelectedByUserHintLabel = new Label()
            {
                Text = noStatsSelectedByUserHintText,
                BasicTooltipText = noStatsSelectedByUserHintText,
                ShowShadow = true,
                AutoSizeHeight = true,
                WrapText = true,
                Width = 300,
                Parent = this
            };

            _openSettingsButton = new OpenSettingsButton(settingsWindowService, this);
        }

        private Label _noStatsSelectedByUserHintLabel;
        private OpenSettingsButton _openSettingsButton;
    }
}