﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SessionTracker.Files.RemoteFiles
{
    public class RemoteFilesService
    {
        public static async Task<bool> IsModuleVersionDeprecated(string deprecatedUrl)
        {
            try
            {
                await GetTextFromUrl(deprecatedUrl);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<string> GetDeprecatedText(string deprecatedUrl)
        {
            try
            {
                var deprecatedText = await GetTextFromUrl(deprecatedUrl);
                return string.IsNullOrWhiteSpace(deprecatedText)
                    ? DEFAULT_DEPRECATED_TEXT
                    : deprecatedText;
            }
            catch (Exception)
            {
                return DEFAULT_DEPRECATED_TEXT;
            }
        }

        public static async Task<bool> TryUpdateLocalWithRemoteFilesIfNecessary(LocalAndRemoteFileLocations localAndRemoteFileLocations)
        {
            try
            {
                var areLocalFilesMissing = AreLocalFilesMissing(localAndRemoteFileLocations.DataFileLocations);
                if (areLocalFilesMissing)
                {
                    await DownloadFilesFromRemote(localAndRemoteFileLocations.DataFileLocations);
                    return true;
                }

                var areNewerRemoteFilesAvailable = await AreNewerRemoteFilesAvailable(localAndRemoteFileLocations.ContentVersionFilePath);
                if (areNewerRemoteFilesAvailable)
                    await DownloadFilesFromRemote(localAndRemoteFileLocations.DataFileLocations);

                return true;
            }
            catch (Exception e)
            {
                // error because there is no fallback data in ref folder. Module may stop working completely without this data
                Module.Logger.Error(e, "Failed to update module data from remote. :(");
                return false;
            }
        }

        private static async Task DownloadFilesFromRemote(List<FileLocation> dataFilePaths)
        {
            foreach (var dataFilePath in dataFilePaths)
            {
                var remoteFileContent = await GetTextFromUrl(dataFilePath.RemoteUrl); // could be optimized by awaiting multiple at once
                await FileService.WriteFileAsync(remoteFileContent, dataFilePath.LocalFilePath);
            }
        }
        private static async Task<bool> AreNewerRemoteFilesAvailable(FileLocation contentVersionFilePath)
        {
            var onlineDataVersion = await GetRemoteVersion(contentVersionFilePath.RemoteUrl);
            var localDataVersion = GetLocalVersion(contentVersionFilePath.LocalFilePath);
            return onlineDataVersion > localDataVersion;
        }

        private static int GetLocalVersion(string contentVersionLocalFilePath)
        {
            var versionText = File.ReadAllText(contentVersionLocalFilePath);
            return int.Parse(versionText);
        }

        private static async Task<int> GetRemoteVersion(string contentVersionRemoteUrl)
        {
            var versionText = await GetTextFromUrl(contentVersionRemoteUrl);
            return int.Parse(versionText);
        }

        private static async Task<string> GetTextFromUrl(string url)
        {
            // dont add try catch. checking if module is deprecated relies on this throwing an exception when deprecated.txt file is not found
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync(url);
        }

        private static bool AreLocalFilesMissing(List<FileLocation> dataFilePaths)
        {
            return dataFilePaths.Any(d => !File.Exists(d.LocalFilePath));
        }

        private const string DEFAULT_DEPRECATED_TEXT = "This module version is deprecated. :-(\n" +
                                                       "Please update to the newest module version.\n" +
                                                       "To have access to the newest module version, make sure you are also using the newest Blish HUD version. :-)";
    }
}