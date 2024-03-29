﻿using Blish_HUD.Modules.Managers;
using System.Collections.Generic;

namespace SessionTracker.Files.RemoteFiles
{
    public class LocalAndRemoteFileLocations
    {
        public LocalAndRemoteFileLocations(FileConstants fileConstants, DirectoriesManager directoriesManager)
        {
            _fileConstants = fileConstants;
            LocalRootFolderPath = directoriesManager.GetFullDirectoryPath(FileConstants.MODULE_FOLDER_NAME);
            CreateFileLocations();
        }

        public string DeprecatedTextUrl => GetRemoteFilePath("deprecated.txt");
        public string LocalRootFolderPath { get; }
        public List<FileLocation> DataFileLocations { get; } = new List<FileLocation>();
        public FileLocation ContentVersionFilePath => new FileLocation
        {
            LocalFilePath = GetLocalFilePath("content_version.txt"),
            RemoteUrl = GetRemoteFilePath("content_version.txt")
        };
        public string GetLocalFilePath(string fileName)
        {
            return $"{LocalRootFolderPath}/{_relativeFolderPath}/{fileName}";
        }

        private void CreateFileLocations()
        {
            foreach (var dataFileName in _fileConstants.RemotelyUpdatableDataFileNames)
                DataFileLocations.Add(new FileLocation
                {
                    LocalFilePath = GetLocalFilePath(dataFileName),
                    RemoteUrl = GetRemoteFilePath(dataFileName),
                });

            // version file MUST be the last file. When a previous file fails to download, version file will not be updated, too.
            // because of that on the next module startup it will then retry the download of all files including the previously failed files.
            DataFileLocations.Add(ContentVersionFilePath);
        }

        private string GetRemoteFilePath(string fileName)
        {
            return $"{FileConstants.REMOTE_BASE_URL}/{_relativeFolderPath}/{fileName}";
        }

        private string _relativeFolderPath => $"data/format_version_{FileConstants.FORMAT_VERSION}";
        private readonly FileConstants _fileConstants;
    }
}
