using System;
using System.IO;
using System.Threading.Tasks;
using Blish_HUD;
using Newtonsoft.Json;
using SessionTracker.Files.RemoteFiles;
using SessionTracker.FilesAndMigration;
using SessionTracker.Models;
using SessionTracker.Other;

namespace SessionTracker.Files
{
    public class FileService
    {
        public FileService(LocalAndRemoteFileLocations localAndRemoteFileLocations)
        {
            _localModelFilePath = Path.Combine(localAndRemoteFileLocations.LocalRootFolderPath, FileConstants.ModelFileName);
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
                Module.Logger.Error(e, "Error: Failed to save model to file. :(");
            }
        }

        // dont use this in Module.Unload. We want it to be sync in .Unload()
        public async Task SaveModelToFileAsync(Model model)
        {
            try
            {
                var modelJson = JsonService.SerializeModelToJson(model);
                await WriteFileAsync(modelJson, _localModelFilePath);
            }
            catch (Exception e)
            {
                Module.Logger.Error(e, "Error: Failed to save model to file. :(");
            }
        }

        public static async Task WriteFileAsync(string fileContent, string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(folderPath);
            using var streamWriter = new StreamWriter(filePath);
            await streamWriter.WriteAsync(fileContent);
            await streamWriter.FlushAsync();
        }

        public async Task<Model> LoadModelFromFile()
        {
            var remoteModel = await LoadModelFromRemoteFolder(_remoteModelFilePath);

            var isFirstModuleStart = File.Exists(_localModelFilePath) == false;
            if (isFirstModuleStart)
                return remoteModel;

            var localModel = await LoadModelFromModuleFolder(_localModelFilePath, remoteModel);
            remoteModel = RemoteModelService.UpdateStatIsVisibleInRemoteModel(localModel, remoteModel);
            remoteModel = RemoteModelService.UpdateStatsOrderInRemoteModel(localModel, remoteModel);
            remoteModel = RemoteModelService.UpdateTotalAtSessionStartInRemoteModel(localModel, remoteModel);
            remoteModel = RemoteModelService.UpdateSessionDurationAndResetTimeInRemoteModel(localModel, remoteModel);
            return remoteModel;
        }

        private static async Task<Model> LoadModelFromRemoteFolder(string modelFilePath)
        {
            try
            {
                var modelJson = await GetFileContentAndThrowIfFileEmpty(modelFilePath);
                return JsonConvert.DeserializeObject<Model>(modelJson);
            }
            catch (Exception e)
            {
                Module.Logger.Error(e, "Error: Failed to load remote model from file. :(");
                return new Model();
            }
        }

        private static async Task<Model> LoadModelFromModuleFolder(string modelFilePath, ModelVersion remoteModelVersion)
        {
            try
            {
                var modelJson = await GetFileContentAndThrowIfFileEmpty(modelFilePath);
                modelJson = MigrationService.RenamePropertyMajorVersionToVersion(modelJson);
                var localModelVersion = JsonConvert.DeserializeObject<ModelVersion>(modelJson);
                modelJson = MigrationService.MigrateModelJsonIfIsOldVersion(modelJson, localModelVersion, remoteModelVersion);
                return JsonConvert.DeserializeObject<Model>(modelJson);
            }
            catch (LogWarnException e)
            {
                Module.Logger.Warn(e.Message);
                return new Model();
            }
            catch (Exception e)
            {
                Module.Logger.Error(e, "Error: Failed to load local model from file. :(");
                return new Model();
            }
        }

        private static async Task<string> GetFileContentAndThrowIfFileEmpty(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            using var streamReader = new StreamReader(fileStream);
            var fileContent = await streamReader.ReadToEndAsync();

            // Because JsonConvert.DeserializeObject returns null for empty string. no idea why file is empty sometimes (was reported in sentry)
            if (string.IsNullOrWhiteSpace(fileContent))
                throw new Exception("file is empty!");

            return fileContent;
        }

        private readonly string _localModelFilePath;
        private readonly string _remoteModelFilePath;
    }
}