namespace SessionTracker.Models
{
    public class Value
    {
        public int Session => Total - TotalAtSessionStart;
        public int Total { get; set; }
        public int TotalAtSessionStart { get; set; }
    }
}