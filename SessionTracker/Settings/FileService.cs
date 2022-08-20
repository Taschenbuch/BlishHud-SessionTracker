using System;
using System.IO;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Modules.Managers;
using Gw2Sharp.WebApi;
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
                var modelJson = JsonConvert.SerializeObject(model, Formatting.Indented);
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

            var noModuleFolderModelExists = File.Exists(_modelFilePath) == false;
            if (noModuleFolderModelExists)
                return refFolderModel;

            var moduleFolderModel = await LoadModelFromModuleFolder(_modelFilePath, _logger);
            moduleFolderModel = MigrationService.MigratePersistedModelIfNecessary(moduleFolderModel, refFolderModel, _logger);

            return moduleFolderModel;
        }

        private static async Task<Model> LoadModelFromModuleFolder(string modelFilePath, Logger logger)
        {
            try
            {
                using (var fileStream = File.OpenRead(modelFilePath))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var modelJson = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Model>(modelJson);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: Failed to load model from file in module folder in Module.LoadAsync(). :(");
                return MODEL_WITH_ERROR_ENTRY; // todo richtiges handling hierfür nötig. Das Model ist ja leer
            }
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
                return MODEL_WITH_ERROR_ENTRY; // todo richtiges handling hierfür nötig. Das Model ist ja leer
            }
        }

        private readonly string _modelFilePath;
        private const string MODEL_FILE_NAME = "model.json";
        private const string MODULE_FOLDER_NAME = "session-tracker";
        private readonly ContentsManager _contentsManager;
        private readonly Logger _logger;

        private static Model MODEL_WITH_ERROR_ENTRY
        {
            get
            {
                var dummyEntry = new Entry();
                dummyEntry.LabelText.SetLocalizedText("Failed to load model from file", Locale.English);

                return new Model {
                    Entries = { dummyEntry }
                };
            }
        }
    }
}