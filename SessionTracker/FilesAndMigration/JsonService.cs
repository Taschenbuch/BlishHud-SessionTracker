using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using SessionTracker.Models;
using Newtonsoft.Json.Linq;

namespace SessionTracker.Files
{
    public static class JsonService
    {
        public static string SerializeModelToJson(Model model) // public for JsonConverter project
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            return JsonConvert.SerializeObject(model, jsonSerializerSettings);
        }

        // https://stackoverflow.com/a/47269811
        // Note: to avoid triggering a clone of the existing property's value,
        // we need to save a reference to it and then null out property.Value
        // before adding the value to the new JProperty.  
        // Thanks to @dbc for the suggestion.
        public static void Rename(this JProperty property, string newName)
        {
            var existingValue = property.Value;
            property.Value = null;
            var newProperty = new JProperty(newName, existingValue);
            property.Replace(newProperty);
        }
    }
}
