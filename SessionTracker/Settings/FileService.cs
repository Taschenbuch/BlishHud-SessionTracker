using System;
using System.IO;
using System.Threading.Tasks;
using Blish_HUD;
using Newtonsoft.Json;
using SessionTracker.Models;
using SessionTracker.Services.RemoteFiles;

namespace SessionTracker.Settings
{
    public class FileService
    {
        public FileService(LocalAndRemoteFileLocations localAndRemoteFileLocations, Logger logger)
        {
            _logger              = logger;
            _localModelFilePath  = Path.Combine(localAndRemoteFileLocations.LocalRootFolderPath, FileConstants.ModelFileName);
            _remoteModelFilePath = localAndRemoteFileLocations.GetLocalFilePath(FileConstants.ModelFileName);
        }

        public void SaveModelToFile(Model model)
        {
            try
            {
                // not async because Modul.Unload() is not async
                var modelJson = JsonService.SerializeModelToJson(model);
                File.WriteAllText(_localModelFilePath, modelJson);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error: Failed to save model to file in Module.Unload(). :(");
            }
        }

        public async Task<Model> LoadModelFromFile()
        {
            var remoteFolderModel = await LoadModelFromRemoteFolder(_remoteModelFilePath, _logger);

            var isFirstModuleStart = File.Exists(_localModelFilePath) == false;
            if (isFirstModuleStart)
                return remoteFolderModel;

            var moduleFolderModel = await LoadModelFromModuleFolder(_localModelFilePath, remoteFolderModel, _logger);
            remoteFolderModel = RemoteModelService.UpdateStatIsVisibleInRemoteModel(moduleFolderModel, remoteFolderModel);
            remoteFolderModel = RemoteModelService.UpdateStatsOrderInRemoteModel(moduleFolderModel, remoteFolderModel);
            return remoteFolderModel;
        }

        private static async Task<Model> LoadModelFromRemoteFolder(string modelFilePath, Logger logger)
        {
            try
            {
                var modelJson = await ReadFile(modelFilePath);
                if (string.IsNullOrWhiteSpace(modelJson))
                {
                    // fix for sentry null reference exception.
                    // Because JsonConvert.DeserializeObject returns null instead of throwing an exception when json file is empty string. no idea why file is empty sometimes
                    logger.Warn("Error: Failed to load model from file in module folder in Module.LoadAsync(). File is empty :(");
                    return new Model();
                }

                return JsonConvert.DeserializeObject<Model>(modelJson);
            }
            catch (MigrationException e)
            {
                logger.Warn(e.Message);
                return new Model();
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: Failed to load model from file in module folder in Module.LoadAsync(). :(");
                return new Model();
            }
        }


        private static async Task<Model> LoadModelFromModuleFolder(string modelFilePath, ModelVersion remoteModelVersion, Logger logger)
        {
            try
            {
                var modelJson = await ReadFile(modelFilePath);

                if (string.IsNullOrWhiteSpace(modelJson))
                {
                    // fix for sentry null reference exception.
                    // Because JsonConvert.DeserializeObject returns null instead of throwing an exception when json file is empty string. no idea why file is empty sometimes
                    logger.Warn("Error: Failed to load model from file in module folder in Module.LoadAsync(). File is empty :(");
                    return new Model();
                }

                modelJson = MigrationService.RenamePropertyMajorVersionToVersion(logger, modelJson);
                var localModelVersion = JsonConvert.DeserializeObject<ModelVersion>(modelJson);
                modelJson = MigrationService.MigrateModelJsonIfIsOldVersion(modelJson, localModelVersion, remoteModelVersion, logger);
                return JsonConvert.DeserializeObject<Model>(modelJson);
            }
            catch (MigrationException e)
            {
                logger.Warn(e.Message);
                return new Model();
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: Failed to load model from file in module folder in Module.LoadAsync(). :(");
                return new Model();
            }
        }

        private static async Task<string> ReadFile(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            using (var streamReader = new StreamReader(fileStream))
                return await streamReader.ReadToEndAsync();
        }

        private readonly string _localModelFilePath;
        private readonly string _remoteModelFilePath;
        private readonly Logger _logger;
    }
}