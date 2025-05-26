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

        public SyncGOController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string not found.");
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _authToken = configuration["AuthToken"] ?? throw new Exception("AuthToken not found in configuration.");
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
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                int currentPage = 1;
                int pageSize = 100;
                int totalRecordsSynced = 0;

                do
                {
                    var url = $"{ApiBaseUrl}/includego/tenants/{tenants_id}/companies/{companies_id}/projects?page[number]={currentPage}&page[size]={pageSize}";
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
                            var attributes = item.GetProperty("attributes");
                            var relationships = item.GetProperty("relationships");

                            var external_project_id = attributes.GetProperty("external_project_id").GetString();
                            var external_division_id = attributes.TryGetProperty("external_division_id", out var edidProp) ? edidProp.GetString() : null;
                            var project_name = attributes.GetProperty("project_name").GetString();
                            var summary_description = attributes.TryGetProperty("summary_description", out var sumDesc) ? sumDesc.GetString() : null;
                            var detail_description = attributes.TryGetProperty("detail_description", out var detDesc) ? detDesc.GetString() : null;
                            var workflow_state = attributes.GetProperty("workflow_state").GetInt32();
                            var parent_projects_id = attributes.TryGetProperty("parent_projects_id", out var parentId) && !parentId.ValueKind.Equals(JsonValueKind.Null) ? Guid.Parse(parentId.GetString()) : (Guid?)null;
                            var project_manager_id = attributes.GetProperty("project_manager_id").GetString();
                            DateTime? start_date_target = attributes.TryGetProperty("start_date_target", out var startProp) &&
                              startProp.ValueKind != JsonValueKind.Null &&
                              !string.IsNullOrWhiteSpace(startProp.GetString())
    ? DateTime.Parse(startProp.GetString())
    : (DateTime?)null;
                            DateTime? end_date_target = attributes.TryGetProperty("end_date_target", out var endProp) &&
                            endProp.ValueKind != JsonValueKind.Null &&
                            !string.IsNullOrWhiteSpace(endProp.GetString())
    ? DateTime.Parse(endProp.GetString())
    : (DateTime?)null;
                            var resource_projects_id = Guid.Parse(attributes.GetProperty("resource_projects_id").GetString());
                            var travel_projects_id = Guid.Parse(attributes.GetProperty("travel_projects_id").GetString());
                            var divisions_id = attributes.TryGetProperty("divisions_id", out var divIdProp) ? int.Parse(divIdProp.GetString()) : (int?)null;
                            var clients_name = attributes.TryGetProperty("clients_name", out var cnProp) ? cnProp.GetString() : null;
                            var deleted = attributes.GetProperty("deleted").GetBoolean();
                            var branch_id = attributes.TryGetProperty("branch_id", out var branchIdProp) && branchIdProp.ValueKind != JsonValueKind.Null ? (int?)branchIdProp.GetInt32() : null;

                            var locations_id = relationships.GetProperty("locations").GetProperty("data")[0].GetProperty("id").GetString();
                            var workflows_id = relationships.GetProperty("workflows").GetProperty("data")[0].GetProperty("id").GetString();
                            var clients_id = relationships.GetProperty("clients").GetProperty("data")[0].GetProperty("id").GetString();

                            string sql = @"
        INSERT INTO Projects (
            id, tenants_id, companies_id, external_project_id, external_division_id, project_name,
            locations_id, summary_description, detail_description, workflows_id, workflow_state,
            clients_id, project_manager_id, parent_projects_id, start_date_target, end_date_target,
            resource_projects_id, travel_projects_id, divisions_id, clients_name, deleted
        )
        VALUES (
            @id, @tenants_id, @companies_id, @external_project_id, @external_division_id, @project_name,
            @locations_id, @summary_description, @detail_description, @workflows_id, @workflow_state,
            @clients_id, @project_manager_id, @parent_projects_id, @start_date_target, @end_date_target,
            @resource_projects_id, @travel_projects_id, @divisions_id, @clients_name, @deleted
        )";

                            var parameters = new
                            {
                                id,
                                tenants_id = Guid.Parse(tenants_id),
                                companies_id = Guid.Parse(companies_id),
                                external_project_id,
                                external_division_id,
                                project_name,
                                locations_id = Guid.Parse(locations_id),
                                summary_description,
                                detail_description,
                                workflows_id = Guid.Parse(workflows_id),
                                workflow_state,
                                clients_id = Guid.Parse(clients_id),
                                project_manager_id,
                                parent_projects_id,
                                start_date_target,
                                end_date_target,
                                resource_projects_id,
                                travel_projects_id,
                                divisions_id,
                                clients_name,
                                deleted
                            };

                            await db.ExecuteAsync(sql, parameters);
                            totalRecordsSynced++;
                        }
                    }


                } while (true);

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