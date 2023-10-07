using Blish_HUD;
using System;

namespace SessionTracker.StatsWindow
{
    public class Interval
    {
        public Interval(TimeSpan intervalTimeSpan)
        {
            // do not store times as timespans. timespan has good readability as parameter, but bad performance for comparison and other operations.
            _intervalInMilliseconds = intervalTimeSpan.TotalMilliseconds; 
            _intervalEndInMilliseconds = GameService.Overlay.CurrentGameTime.TotalGameTime.TotalMilliseconds + _intervalInMilliseconds;
        }

        public bool HasEnded()
        {
            if (_intervalEndInMilliseconds > GameService.Overlay.CurrentGameTime.TotalGameTime.TotalMilliseconds)
                return false;

            _intervalEndInMilliseconds = GameService.Overlay.CurrentGameTime.TotalGameTime.TotalMilliseconds + _intervalEndInMilliseconds;
            return true;
        }

        private readonly double _intervalInMilliseconds;
        private double _intervalEndInMilliseconds;
    }
}
