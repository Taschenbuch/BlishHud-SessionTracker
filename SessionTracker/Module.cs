using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Settings;

namespace SessionTracker
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingService = new SettingService(settings);
        }

        public override IView GetSettingsView()
        {
            return new ModuleSettingsView(_model, _entriesContainer, _settingService, _textureService);
        }

        protected override async Task LoadAsync()
        {
            _fileService    = new FileService(DirectoriesManager, ContentsManager, Logger);
            _model          = await _fileService.LoadModelFromFile();
            _textureService = new TextureService(_model, ContentsManager, Logger);

            _entriesContainer = new EntriesContainer(_model, Gw2ApiManager, _textureService, _settingService, Logger)
            {
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                BackgroundColor  = new Color(Color.Black, _settingService.BackgroundOpacitySetting.Value),
                Visible          = _settingService.UiIsVisibleSetting.Value,
                Parent           = GameService.Graphics.SpriteScreen
            };

            _cornerIconService = new CornerIconService(
                _settingService.CornerIconIsVisibleSetting, 
                "Click to show or hide the session tracker UI.\nIcon can be hidden by module settings.\n" +
                "Whether UI is really shown depends on other visibility settings. " +
                "e.g. when 'on world map' is unchecked, clicking the icon will still not show the UI on the world map.",
                CornerIconClickEventHandler, 
                _textureService);

            _settingService.UiVisibilityKeyBindingSetting.Value.Activated += OnUiVisibilityKeyBindingActivated;
            _settingService.UiVisibilityKeyBindingSetting.Value.Enabled   =  true;
        }

        private void CornerIconClickEventHandler(object s, MouseEventArgs e) => _entriesContainer.ToggleVisibility();
        private void OnUiVisibilityKeyBindingActivated(object s, EventArgs e) => _entriesContainer.ToggleVisibility();

        protected override void Unload()
        {
            if (_model != null)
                _fileService.SaveModelToFile(_model);

            _settingService.UiVisibilityKeyBindingSetting.Value.Activated -= OnUiVisibilityKeyBindingActivated;

            _cornerIconService?.Dispose();
            _textureService?.Dispose();
            _entriesContainer?.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            _entriesContainer.Update2(gameTime);
        }

        private SettingService _settingService;
        private static readonly Logger Logger = Logger.GetLogger<Module>();
        private EntriesContainer _entriesContainer;
        private FileService _fileService;
        private Model _model;
        private TextureService _textureService;
        private CornerIconService _cornerIconService;
    }
}