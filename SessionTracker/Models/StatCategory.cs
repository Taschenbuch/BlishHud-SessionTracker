namespace SessionTracker.Models
{
    public class StatCategory
    {
        public StatCategoryType Type { get; set; } = StatCategoryType.Undefined;
        public int ApiId { get; set; }
        public int ApiPosition { get; set; } // called "Order" in API
        public LocalizedText Name { get; } = new LocalizedText(); // json is easier to read when this is the last property
    }
}
