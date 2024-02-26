using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.OtherServices;
using SessionTracker.SelectStats;

namespace SessionTracker.SettingsWindow
{
    public class SettingsWindowService : IDisposable
    {
        public SettingsWindowService(Services services)
        {
            _settingsWindow = new TabbedWindow2(
                services.TextureService.SelectStatsWindowBackgroundTexture,
                new Rectangle(40, 30, 950, 950), 
                new Rectangle(80, 30, 910, 930))
            {
                Title = "Session Tracker",
                Emblem = services.TextureService.SettingsWindowEmblemTexture,
                Location = new Point(300, 300),
                SavesPosition = true,
                Id = "Ecksofa.SessionTracker: settings window",
                Parent = GameService.Graphics.SpriteScreen,
            };

            _selectStatsTab = new Tab(services.TextureService.StatsTabTexture, () => new SelectStatsSettingsTabView(services), "Select Stats");
            _settingsWindow.Tabs.Add(_selectStatsTab);
            _settingsWindow.Tabs.Add(new Tab(services.TextureService.StatsTabTexture, () => new ArrangeStatsSettingsTabView(services), "Arrange Stats"));
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
            _settingsWindow.SelectedTab = _selectStatsTab;
        }

        private readonly TabbedWindow2 _settingsWindow;
        private readonly Tab _selectStatsTab;
    }
}