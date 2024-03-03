using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.GameIntegration;
using Blish_HUD.Graphics.UI;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using SessionTracker.Api;
using SessionTracker.Files.RemoteFiles;
using SessionTracker.OtherServices;
using SessionTracker.Settings;
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
            _services.InitializeServicesAndDefineSettings(settings, Version, Gw2ApiManager);
            _services.DateTimeService.DefineSettings(settings);
        }

        public override IView GetSettingsView()
        {
            if (_moduleLoadError.HasModuleLoadFailed)
                return _moduleLoadError.CreateErrorSettingsView();
            else
                return new ModuleSettingsView(_services.SettingsWindowService);
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
                    $"api permissions. A Blish restart or module reinstall may not fix this. Disable and then enable works more reliable until the bug is fixed.";
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

            await _services.InitializeServices(localAndRemoteFileLocations, ContentsManager);
            var statsContainer = new StatsContainer(_services);
            _cornerIconService = new CornerIconService(statsContainer, CornerIconClickEventHandler, _services);
            _statsContainer = statsContainer;

            _services.SettingService.UiVisibilityKeyBindingSetting.Value.Activated += OnUiVisibilityKeyBindingActivated;
            _services.SettingService.UiVisibilityKeyBindingSetting.Value.Enabled   =  true;
        }

        private void CornerIconClickEventHandler(object s, MouseEventArgs e) => _statsContainer.ToggleVisibility();
        private void OnUiVisibilityKeyBindingActivated(object s, EventArgs e) => _statsContainer.ToggleVisibility();

        protected override void Unload()
        {
            if(_services != null )
            {
                _services?.Dispose();
                if (_services.SettingService != null)
                {
                    _services.SettingService.UiVisibilityKeyBindingSetting.Value.Enabled = false; // workaround to fix keybinding memory leak
                    _services.SettingService.UiVisibilityKeyBindingSetting.Value.Activated -= OnUiVisibilityKeyBindingActivated;
                }
            }

            _cornerIconService?.Dispose();
            _moduleLoadError?.Dispose();
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
                catch 
                {
                    /* NOOP */ 
                }
            }
        }

        public static readonly Logger Logger = Logger.GetLogger<Module>();
        private readonly Services _services = new Services();
        private StatsContainer _statsContainer;
        private CornerIconService _cornerIconService;
        private readonly ModuleLoadError _moduleLoadError = new ModuleLoadError();
    }
}