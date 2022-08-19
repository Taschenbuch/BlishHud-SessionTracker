using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Settings.SettingEntries;

namespace SessionTracker.Settings.Window
{
    public class SettingsWindowService : IDisposable
    {
        public SettingsWindowService(Model model, SettingService settingService, TextureService textureService)
        {
            _statsSettingsTabView      = new StatsSettingsTabView(model, settingService, textureService);
            _generalSettingsTabView    = new GeneralSettingsTabView(settingService);
            _visibilitySettingsTabView = new VisibilitySettingsTabView(settingService);

            _settingsWindow = new TabbedWindow2(
                textureService.WindowBackgroundTexture,
                new Rectangle(40, 30, 720, 700),
                new Rectangle(80, 30, 680, 630))
            {
                Title         = "Session Tracker",
                Emblem        = textureService.SettingsWindowEmblemTexture,
                Location      = new Point(300, 300),
                SavesPosition = true,
                Id            = "SessionTracker settings window",
                Parent        = GameService.Graphics.SpriteScreen,
            };

            _statsTab      = new Tab(textureService.StatsTabTexture, () => _statsSettingsTabView, "Tracked Stats");
            _generalTab    = new Tab(textureService.GeneralTabTexture, () => _generalSettingsTabView, "General");
            _visibilityTab = new Tab(textureService.VisibilityTabTexture, () => _visibilitySettingsTabView, "UI Visibility");

            _settingsWindow.Tabs.Add(_statsTab);
            _settingsWindow.Tabs.Add(_generalTab);
            _settingsWindow.Tabs.Add(_visibilityTab);
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
        private readonly StatsSettingsTabView _statsSettingsTabView;
        private readonly GeneralSettingsTabView _generalSettingsTabView;
        private readonly VisibilitySettingsTabView _visibilitySettingsTabView;
        private readonly Tab _statsTab;
        private readonly Tab _generalTab;
        private readonly Tab _visibilityTab;
    }
}