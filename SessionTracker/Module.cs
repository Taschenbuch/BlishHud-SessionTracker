using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.GameIntegration;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using SessionTracker.Controls;
using SessionTracker.Models;
using SessionTracker.Services;
using SessionTracker.Services.Api;
using SessionTracker.Services.RemoteFiles;
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
            if (_moduleLoadError.HasModuleLoadFailed)
                return _moduleLoadError.CreateErrorSettingsView();
            else
                return new ModuleSettingsView(_settingsWindowService);
        }

        protected override async Task LoadAsync()
        {
            RunShiftBlishCornerIconsWorkaroundBecauseOfNewWizardVaultIcon();

            _moduleLoadError.HasModuleLoadFailed = await ApiService.IsApiTokenGeneratedWithoutRequiredPermissions(Logger);
            if (_moduleLoadError.HasModuleLoadFailed)
            {
                _moduleLoadError.InfoText = $"DISABLE {Name} module, wait 5-10 seconds, after that ENABLE the module again here: " +
                    $"click Blish icon to open settings -> Manage Modules -> Session Tracker.\n" +
                    $"Reason: You recently updated this module. There is a bug in blish that prevents a module from getting api access after a module update with new " +
                    $"api permissions. A blish restart or module reinstall may not fix this. Disable and then enable works more reliable until the bug is fixed.";
                _moduleLoadError.ShowErrorWindow($"{Name}: !! READ THIS !!!");
                return;
            }

            var localAndRemoteFileLocations = new LocalAndRemoteFileLocations(new FileConstants(), DirectoriesManager);
            
            _moduleLoadError.HasModuleLoadFailed = await RemoteFilesService.IsModuleVersionDeprecated(localAndRemoteFileLocations.DeprecatedTextUrl);
            if (_moduleLoadError.HasModuleLoadFailed)
            {
                _moduleLoadError.InfoText = await RemoteFilesService.GetDeprecatedText(localAndRemoteFileLocations.DeprecatedTextUrl);
                _moduleLoadError.ShowErrorWindow($"{Name}: Update module version :-)");               
                return;
            }

            _moduleLoadError.HasModuleLoadFailed = !await RemoteFilesService.TryUpdateLocalWithRemoteFilesIfNecessary(localAndRemoteFileLocations, Logger);
            if (_moduleLoadError.HasModuleLoadFailed)
            {
                _moduleLoadError.InitForFailedDownload(Name);
                _moduleLoadError.ShowErrorWindow($"{Name}: Download failed :-(");
                return;
            }

            _fileService              = new FileService(localAndRemoteFileLocations, Logger);
            var model                 = await _fileService.LoadModelFromFile();
            var textureService        = new TextureService(model, ContentsManager, Logger);
            var settingsWindowService = new SettingsWindowService(model, _settingService, textureService);

            var statsContainer = new StatsContainer(model, Gw2ApiManager, textureService, settingsWindowService, _settingService, Logger)
            {
                HeightSizingMode = SizingMode.AutoSize,
                WidthSizingMode  = SizingMode.AutoSize,
                BackgroundColor  = ColorService.CreateBackgroundColor(_settingService),
                Visible          = _settingService.UiIsVisibleSetting.Value,
                Parent           = GameService.Graphics.SpriteScreen
            };

            _cornerIconService = new CornerIconService(_settingService.CornerIconIsVisibleSetting, statsContainer, settingsWindowService, CornerIconClickEventHandler, textureService);

            // set at the end to prevent that one of the ctors accidently gets a null reference because of creating the objects above in the wrong order.
            // e.g. creating model after textureService, though model needs the reference of model.
            _model                 = model;
            _textureService        = textureService;
            _statsContainer        = statsContainer;
            _settingsWindowService = settingsWindowService;

            _settingService.UiVisibilityKeyBindingSetting.Value.Activated += OnUiVisibilityKeyBindingActivated;
            _settingService.UiVisibilityKeyBindingSetting.Value.Enabled   =  true;
        }

        private void CornerIconClickEventHandler(object s, MouseEventArgs e) => _statsContainer.ToggleVisibility();
        private void OnUiVisibilityKeyBindingActivated(object s, EventArgs e) => _statsContainer.ToggleVisibility();

        protected override void Unload()
        {
            if (_model != null)
                _fileService.SaveModelToFile(_model);

            if (_settingService != null)
            {
                _settingService.UiVisibilityKeyBindingSetting.Value.Enabled = false; // workaround to fix keybinding memory leak
                _settingService.UiVisibilityKeyBindingSetting.Value.Activated -= OnUiVisibilityKeyBindingActivated;
            }

            _moduleLoadError?.Dispose();
            _settingsWindowService?.Dispose();
            _cornerIconService?.Dispose();
            _textureService?.Dispose();
            _statsContainer?.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_moduleLoadError.HasModuleLoadFailed)
                return;

            _statsContainer.Update2(gameTime);
        }

        private static void RunShiftBlishCornerIconsWorkaroundBecauseOfNewWizardVaultIcon()
        {
            if (Program.OverlayVersion < new SemVer.Version(1, 1, 0))
            {
                try
                {
                    var tacoActive = typeof(TacOIntegration).GetProperty(nameof(TacOIntegration.TacOIsRunning)).GetSetMethod(true);
                    tacoActive?.Invoke(GameService.GameIntegration.TacO, new object[] { true });
                }
                catch { /* NOOP */ }
            }
        }

        private SettingService _settingService;
        private static readonly Logger Logger = Logger.GetLogger<Module>();
        private StatsContainer _statsContainer;
        private FileService _fileService;
        private Model _model;
        private TextureService _textureService;
        private CornerIconService _cornerIconService;
        private SettingsWindowService _settingsWindowService;
        private readonly ModuleLoadError _moduleLoadError = new ModuleLoadError();
    }
}