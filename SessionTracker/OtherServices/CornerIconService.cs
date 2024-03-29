﻿using System;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using SessionTracker.StatsWindow;

namespace SessionTracker.OtherServices
{
    public class CornerIconService : IDisposable
    {
        public CornerIconService(StatsContainer statsContainer, EventHandler<MouseEventArgs> cornerIconClickEventHandler, Services services)
        {
            _statsContainer              = statsContainer;
            _cornerIconClickEventHandler = cornerIconClickEventHandler;
            _services = services;

            services.SettingService.CornerIconIsVisibleSetting.SettingChanged += OnCornerIconIsVisibleSettingChanged;

            if (services.SettingService.CornerIconIsVisibleSetting.Value)
                CreateCornerIcon();
        }

        public void Dispose()
        {
            _services.SettingService.CornerIconIsVisibleSetting.SettingChanged -= OnCornerIconIsVisibleSettingChanged;

            if (_cornerIcon != null)
                RemoveCornerIcon();
        }

        private void CreateCornerIcon()
        {
            RemoveCornerIcon();

            _cornerIcon = new CornerIcon
            {
                Icon             = _services.TextureService.CornerIconTexture,
                HoverIcon        = _services.TextureService.CornerIconHoverTexture,
                BasicTooltipText = TOOLTIP_TEXT,
                Parent           = GameService.Graphics.SpriteScreen,
                Priority         = RANDOM_INTEGER_FOR_PRIORITY,
                Menu             = CreateContextMenu()
            };

            _cornerIcon.Menu  =  CreateContextMenu();
            _cornerIcon.Click += _cornerIconClickEventHandler;
        }

        private ContextMenuStrip CreateContextMenu()
        {
            _contextMenuStrip                   =  new ContextMenuStrip();
            _settingsContextMenuStripItem       =  _contextMenuStrip.AddMenuItem("Settings");
            _resetContextMenuStripItem          =  _contextMenuStrip.AddMenuItem("Reset session stats to 0");
            _settingsContextMenuStripItem.Click += OnSettingsContextMenuStripItemClick;
            _resetContextMenuStripItem.Click    += OnResetContextMenuStripItemClick;
            return _contextMenuStrip;
        }

        private void OnResetContextMenuStripItemClick(object sender, MouseEventArgs e)
        {
            Module.Logger.Info("Manual reset triggered with corner icon context menu by user");
            _statsContainer.ResetSession();
        }

        private void OnSettingsContextMenuStripItemClick(object sender, MouseEventArgs e)
        {
            _services.SettingsWindowService.ShowWindow();
        }

        private void RemoveCornerIcon()
        {
            if (_cornerIcon != null)
            {
                _settingsContextMenuStripItem.Click -= OnSettingsContextMenuStripItemClick;
                _resetContextMenuStripItem.Click    -= OnResetContextMenuStripItemClick;
                _settingsContextMenuStripItem.Dispose();
                _resetContextMenuStripItem.Dispose();
                CreateContextMenu()?.Dispose();
                _cornerIcon.Click -= _cornerIconClickEventHandler;
                _cornerIcon.Dispose();
                _cornerIcon = null;
            }
        }

        private void OnCornerIconIsVisibleSettingChanged(object sender, ValueChangedEventArgs<bool> e)
        {
            if (e.NewValue)
                CreateCornerIcon();
            else
                RemoveCornerIcon();
        }

        private readonly StatsContainer _statsContainer;
        private readonly EventHandler<MouseEventArgs> _cornerIconClickEventHandler;
        private readonly Services _services;
        private CornerIcon _cornerIcon;
        private ContextMenuStripItem _settingsContextMenuStripItem;
        private ContextMenuStripItem _resetContextMenuStripItem;
        private ContextMenuStrip _contextMenuStrip;
        private const int RANDOM_INTEGER_FOR_PRIORITY = 1275551536;

        private const string TOOLTIP_TEXT = "Left click:\n" +
                                            "Show or hide the session tracker UI. " +
                                            "Whether the UI is really shown depends on further visibility settings. " +
                                            "e.g. when 'on world map' is unchecked, clicking this menu icon will still not show the UI on the world map.\n\n" +
                                            "Right click:\n" +
                                            "Open context menu for more options\n\n" +
                                            "This menu icon can be hidden by module settings.";
    }
}