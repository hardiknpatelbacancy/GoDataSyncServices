using GoDataSyncServices.RequestModels;

namespace GoDataSyncServices.Services
{
    public class CompanySyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public CompanySyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<CompanySyncService> logger,
            ApiConfiguration apiConfig)
            : base(httpClientFactory, configuration, logger)
        {
            _apiConfig = apiConfig;
        }

        public override async Task<SyncResult> SyncAsync(string tenantId = null, string companyId = null)
        {
            try
            {
                var httpClient = CreateHttpClient();
                var url = $"{_apiConfig.BaseUrl}/admin/companies";
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return CreateErrorResult($"API request failed: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CompanyApiResponse>(content);

                if (apiResponse?.Data == null || !apiResponse.Data.Any())
                {
                    return CreateErrorResult("No data found in the API response.");
                }

                int totalRecordsSynced = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    foreach (var company in apiResponse.Data)
                    {
                        var companyEntity = new
                        {
                            Id = Guid.Parse(company.Id),
                            TenantId = Guid.Parse(company.Attributes.TenantsId),
                            Name = company.Attributes.Name,
                            Enabled = company.Attributes.Enabled,
                            IsDeleted = company.Attributes.IsDeleted,
                            CreatedAt = company.Attributes.CreatedAt
                        };

                        await connection.ExecuteAsync(@"
                            IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE id = @Id)
                            BEGIN
                                INSERT INTO [dbo].[Companies] (id, tenant_id, name, enabled, is_deleted, created_at)
                                VALUES (@Id, @TenantId, @Name, @Enabled, @IsDeleted, @CreatedAt)
                            END", companyEntity);

                        totalRecordsSynced++;
                    }
                }

                return CreateSuccessResult("Companies sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing companies");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 