namespace GoDataSyncServices.Services
{
    public class ProjectSyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public ProjectSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<ProjectSyncService> logger,
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
                    var url = $"{_apiConfig.BaseUrl}/includego/tenants/{tenantId}/companies/{companyId}/projects?page[number]={currentPage}&page[size]={_apiConfig.PageSize}";
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

                        foreach (var project in dataArray.EnumerateArray())
                        {
                            var attributes = project.GetProperty("attributes");
                            var relationships = project.GetProperty("relationships");

                            var locationsData = relationships.GetProperty("locations").GetProperty("data");
                            var workflowsData = relationships.GetProperty("workflows").GetProperty("data");
                            var clientsData = relationships.GetProperty("clients").GetProperty("data");

                            var locationId = locationsData.GetArrayLength() > 0 ?
                                locationsData[0].GetProperty("id").GetString() : null;
                            var workflowId = workflowsData.GetArrayLength() > 0 ?
                                workflowsData[0].GetProperty("id").GetString() : null;
                            var clientId = clientsData.GetArrayLength() > 0 ?
                                clientsData[0].GetProperty("id").GetString() : null;

                            var projectEntity = new
                            {
                                Id = Guid.Parse(project.GetProperty("id").GetString()),
                                TenantsId = Guid.Parse(tenantId),
                                CompaniesId = Guid.Parse(companyId),
                                ExternalProjectId = ResponseHelper.GetStringSafe(attributes, "external_project_id"),
                                ExternalDivisionId = ResponseHelper.GetStringSafe(attributes, "external_division_id"),
                                ProjectName = ResponseHelper.GetStringSafe(attributes, "project_name"),
                                LocationsId = locationId != null ? Guid.Parse(locationId) : (Guid?)null,
                                SummaryDescription = ResponseHelper.GetStringSafe(attributes, "summary_description"),
                                DetailDescription = ResponseHelper.GetStringSafe(attributes, "detail_description"),
                                WorkflowsId = workflowId != null ? Guid.Parse(workflowId) : Guid.Empty,
                                WorkflowState = attributes.TryGetProperty("workflow_state", out var ws) ? ws.GetInt32() : 0,
                                ClientsId = clientId != null ? Guid.Parse(clientId) : Guid.Empty,
                                ProjectManagerId = ResponseHelper.GetStringSafe(attributes, "project_manager_id"),
                                ParentProjectsId = ResponseHelper.GetNullableGuidSafe(attributes, "parent_projects_id"),
                                StartDateTarget = ResponseHelper.GetNullableDateTimeSafe(attributes, "start_date_target"),
                                EndDateTarget = ResponseHelper.GetNullableDateTimeSafe(attributes, "end_date_target"),
                                ResourceProjectsId = ResponseHelper.GetNullableGuidSafe(attributes, "resource_projects_id"),
                                TravelProjectsId = ResponseHelper.GetNullableGuidSafe(attributes, "travel_projects_id"),
                                ClientsName = ResponseHelper.GetStringSafe(attributes, "clients_name"),
                                Deleted = attributes.TryGetProperty("deleted", out var del) ? del.GetBoolean() : false,
                                ResourceTasksId = ResponseHelper.GetNullableGuidSafe(attributes, "resource_tasks_id"),
                                TravelTasksId = ResponseHelper.GetNullableGuidSafe(attributes, "travel_tasks_id"),
                                DivisionsId = ResponseHelper.GetNullableInt32Safe(attributes, "divisions_id"),
                                BranchId = ResponseHelper.GetNullableInt32Safe(attributes, "branch_id"),
                                CreatedAt = ResponseHelper.GetNullableDateTimeSafe(attributes, "created_at"),
                                UpdatedAt = ResponseHelper.GetNullableDateTimeSafe(attributes, "updated_at")
                            };

                            await connection.ExecuteAsync(@"
                                INSERT INTO Projects (
                                    id, tenants_id, companies_id, external_project_id, external_division_id,
                                    project_name, locations_id, summary_description, detail_description,
                                    workflows_id, workflow_state, clients_id, project_manager_id,
                                    parent_projects_id, start_date_target, end_date_target,
                                    resource_tasks_id, travel_tasks_id, divisions_id, branch_id,
                                    resource_projects_id, travel_projects_id, clients_name,
                                    deleted, created_at, updated_at
                                ) VALUES (
                                    @Id, @TenantsId, @CompaniesId, @ExternalProjectId, @ExternalDivisionId,
                                    @ProjectName, @LocationsId, @SummaryDescription, @DetailDescription,
                                    @WorkflowsId, @WorkflowState, @ClientsId, @ProjectManagerId,
                                    @ParentProjectsId, @StartDateTarget, @EndDateTarget,
                                    @ResourceTasksId, @TravelTasksId, @DivisionsId, @BranchId,
                                    @ResourceProjectsId, @TravelProjectsId, @ClientsName,
                                    @Deleted, @CreatedAt, @UpdatedAt
                                )", projectEntity);

                            totalRecordsSynced++;
                        }
                    }

                    currentPage++;
                } while (currentPage <= totalPages);

                return CreateSuccessResult("Projects sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing projects");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 