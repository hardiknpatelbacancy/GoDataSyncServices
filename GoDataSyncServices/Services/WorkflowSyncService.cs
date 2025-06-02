namespace GoDataSyncServices.Services
{
    public class WorkflowSyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public WorkflowSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<WorkflowSyncService> logger,
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
                    var url = $"{_apiConfig.BaseUrl}/includego/tenants/{tenantId}/companies/{companyId}/workflows?page[number]={currentPage}&page[size]={_apiConfig.PageSize}";
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

                        foreach (var workflow in dataArray.EnumerateArray())
                        {
                            var attributes = workflow.GetProperty("attributes");
                            var workflowStates = attributes.GetProperty("workflow_states");

                            var workflowEntity = new
                            {
                                Id = ResponseHelper.GetNullableGuidSafe(workflow, "id"),
                                TenantsId = Guid.Parse(tenantId),
                                CompaniesId = Guid.Parse(companyId),
                                SummaryDescription = ResponseHelper.GetStringSafe(attributes, "summary_description"),
                                DetailDescription = ResponseHelper.GetStringSafe(attributes, "detail_description"),
                                SortOrder = ResponseHelper.GetNullableFloatSafe(attributes, "sort_order"),
                                Deleted = false,
                                CreatedAt = ResponseHelper.GetNullableDateTimeSafe(attributes, "createdAt")
                            };

                            await connection.ExecuteAsync(@"
                                INSERT INTO Workflows (
                                    id, tenants_id, companies_id, summary_description, detail_description,
                                    sort_order, accounts_id, deleted, workflow_state_completed, created_at
                                ) VALUES (
                                    @Id, @TenantsId, @CompaniesId, @SummaryDescription, @DetailDescription,
                                    @SortOrder, null, @Deleted, null, @CreatedAt
                                )", workflowEntity);

                            // Insert workflow states
                            foreach (var state in workflowStates.EnumerateArray())
                            {
                                var stateEntity = new 
                                {
                                    Id = Guid.NewGuid(),
                                    WorkflowsId = workflowEntity.Id,
                                    StateName = ResponseHelper.GetStringSafe(state, "name"),
                                    StateOrder = ResponseHelper.GetNullableInt32Safe(state, "id")
                                };

                                await connection.ExecuteAsync(@"
                                    INSERT INTO WorkflowStates (
                                        id, workflows_id, state_name, state_order
                                    ) VALUES (
                                        @Id, @WorkflowsId, @StateName, @StateOrder
                                    )", stateEntity);
                            }

                            totalRecordsSynced++;
                        }
                    }

                    currentPage++;
                } while (currentPage <= totalPages);

                return CreateSuccessResult("Workflows sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing workflows");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 