using System;

namespace SessionTracker.Settings
{
    [Serializable]
    public class LogWarnException : Exception
    {
        public LogWarnException(string message) 
            : base(message) { }

        public LogWarnException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
