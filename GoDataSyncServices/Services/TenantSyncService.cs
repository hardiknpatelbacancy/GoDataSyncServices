using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using GoDataSyncServices.Models;
using GoDataSyncServices.RequestModels;
using GoDataSyncServices.Services.Base;
using GoDataSyncServices.Services.Configuration;
using GoDataSyncServices.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoDataSyncServices.Services
{
    public class TenantSyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public TenantSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<TenantSyncService> logger,
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
                int currentPage = 1;
                int totalRecordsSynced = 0;
                int totalPages = 1;

                do
                {
                    var url = $"{_apiConfig.BaseUrl}/admin/tenants?page[number]={currentPage}&page[size]={_apiConfig.PageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return CreateErrorResult($"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<TenantApiResponse>(content);

                    if (apiResponse?.Data == null || !apiResponse.Data.Any())
                    {
                        break;
                    }

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        foreach (var tenant in apiResponse.Data)
                        {
                            var tenantEntity = new
                            {
                                Id = Guid.Parse(tenant.Id),
                                Name = tenant.Attributes.Name,
                                OwnerEmail = tenant.Attributes.OwnerEmail,
                                OwnerName = tenant.Attributes.OwnerName,
                                CreatedAt = tenant.Attributes.CreatedAt
                            };

                            await connection.ExecuteAsync(@"
                                IF NOT EXISTS (SELECT 1 FROM [dbo].[Tenants] WHERE id = @Id)
                                BEGIN
                                    INSERT INTO [dbo].[Tenants] (id, name, owner_email, owner_name, created_at)
                                    VALUES (@Id, @Name, @OwnerEmail, @OwnerName, @CreatedAt)
                                END", tenantEntity);
                        }
                    }

                    totalRecordsSynced += apiResponse.Data.Count;
                    currentPage++;
                    totalPages = apiResponse.Meta.TotalPages;

                } while (currentPage <= totalPages);

                return CreateSuccessResult("Tenants sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing tenants");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 