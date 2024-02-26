using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using SessionTracker.DateTimeUtcNow;
using SessionTracker.Files;
using SessionTracker.Files.RemoteFiles;
using SessionTracker.Models;
using SessionTracker.Other;
using SessionTracker.SettingEntries;
using SessionTracker.SettingsWindow;
using SessionTracker.StatsWindow;
using System;
using System.Threading.Tasks;

namespace SessionTracker.OtherServices

{
    public class Services : IDisposable
    {
        // has to be called FIRST
        public void InitializeServicesAndDefineSettings(SettingCollection settings, SemVer.Version moduleVersion, Gw2ApiManager gw2ApiManager)
        {
            var settingService = new SettingService(settings);
            var updateLoop = new UpdateLoop(settingService);
            var debugLogService = new DebugLogService(moduleVersion, settings); // must be created AFTER SettingService

            Gw2ApiManager = gw2ApiManager;
            SettingService = settingService;
            UpdateLoop = updateLoop;
            DebugLogService = debugLogService;
        }

        // has to be called SECOND
        public async Task InitializeServices(LocalAndRemoteFileLocations localAndRemoteFileLocations, ContentsManager contentsManager)
        {
            var fileService = new FileService(localAndRemoteFileLocations);
            var model = await fileService.LoadModelFromFile();
            var textureService = new TextureService(model, contentsManager);

            FileService = fileService;
            Model = model;
            TextureService = textureService;
            SettingsWindowService = new SettingsWindowService(this);
        }

        public void Dispose()
        {
            if (Model != null)
                FileService.SaveModelToFile(Model);

            DebugLogService?.Dispose();
            TextureService?.Dispose();
            DateTimeService?.Dispose();
            SettingsWindowService?.Dispose();
        }

        public SettingService SettingService { get; private set; }
        public Model Model { get; private set; }
        public TextureService TextureService { get; private set; }
        public UpdateLoop UpdateLoop { get; private set; }
        public DateTimeService DateTimeService { get; } = new DateTimeService();
        public FileService FileService { get; private set; }
        public Gw2ApiManager Gw2ApiManager { get; private set; }
        public SettingsWindowService SettingsWindowService { get; private set; }
        public DebugLogService DebugLogService { get; set; }
    }
}
