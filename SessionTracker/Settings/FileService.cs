using System;
using System.IO;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Newtonsoft.Json;
using SessionTracker.Models;

namespace SessionTracker.Settings
{
    public class FileService
    {
        public FileService(DirectoriesManager directoriesManager, ContentsManager contentsManager, Logger logger)
        {
            _contentsManager = contentsManager;
            _logger          = logger;
            var moduleFolderPath = directoriesManager.GetFullDirectoryPath(MODULE_FOLDER_NAME);
            _modelFilePath = Path.Combine(moduleFolderPath, MODEL_FILE_NAME);
        }

        public void SaveModelToFile(Model model)
        {
            try
            {
                // not async because Modul.Unload() is not async
                var modelJson = JsonService.SerializeModelToJson(model);
                File.WriteAllText(_modelFilePath, modelJson);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error: Failed to save model to file in Module.Unload(). :(");
            }
        }

        public async Task<Model> LoadModelFromFile()
        {
            var refFolderModel = await LoadModelFromRefFolder(_contentsManager, _logger);

            var isFirstModuleStart = File.Exists(_modelFilePath) == false;
            if (isFirstModuleStart)
                return refFolderModel;

            var moduleFolderModel = await LoadModelFromModuleFolder(_modelFilePath, refFolderModel, _logger);
            refFolderModel = RefModelService.UpdateStatIsVisibleInRefModel(moduleFolderModel, refFolderModel);
            refFolderModel = RefModelService.UpdateStatsOrderInRefModel(moduleFolderModel, refFolderModel);
            return refFolderModel;
        }

        private static async Task<Model> LoadModelFromModuleFolder(string modelFilePath, ModelVersion refModelVersion, Logger logger)
        {
            try
            {
                var modelJson = await ReadModuleFolderFile(modelFilePath);

                if (string.IsNullOrWhiteSpace(modelJson))
                {
                    // fix for sentry null reference exception.
                    // Because JsonConvert.DeserializeObject returns null instead of throwing an exception when json file is empty string. no idea why file is empty sometimes
                    logger.Warn("Error: Failed to load model from file in module folder in Module.LoadAsync(). File is empty :(");
                    return new Model();
                }

                modelJson = MigrationService.RenamePropertyMajorVersionToVersion(logger, modelJson);
                var modelVersion = JsonConvert.DeserializeObject<ModelVersion>(modelJson);
                modelJson = MigrationService.MigrateModelIfIsOldVersion(modelJson, modelVersion, refModelVersion, logger);
                var model = JsonConvert.DeserializeObject<Model>(modelJson);
                return model;
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

        private static async Task<string> ReadModuleFolderFile(string modelFilePath)
        {
            using (var fileStream = File.OpenRead(modelFilePath))
            using (var streamReader = new StreamReader(fileStream))
                return await streamReader.ReadToEndAsync();
        }

        private static async Task<Model> LoadModelFromRefFolder(ContentsManager contentsManager, Logger logger)
        {
            try
            {
                using (var fileStream = contentsManager.GetFileStream(MODEL_FILE_NAME))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var modelJson = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Model>(modelJson);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: Failed to load model from file in ref folder in Module.LoadAsync(). :(");
                return new Model();
            }
        }

        private readonly string _modelFilePath;
        private const string MODEL_FILE_NAME = "model.json";
        private const string MODULE_FOLDER_NAME = "session-tracker";
        private readonly ContentsManager _contentsManager;
        private readonly Logger _logger;
    }
}