using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Net.Http.Headers;
using System.Text.Json;
using GoDataSyncServices.RequestModels;

namespace GoDataSyncServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncGOController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private const string ApiBaseUrl = "https://dev-portal-api.include.com";
        private readonly string _authToken;
        private readonly ILogger<SyncGOController> _logger;

        public SyncGOController(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<SyncGOController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found.");
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _authToken = configuration["AuthToken"] ?? throw new Exception("AuthToken not found in configuration.");
            _logger = logger;
        }

        [HttpPost("tenants")]
        public async Task<IActionResult> SyncTenants()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int currentPage = 1;
                int pageSize = 100; // Adjust page size as needed
                int totalPages = 1;
                int totalRecordsSynced = 0;

                do
                {
                    var url = $"{ApiBaseUrl}/admin/tenants?page[number]={currentPage}&page[size]={pageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<TenantApiResponse>(content);

                    if (apiResponse?.Data == null || !apiResponse.Data.Any())
                    {
                        break;
                    }

                    // Update total pages from first response
                    if (currentPage == 1)
                    {
                        totalPages = apiResponse.Meta.TotalPages;
                    }

                    using (IDbConnection db = new SqlConnection(_connectionString))
                    {
                        foreach (var tenant in apiResponse.Data)
                        {
                            var sql = @"
                                IF NOT EXISTS (SELECT 1 FROM [dbo].[Tenants] WHERE Id = @Id)
                                BEGIN
                                    INSERT INTO [dbo].[Tenants] (Id, name, owner_email, owner_name, created_at, enabled)
                                    VALUES (@Id, @Name, @OwnerEmail, @OwnerName, @CreatedAt, 1)
                                END";

                            await db.ExecuteAsync(sql, new
                            {
                                Id = Guid.Parse(tenant.Id),
                                Name = tenant.Attributes.Name,
                                OwnerEmail = tenant.Attributes.OwnerEmail,
                                OwnerName = tenant.Attributes.OwnerName,
                                CreatedAt = tenant.Attributes.CreatedAt
                            });
                        }
                    }

                    totalRecordsSynced += apiResponse.Data.Count;
                    currentPage++;

                } while (currentPage <= totalPages);

                return Ok(new
                {
                    Message = "Sync completed successfully",
                    TotalRecordsSynced = totalRecordsSynced
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("companies")]
        public async Task<IActionResult> SyncCompanies()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"{ApiBaseUrl}/admin/companies";
                var response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CompanyApiResponse>(content);

                if (apiResponse?.Data == null || !apiResponse.Data.Any())
                {
                    throw new Exception("No data found in the API response.");
                }

                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    foreach (var company in apiResponse.Data)
                    {
                        var sql = @"
                                IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE id = @Id)
                                BEGIN
                                    INSERT INTO [dbo].[Companies] (id, tenant_id, name, enabled, is_deleted, created_at)
                                    VALUES (@Id, @TenantId, @Name, @Enabled, @IsDeleted, @CreatedAt)
                                END
                                ELSE
                                BEGIN
                                    UPDATE [dbo].[Companies]
                                    SET tenant_id = @TenantId,
                                        name = @Name,
                                        enabled = @Enabled,
                                        is_deleted = @IsDeleted,
                                        updated_at = GETDATE()
                                    WHERE id = @Id
                                END";

                        await db.ExecuteAsync(sql, new
                        {
                            Id = Guid.Parse(company.Id),
                            TenantId = Guid.Parse(company.Attributes.TenantsId),
                            Name = company.Attributes.Name,
                            Enabled = company.Attributes.Enabled,
                            IsDeleted = company.Attributes.IsDeleted,
                            CreatedAt = company.Attributes.CreatedAt
                        });
                    }
                }

                return Ok(new
                {
                    Message = "Companies sync completed successfully",
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("projects")]
        public async Task<IActionResult> SyncProjects([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            try
            {
                if (string.IsNullOrEmpty(tenants_id) || string.IsNullOrEmpty(companies_id))
                {
                    return BadRequest("Tenants ID and Companies ID are required");
                }

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int currentPage = 1;
                int pageSize = 100;
                int totalRecordsSynced = 0;
                int totalPages = 1;

                do
                {
                    var url = $"{ApiBaseUrl}/includego/tenants/{tenants_id}/companies/{companies_id}/projects?page[number]={currentPage}&page[size]={pageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    using (IDbConnection db = new SqlConnection(_connectionString))
                    {
                        using var doc = JsonDocument.Parse(content);
                        var dataArray = doc.RootElement.GetProperty("data");
                        var meta = doc.RootElement.GetProperty("meta");
                        totalPages = meta.GetProperty("total_pages").GetInt32();

                        if (dataArray.GetArrayLength() == 0)
                            break;

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

                            var projects = new
                            {
                                Id = Guid.Parse(project.GetProperty("id").GetString()),
                                TenantsId = Guid.Parse(tenants_id),
                                CompaniesId = Guid.Parse(companies_id),
                                ExternalProjectId = ResponseHelper.GetStringSafe(attributes, "external_project_id"),
                                ExternalDivisionId = ResponseHelper.GetStringSafe(attributes, "external_division_id"),
                                ProjectName = ResponseHelper.GetStringSafe(attributes, "project_name"),
                                LocationsId = locationId != null ? Guid.Parse(locationId) : (Guid?)null,
                                SummaryDescription = ResponseHelper.GetStringSafe(attributes, "summary_description"),
                                DetailDescription = ResponseHelper.GetStringSafe(attributes, "detail_description"),
                                WorkflowsId = workflowId != null ? Guid.Parse(workflowId) : Guid.Empty,
                                WorkflowState = attributes.TryGetProperty("workflow_state", out var ws) ? ws.GetInt32() : 0, // Default value
                                ClientsId = clientId != null ? Guid.Parse(clientId) : Guid.Empty,
                                ProjectManagerId = ResponseHelper.GetStringSafe(attributes, "project_manager_id"),
                                ParentProjectsId = ResponseHelper.GetNullableGuidSafe(attributes, "parent_projects_id"),
                                StartDateTarget = ResponseHelper.GetNullableDateTimeSafe(attributes, "start_date_target"),
                                EndDateTarget = ResponseHelper.GetNullableDateTimeSafe(attributes, "end_date_target"),
                                ResourceProjectsId = ResponseHelper.GetNullableGuidSafe(attributes, "resource_projects_id"),
                                TravelProjectsId = ResponseHelper.GetNullableGuidSafe(attributes, "travel_projects_id"),
                                ClientsName = ResponseHelper.GetStringSafe(attributes, "clients_name"),
                                Deleted = attributes.TryGetProperty("deleted", out var del) ? del.GetBoolean() : false, // Default value
                                ResourceTasksId = ResponseHelper.GetNullableGuidSafe(attributes, "resource_tasks_id"),
                                TravelTasksId = ResponseHelper.GetNullableGuidSafe(attributes, "travel_tasks_id"),
                                DivisionsId = ResponseHelper.GetNullableInt32Safe(attributes, "divisions_id"),
                                BranchId = ResponseHelper.GetNullableInt32Safe(attributes, "branch_id"),
                                CreatedAt = ResponseHelper.GetNullableDateTimeSafe(attributes, "created_at"),
                                UpdatedAt = ResponseHelper.GetNullableDateTimeSafe(attributes, "updated_at")
                            };

                            string insertQuery = @"INSERT INTO Projects (
                                id, tenants_id, companies_id, external_project_id, external_division_id,
                                project_name, locations_id, summary_description, detail_description,
                                workflows_id, workflow_state, clients_id, project_manager_id,
                                parent_projects_id, start_date_target, end_date_target,
                                resource_tasks_id,travel_tasks_id, divisions_id, branch_id,
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
                            )";

                            await db.ExecuteAsync(insertQuery, projects);
                            totalRecordsSynced++;
                        }

                    }

                    currentPage++;
                } while (currentPage <= totalPages);

                return Ok(new
                {
                    Message = "Projects sync completed successfully",
                    TotalRecordsSynced = totalRecordsSynced
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("tasks")]
        public async Task<IActionResult> SyncTasks([FromQuery] string tenants_id, [FromQuery] string companies_id)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int currentPage = 1;
                int pageSize = 100;
                int totalRecordsSynced = 0;

                do
                {
                    var url = $"{ApiBaseUrl}/includego/tenants/{tenants_id}/companies/{companies_id}/tasks?page[number]={currentPage}&page[size]={pageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode, $"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    using var doc = JsonDocument.Parse(content);
                    var dataArray = doc.RootElement.GetProperty("data");

                    if (dataArray.GetArrayLength() == 0)
                        break;

                    using (IDbConnection db = new SqlConnection(_connectionString))
                    {
                        foreach (var item in dataArray.EnumerateArray())
                        {
                            var id = Guid.Parse(item.GetProperty("id").GetString());

                            var attr = item.GetProperty("attributes");
                            var rel = item.GetProperty("relationships");

                            var task = new
                            {
                                Id = id,
                                Name = attr.GetProperty("name").GetString(),
                                TenantsId = Guid.Parse(tenants_id),
                                CompaniesId = Guid.Parse(companies_id),
                                ParentTasksId = attr.TryGetProperty("parent_tasks_id", out var parentTaskVal) &&
                parentTaskVal.ValueKind == JsonValueKind.String &&
                Guid.TryParse(parentTaskVal.GetString(), out var parsedParentTaskId)
                ? parsedParentTaskId
                : (Guid?)null,
                                LocationsId = rel.TryGetProperty("locations", out var locs) && locs.GetProperty("data").GetArrayLength() > 0
                                    ? Guid.Parse(locs.GetProperty("data")[0].GetProperty("id").GetString())
                                    : (Guid?)null,
                                ProjectsId = Guid.Parse(rel.GetProperty("projects").GetProperty("data")[0].GetProperty("id").GetString()),
                                WorkflowsId = Guid.Parse(rel.GetProperty("workflows").GetProperty("data")[0].GetProperty("id").GetString()),
                                SummaryContractDescription = attr.GetProperty("summary_contract_description").GetString(),
                                SummaryProductionDescription = attr.GetProperty("summary_production_description").GetString(),
                                DetailContractDescription = attr.GetProperty("detail_contract_description").GetString(),
                                DetailProductionDescription = attr.GetProperty("detail_production_description").GetString(),
                                WorkflowState = attr.GetProperty("workflow_state").GetInt32(),
                                Priority = attr.TryGetProperty("priority", out var priorityVal) ? (double?)priorityVal.GetDouble() : null,
                                CustomSort = attr.GetProperty("custom_sort").GetString(),
                                Sequence = attr.TryGetProperty("sequence", out var seqVal) ? (double?)seqVal.GetDouble() : null,
                                PromisedDate = attr.TryGetProperty("promised_date", out var pd) && pd.ValueKind == JsonValueKind.String ? (DateTime?)DateTime.Parse(pd.GetString()) : null,
                                TargetStartDate = attr.TryGetProperty("target_start_date", out var tsd) && tsd.ValueKind == JsonValueKind.String ? (DateTime?)DateTime.Parse(tsd.GetString()) : null,
                                TargetEndDate = attr.TryGetProperty("target_end_date", out var ted) && ted.ValueKind == JsonValueKind.String ? (DateTime?)DateTime.Parse(ted.GetString()) : null,
                                ActualEndDate = attr.TryGetProperty("actual_end_date", out var aed) && aed.ValueKind == JsonValueKind.String ? (DateTime?)DateTime.Parse(aed.GetString()) : null,
                                ProductionClockHoursAllowedPerDay = attr.TryGetProperty("production_clock_hours_allowed_per_day", out var pch) ? (double?)pch.GetDouble() : null,
                                StoryPoints = attr.TryGetProperty("story_points", out var sp) ? (double?)sp.GetDouble() : null,
                                BudgetHoursTotal = attr.TryGetProperty("budget_hours_total", out var bht) ? (double?)bht.GetDouble() : null,
                                BillingType = attr.TryGetProperty("billing_type", out var bt) ? (int?)bt.GetInt32() : null,
                                BillingRateInCents = attr.TryGetProperty("billing_rate_in_cents", out var br) ? (decimal?)br.GetDecimal() : null,
                                BillingAmountInCents = attr.TryGetProperty("billing_amount_in_cents", out var ba) ? (decimal?)ba.GetDecimal() : null,
                                AccountsId = attr.TryGetProperty("accounts_id", out var accounts_id),
                                Deleted = attr.TryGetProperty("deleted", out var deleted),
                                RecurringOptionsId = attr.TryGetProperty("recurring_options_id", out var ro) && ro.ValueKind == JsonValueKind.String ? (Guid?)Guid.Parse(ro.GetString()) : null,
                                RecurringState = attr.TryGetProperty("recurring_state", out var rs) ? (int?)rs.GetInt32() : null,
                                ProjectManagerMemberId = attr.TryGetProperty("project_manager_member_id", out var pm) && pm.ValueKind == JsonValueKind.String ? (Guid?)Guid.Parse(pm.GetString()) : null,
                                SalesPersonMemberId = attr.TryGetProperty("sales_person_member_id", out var spm) && spm.ValueKind == JsonValueKind.String ? (Guid?)Guid.Parse(spm.GetString()) : null,
                                EstimatorMemberId = attr.TryGetProperty("estimator_member_id", out var em) && em.ValueKind == JsonValueKind.String ? (Guid?)Guid.Parse(em.GetString()) : null,
                                PurchaserMemberId = attr.TryGetProperty("purchaser_member_id", out var pmem) && pmem.ValueKind == JsonValueKind.String ? (Guid?)Guid.Parse(pmem.GetString()) : null,
                                ProfitCenterId = attr.TryGetProperty("profit_center_id", out var pcid) && pcid.ValueKind == JsonValueKind.Number ? (int?)pcid.GetInt32() : null,
                                BranchId = attr.TryGetProperty("branch_id", out var bid) && bid.ValueKind == JsonValueKind.Number ? (int?)bid.GetInt32() : null,
                                BillingBatchId = attr.TryGetProperty("billing_batch_id", out var bbid) && bbid.ValueKind == JsonValueKind.Number ? (int?)bbid.GetInt32() : null,
                                BudgetOccurrences = attr.TryGetProperty("budget_occurrences", out var bo) && bo.ValueKind == JsonValueKind.Number ? (int?)bo.GetInt32() : null,
                                ExternalId = attr.GetProperty("external_id").GetString(),
                                UpdatedAt = attr.TryGetProperty("updated", out var updatedAt) && updatedAt.ValueKind == JsonValueKind.String ? (DateTime?)DateTime.Parse(updatedAt.GetString()) : null
                            };

                            string insertQuery = @"
                        INSERT INTO dbo.Tasks (
                            id, name, tenants_id, companies_id, parent_tasks_id, locations_id, projects_id, workflows_id,
                            summary_contract_description, summary_production_description, detail_contract_description, detail_production_description,
                            workflow_state, priority, custom_sort, sequence, promised_date, target_start_date, target_end_date, actual_end_date,
                            production_clock_hours_allowed_per_day, story_points, budget_hours_total, billing_type, billing_rate_in_cents,
                            billing_amount_in_cents, accounts_id, deleted, recurring_options_id, recurring_state,
                            project_manager_member_id, sales_person_member_id, estimator_member_id, purchaser_member_id,
                            profit_center_id, branch_id, billing_batch_id, budget_occurrences, external_id,
                            updated_at)
                        VALUES (
                            @Id, @Name, @TenantsId, @CompaniesId, @ParentTasksId, @LocationsId, @ProjectsId, @WorkflowsId,
                            @SummaryContractDescription, @SummaryProductionDescription, @DetailContractDescription, @DetailProductionDescription,
                            @WorkflowState, @Priority, @CustomSort, @Sequence, @PromisedDate, @TargetStartDate, @TargetEndDate, @ActualEndDate,
                            @ProductionClockHoursAllowedPerDay, @StoryPoints, @BudgetHoursTotal, @BillingType, @BillingRateInCents,
                            @BillingAmountInCents, @AccountsId, @Deleted, @RecurringOptionsId, @RecurringState,
                            @ProjectManagerMemberId, @SalesPersonMemberId, @EstimatorMemberId, @PurchaserMemberId,
                            @ProfitCenterId, @BranchId, @BillingBatchId, @BudgetOccurrences, @ExternalId,
                            @UpdatedAt
                        );";

                            await db.ExecuteAsync(insertQuery, task);
                            totalRecordsSynced++;
                        }
                    }

                    currentPage++;

                } while (true);

                return Ok(new
                {
                    Message = "Tasks sync completed successfully",
                    TotalRecordsSynced = totalRecordsSynced
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}


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
}

  