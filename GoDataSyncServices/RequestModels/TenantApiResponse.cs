using System.Text.Json.Serialization;

namespace GoDataSyncServices.RequestModels
{
    public class TenantApiResponse
    {
        [JsonPropertyName("data")]
        public List<TenantData> Data { get; set; }

        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; }
    }

    public class TenantData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("attributes")]
        public TenantAttributes Attributes { get; set; }
    }

    public class TenantAttributes
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("owner_email")]
        public string OwnerEmail { get; set; }

        [JsonPropertyName("owner_name")]
        public string OwnerName { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class MetaData
    {
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_items")]
        public int TotalItems { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
} 