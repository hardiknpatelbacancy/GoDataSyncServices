using System.Text.Json;

namespace GoDataSyncServices.Helper
{
    public static class ResponseHelper
    {
        // Helper methods to safely retrieve values
        public static string GetStringSafe(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return prop.GetString();
            return null;
        }

        public static Guid? GetNullableGuidSafe(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return Guid.Parse(prop.GetString());
            return null;
        }

        public static int? GetNullableInt32Safe(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
                return null;

            try
            {
                return prop.ValueKind switch
                {
                    JsonValueKind.Number => prop.GetInt32(),
                    JsonValueKind.String => int.Parse(prop.GetString()),
                    _ => throw new FormatException($"Invalid format for {propertyName}")
                };
            }
            catch (FormatException ex)
            {
                // Handle invalid format (log, use default, etc.)
                return null; // or throw custom exception
            }
        }

        public static DateTime? GetNullableDateTimeSafe(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
                return DateTime.Parse(prop.GetString());
            return null;
        }

        public static float? GetNullableFloatSafe(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
                return null;

            try
            {
                return prop.ValueKind switch
                {
                    JsonValueKind.Number => (float)prop.GetDouble(),
                    JsonValueKind.String => float.Parse(prop.GetString()),
                    _ => null
                };
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
