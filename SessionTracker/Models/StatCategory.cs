namespace SessionTracker.Models
{
    public class StatCategory
    {
        public string Id { get; set; } = string.Empty;
        public LocalizedText Name { get; } = new LocalizedText(); // json is easier to read when this is the last property
    }
}
