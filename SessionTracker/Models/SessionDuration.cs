using System;
using System.Diagnostics;

namespace SessionTracker.Models
{
    public class SessionDuration
    {
        public TimeSpan Value 
        { 
            get => _value + _stopwatch.Elapsed; 
            set => _value = value; 
        }

        public void StartMeasuring()
        {
            _stopwatch.Restart();
        }

        public void ResetDuration()
        {
            _stopwatch.Restart();
            _value = new TimeSpan();
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private TimeSpan _value;
    }
}
