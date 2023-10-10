using System.Collections.Generic;

namespace SessionTracker.Files.RemoteFiles
{
    public class FileConstants
    {
        public const string REMOTE_BASE_URL = "https://bhm.blishhud.com/ecksofa.sessiontracker";
        public const string MODULE_FOLDER_NAME = "session-tracker";
        public const string FORMAT_VERSION = "1";
        public const string MODEL_FILE_NAME = "model.json"; // used for remote AND local model because they are in different folders
        public List<string> RemotelyUpdatableDataFileNames { get; } = new List<string>
        {
            MODEL_FILE_NAME
        };
    }
}
