using System;

namespace SessionTracker.Settings
{
    [Serializable]
    public class MigrationException : Exception
    {
        public MigrationException(string message) 
            : base(message) { }
    }
}
