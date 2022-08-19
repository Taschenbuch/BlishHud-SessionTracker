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
using SessionTracker.Settings.SettingEntries;
using SessionTracker.Settings.Window;

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
            return new ModuleSettingsView(_settingsWindowService);
        }

        protected override async Task LoadAsync()
        {
            _fileService = new FileService(DirectoriesManager, ContentsManager, Logger);
            var model                 = await _fileService.LoadModelFromFile();
            var textureService        = new TextureService(model, ContentsManager, Logger);
            var settingsWindowService = new SettingsWindowService(model, _settingService, textureService);

            var entriesContainer = new EntriesContainer(model, Gw2ApiManager, textureService, settingsWindowService, _settingService, Logger)
            {
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                BackgroundColor  = new Color(Color.Black, _settingService.BackgroundOpacitySetting.Value),
                Visible          = _settingService.UiIsVisibleSetting.Value,
                Parent           = GameService.Graphics.SpriteScreen
            };

            _cornerIconService = new CornerIconService(_settingService.CornerIconIsVisibleSetting, entriesContainer, settingsWindowService, CornerIconClickEventHandler, textureService);

            // set at the end to prevents that one of the ctors accidently gets a null reference because of creating the objects above in the wrong order.
            // e.g. creating model after textureService, though model needs the reference of model.
            _model                 = model;
            _textureService        = textureService;
            _entriesContainer      = entriesContainer;
            _settingsWindowService = settingsWindowService;

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

            _settingsWindowService?.Dispose();
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
        private SettingsWindowService _settingsWindowService;
    }
}