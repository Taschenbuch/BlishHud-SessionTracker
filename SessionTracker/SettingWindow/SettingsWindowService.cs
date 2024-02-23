using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.OtherServices;

namespace SessionTracker.SettingsWindow
{
    public class SettingsWindowService : IDisposable
    {
        public SettingsWindowService(Services services)
        {
            _settingsWindow = new TabbedWindow2(
                services.TextureService.SettingsWindowBackgroundTexture,
                new Rectangle(40, 30, 720, 700),
                new Rectangle(80, 30, 680, 630))
            {
                Title = "Session Tracker",
                Emblem = services.TextureService.SettingsWindowEmblemTexture,
                Location = new Point(300, 300),
                SavesPosition = true,
                Id = "Ecksofa.SessionTracker: settings window",
                Parent = GameService.Graphics.SpriteScreen,
            };

            _statsTab = new Tab(services.TextureService.StatsTabTexture, () => new StatsSettingsTabView(services), "Tracked Stats");
            _settingsWindow.Tabs.Add(_statsTab);
            _settingsWindow.Tabs.Add(new Tab(services.TextureService.GeneralTabTexture, () => new GeneralSettingsTabView(services.SettingService), "General"));
            _settingsWindow.Tabs.Add(new Tab(services.TextureService.VisibilityTabTexture, () => new VisibilitySettingsTabView(services.SettingService), "Window Visibility"));
#if DEBUG
            _settingsWindow.Tabs.Add(new Tab(services.TextureService.DebugTabTexture, () => new DebugSettingsTabView(services), "Debug"));
#endif
        }

        public void Dispose()
        {
            _settingsWindow?.Dispose();
        }

        public void ShowWindow()
        {
            _settingsWindow.Show();
        }

        public void ShowWindowAndSelectStatsTab()
        {
            _settingsWindow.Show();
            _settingsWindow.SelectedTab = _statsTab;
        }

        private readonly TabbedWindow2 _settingsWindow;
        private readonly Tab _statsTab;
    }
}