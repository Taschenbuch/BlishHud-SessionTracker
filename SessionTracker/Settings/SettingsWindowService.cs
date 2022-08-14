using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Models;
using SessionTracker.Services;

namespace SessionTracker.Settings
{
    public class SettingsWindowService : IDisposable
    {
        public SettingsWindowService(Model model, SettingService settingService, TextureService textureService)
        {
            _settingsWindowView = new SettingsWindowView(model, settingService, textureService);

            _settingsWindow = new StandardWindow(
                textureService.WindowBackground,
                new Rectangle(40, 26, 720, 700),
                new Rectangle(40, 20, 700, 630))
            {
                Title = "Session Tracker",
                Emblem = textureService.CornerIconTexture,
                Location = new Point(300, 300),
                SavesPosition = true,
                Id = "SessionTracker settings window",
                Parent = GameService.Graphics.SpriteScreen,
            };
        }

        public void Dispose()
        {
            _settingsWindow?.Dispose();
        }

        public void ShowWindow()
        {
            if (_settingsWindow.CurrentView == null) // prevents memory leak
                _settingsWindow.Show(_settingsWindowView);
            else
                _settingsWindow.Show();
        }

        private readonly StandardWindow _settingsWindow;
        private readonly SettingsWindowView _settingsWindowView;
    }
}