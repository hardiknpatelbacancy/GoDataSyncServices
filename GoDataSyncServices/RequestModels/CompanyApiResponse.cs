using System.Text.Json.Serialization;

namespace GoDataSyncServices.RequestModels
{
    public class CompanyApiResponse
    {
        [JsonPropertyName("data")]
        public List<CompanyData> Data { get; set; }

    }

    public class CompanyData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("attributes")]
        public CompanyAttributes Attributes { get; set; }
    }

    public class CompanyAttributes
    {
        [JsonPropertyName("tenants_id")]
        public string TenantsId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
} 