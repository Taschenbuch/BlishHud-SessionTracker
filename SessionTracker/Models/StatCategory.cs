using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SessionTracker.Models
{
    public class StatCategory
    {
        public string Id { get; set; } = string.Empty;
        public LocalizedText Name { get; } = new LocalizedText(); // json is easier to read when this is the last property
        public List<string> SubCategoryIds { get; set; } = new List<string>();
        public List<string> StatIds { get; } = new List<string>();
        [JsonIgnore] public bool IsSuperCategory => SubCategoryIds.Any();
        [JsonIgnore] public bool IsSubCategory => !IsSuperCategory;
    }
}
