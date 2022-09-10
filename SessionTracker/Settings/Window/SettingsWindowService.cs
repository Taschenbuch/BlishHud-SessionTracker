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
            textureService.SettingsWindowEmblemTexture.TextureSwapped += (s, e) => 
            {
                // hack for blish 0.11.7 tabbedWindow2 not handling asyncTexture swapping
                _settingsWindow = new TabbedWindow2(
                    textureService.SettingsWindowBackgroundTexture,
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

                _statsTab = new Tab(textureService.StatsTabTexture, () => new StatsSettingsTabView(model, settingService, textureService), "Tracked Stats");
                _settingsWindow.Tabs.Add(_statsTab);
                _settingsWindow.Tabs.Add(new Tab(textureService.GeneralTabTexture, () => new GeneralSettingsTabView(settingService), "General"));
                _settingsWindow.Tabs.Add(new Tab(textureService.VisibilityTabTexture, () => new VisibilitySettingsTabView(settingService), "UI Visibility"));
#if DEBUG
                _settingsWindow.Tabs.Add(new Tab(textureService.DebugTabTexture, () => new DebugSettingsTabView(settingService), "Debug"));
#endif
            };
        }

        public void Dispose()
        {
            _settingsWindow?.Dispose();
        }

        public void ShowWindow()
        {
            if (_settingsWindow == null) // hack for blish 0.11.7 tabbedWindow2 not handling asyncTexture swapping
                return;
            
            _settingsWindow.Show();
        }

        public void ShowWindowAndSelectStatsTab()
        {
            if (_settingsWindow == null) // hack for blish 0.11.7 tabbedWindow2 not handling asyncTexture swapping
                return;

            _settingsWindow.Show();
            _settingsWindow.SelectedTab = _statsTab;
        }

        private TabbedWindow2 _settingsWindow;
        private Tab _statsTab;
    }
}