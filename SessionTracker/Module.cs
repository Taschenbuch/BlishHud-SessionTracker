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
using SessionTracker.Api;
using SessionTracker.DateTimeUtcNow;
using SessionTracker.Files;
using SessionTracker.Files.RemoteFiles;
using SessionTracker.Models;
using SessionTracker.Other;
using SessionTracker.Services;
using SessionTracker.SettingEntries;
using SessionTracker.Settings;
using SessionTracker.SettingsWindow;
using SessionTracker.StatsWindow;

namespace SessionTracker
{
    [Export(typeof(Blish_HUD.Modules.Module))]
    public class Module : Blish_HUD.Modules.Module
    {
        [ImportingConstructor]
        public Module([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }

        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingService = new SettingService(settings);
            _dateTimeService.DefineSettings(settings);
            _debugLogService = new DebugLogService(Version, settings);
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

            if (await ApiService.IsApiTokenGeneratedWithoutRequiredPermissions(DirectoriesManager))
            {
                _moduleLoadError.HasModuleLoadFailed = true;
                _moduleLoadError.InfoText = $"DISABLE {Name} module, wait 5-10 seconds, after that ENABLE the module again here: " +
                    $"click Blish icon to open settings -> Manage Modules -> Session Tracker.\n" +
                    $"Reason: You recently updated this module. There is a bug in blish that prevents a module from getting api access after a module update with new " +
                    $"api permissions. A blish restart or module reinstall may not fix this. Disable and then enable works more reliable until the bug is fixed.";
                _moduleLoadError.ShowErrorWindow($"{Name}: !! READ THIS !!!");
                return;
            }

            var localAndRemoteFileLocations = new LocalAndRemoteFileLocations(new FileConstants(), DirectoriesManager);
            
            if (await RemoteFilesService.IsModuleVersionDeprecated(localAndRemoteFileLocations.DeprecatedTextUrl))
            {
                _moduleLoadError.HasModuleLoadFailed = true;
                _moduleLoadError.InfoText = await RemoteFilesService.GetDeprecatedText(localAndRemoteFileLocations.DeprecatedTextUrl);
                _moduleLoadError.ShowErrorWindow($"{Name}: Update module version :-)");               
                return;
            }

            if (!await RemoteFilesService.TryUpdateLocalWithRemoteFilesIfNecessary(localAndRemoteFileLocations))
            {
                _moduleLoadError.HasModuleLoadFailed = true;
                _moduleLoadError.InitForFailedDownload(Name);
                _moduleLoadError.ShowErrorWindow($"{Name}: Download failed :-(");
                return;
            }

            var fileService           = new FileService(localAndRemoteFileLocations);
            var model                 = await fileService.LoadModelFromFile();
            var textureService        = new TextureService(model, ContentsManager);
            var updateLoop            = new UpdateLoop(_settingService);
            var settingsWindowService = new SettingsWindowService(model, _settingService, _dateTimeService, textureService, updateLoop);

            var statsContainer = new StatsContainer(model, Gw2ApiManager, textureService, fileService, updateLoop, settingsWindowService, _settingService)
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
            _fileService           = fileService;
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

            _debugLogService?.Dispose();
            _moduleLoadError?.Dispose();
            _settingsWindowService?.Dispose();
            _cornerIconService?.Dispose();
            _textureService?.Dispose();
            _statsContainer?.Dispose();
            _dateTimeService?.Dispose();
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
        public static readonly Logger Logger = Logger.GetLogger<Module>();
        private StatsContainer _statsContainer;
        private FileService _fileService;
        private Model _model;
        private TextureService _textureService;
        private CornerIconService _cornerIconService;
        private SettingsWindowService _settingsWindowService;
        private DebugLogService _debugLogService;
        private readonly DateTimeService _dateTimeService = new DateTimeService();
        private readonly ModuleLoadError _moduleLoadError = new ModuleLoadError();
    }
}