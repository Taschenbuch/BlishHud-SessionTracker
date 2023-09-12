using System.Collections.Generic;

namespace SessionTracker.Services.RemoteFiles
{
    public class FileConstants
    {
        public string RemoteBaseUrl = "https://bhm.blishhud.com/ecksofa.sessiontracker";
        public string ModuleFolderName = "session-tracker";
        public string FormatVersion = "1";
        public const string ModelFileName = "model.json"; // used for remote AND local model because they are in different folders
        public List<string> RemotelyUpdatableDataFileNames { get; } = new List<string>
        {
            ModelFileName,
        };
    }
}
