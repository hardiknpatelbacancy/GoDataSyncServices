using System.Text.Json;
using GoDataSyncServices.Helper;
using GoDataSyncServices.Services.Base;
using GoDataSyncServices.Services.Configuration;
using GoDataSyncServices.Services.Interfaces;
using System.Data.SqlClient;
using Dapper;

namespace GoDataSyncServices.Services
{
    public class LocationSyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public LocationSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<LocationSyncService> logger,
            ApiConfiguration apiConfig)
            : base(httpClientFactory, configuration, logger)
        {
            _apiConfig = apiConfig;
        }

        public override async Task<SyncResult> SyncAsync(string tenantId = null, string companyId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(companyId))
                {
                    return CreateErrorResult("Tenants ID and Companies ID are required");
                }

                var httpClient = CreateHttpClient();
                int currentPage = 1;
                int totalRecordsSynced = 0;
                int totalPages = 1;

                do
                {
                    var url = $"{_apiConfig.BaseUrl}/includego/tenants/{tenantId}/companies/{companyId}/locations?page[number]={currentPage}&page[size]={_apiConfig.PageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return CreateErrorResult($"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    var dataArray = doc.RootElement.GetProperty("data");
                    var meta = doc.RootElement.GetProperty("meta");
                    totalPages = meta.GetProperty("total_pages").GetInt32();

                    if (dataArray.GetArrayLength() == 0)
                        break;

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        foreach (var location in dataArray.EnumerateArray())
                        {
                            var attributes = location.GetProperty("attributes");

                            var locationEntity = new
                            {
                                Id = ResponseHelper.GetNullableGuidSafe(location, "id"),
                                Address = ResponseHelper.GetStringSafe(attributes, "address"),
                                TenantsId = Guid.Parse(tenantId),
                                CompaniesId = Guid.Parse(companyId),
                                Latitude = ResponseHelper.GetNullableFloatSafe(attributes, "latitude"),
                                Longitude = ResponseHelper.GetNullableFloatSafe(attributes, "longitude"),
                                Deleted = false,
                                CreatedAt = ResponseHelper.GetNullableDateTimeSafe(attributes, "createdAt")
                            };

                            await connection.ExecuteAsync(@"
                                INSERT INTO Locations (
                                    id, address, tenants_id, companies_id, latitude, longitude,
                                    accounts_id, deleted, created_at
                                ) VALUES (
                                    @Id, @Address, @TenantsId, @CompaniesId, @Latitude, @Longitude,
                                    null, @Deleted, @CreatedAt
                                )", locationEntity);

                            totalRecordsSynced++;
                        }
                    }

                    currentPage++;
                } while (currentPage <= totalPages);

                return CreateSuccessResult("Locations sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing locations");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 