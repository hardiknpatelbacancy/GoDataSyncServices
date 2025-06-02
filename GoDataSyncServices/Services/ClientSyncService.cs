namespace GoDataSyncServices.Services
{
    public class ClientSyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public ClientSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ClientSyncService> logger,
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
                    var url = $"{_apiConfig.BaseUrl}/payments/tenants/{tenantId}/companies/{companyId}/clients?page[number]={currentPage}&page[size]={_apiConfig.PageSize}";
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

                        foreach (var client in dataArray.EnumerateArray())
                        {
                            var attributes = client.GetProperty("attributes");

                            var clientEntity = new
                            {
                                Id = ResponseHelper.GetNullableGuidSafe(client, "id"),
                                TenantId = Guid.Parse(tenantId),
                                CompanyId = Guid.Parse(companyId),
                                Name = ResponseHelper.GetStringSafe(attributes, "client_name"),
                                CreatedAt = (DateTime?)null,
                                UpdatedAt = (DateTime?)null,
                                ClientExternalId = ResponseHelper.GetStringSafe(attributes, "external_client_id"),
                                BillEmail = ResponseHelper.GetStringSafe(attributes, "bill_email"),
                                AddressLine1 = ResponseHelper.GetStringSafe(attributes, "address_line1"),
                                AddressLine2 = ResponseHelper.GetStringSafe(attributes, "address_line2"),
                                BillAddressLine1 = ResponseHelper.GetStringSafe(attributes, "bill_address_line_1"),
                                BillAddressLine2 = ResponseHelper.GetStringSafe(attributes, "bill_address_line_2")
                            };

                            await connection.ExecuteAsync(@"
                                INSERT INTO [dbo].[Clients] (
                                    [id], [tenant_id],[company_id],[name],[created_at],[updated_at],
                                    [client_external_id],[bill_email],[address_line1],[address_line2],[bill_address_line_1],[bill_address_line_2]
                                )
                                VALUES (
                                    @Id,@TenantId,@CompanyId,@Name,@CreatedAt,@UpdatedAt,
                                    @ClientExternalId,@BillEmail,@AddressLine1,@AddressLine2,@BillAddressLine1,@BillAddressLine2
                                );", clientEntity);

                            totalRecordsSynced++;
                        }
                    }

                    currentPage++;
                } while (currentPage <= totalPages);

                return CreateSuccessResult("Clients sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing clients");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 