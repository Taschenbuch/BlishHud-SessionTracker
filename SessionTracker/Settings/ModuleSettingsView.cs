using System;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Microsoft.Xna.Framework;
using SessionTracker.Settings.Window;

namespace SessionTracker.Settings
{
    public class ModuleSettingsView : View
    {
        public ModuleSettingsView(SettingsWindowService settingsWindowService)
        {
            _settingsWindowService = settingsWindowService;
        }

        protected override void Build(Container buildPanel)
        {
            _settingsButton = new OpenSettingsButton(_settingsWindowService, buildPanel);

            var x = Math.Max(buildPanel.Width / 2 - _settingsButton.Width / 2, 20);
            var y = Math.Max(buildPanel.Height / 2 - _settingsButton.Height / 2, 20);
            _settingsButton.Location = new Point(x, y);
        }

        protected override void Unload()
        {
            _settingsButton?.Dispose();
        }

        private OpenSettingsButton _settingsButton;
        private readonly SettingsWindowService _settingsWindowService;
    }
}