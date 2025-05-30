using System.Text.Json;
using GoDataSyncServices.Helper;
using GoDataSyncServices.Services.Base;
using GoDataSyncServices.Services.Configuration;
using GoDataSyncServices.Services.Interfaces;
using System.Data.SqlClient;
using Dapper;

namespace GoDataSyncServices.Services
{
    public class TaskSyncService : BaseSyncService
    {
        private readonly ApiConfiguration _apiConfig;

        public TaskSyncService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<TaskSyncService> logger,
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

                do
                {
                    var url = $"{_apiConfig.BaseUrl}/includego/tenants/{tenantId}/companies/{companyId}/tasks?page[number]={currentPage}&page[size]={_apiConfig.PageSize}";
                    var response = await httpClient.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return CreateErrorResult($"API request failed: {response.ReasonPhrase}");
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(content);
                    var dataArray = doc.RootElement.GetProperty("data");

                    if (dataArray.GetArrayLength() == 0)
                        break;

                    using (var connection = new SqlConnection(_connectionString))
                    {
                        await connection.OpenAsync();

                        foreach (var item in dataArray.EnumerateArray())
                        {
                            var attr = item.GetProperty("attributes");
                            var rel = item.GetProperty("relationships");

                            var taskEntity = new
                            {
                                Id = Guid.Parse(item.GetProperty("id").GetString()),
                                Name = ResponseHelper.GetStringSafe(attr, "name"),
                                TenantsId = Guid.Parse(tenantId),
                                CompaniesId = Guid.Parse(companyId),
                                ParentTasksId = ResponseHelper.GetNullableGuidSafe(attr, "parent_tasks_id"),
                                LocationsId = rel.TryGetProperty("locations", out var locs) && locs.GetProperty("data").GetArrayLength() > 0
                                    ? Guid.Parse(locs.GetProperty("data")[0].GetProperty("id").GetString())
                                    : (Guid?)null,
                                ProjectsId = Guid.Parse(rel.GetProperty("projects").GetProperty("data")[0].GetProperty("id").GetString()),
                                WorkflowsId = Guid.Parse(rel.GetProperty("workflows").GetProperty("data")[0].GetProperty("id").GetString()),
                                SummaryContractDescription = ResponseHelper.GetStringSafe(attr, "summary_contract_description"),
                                SummaryProductionDescription = ResponseHelper.GetStringSafe(attr, "summary_production_description"),
                                DetailContractDescription = ResponseHelper.GetStringSafe(attr, "detail_contract_description"),
                                DetailProductionDescription = ResponseHelper.GetStringSafe(attr, "detail_production_description"),
                                WorkflowState = ResponseHelper.GetNullableInt32Safe(attr, "workflow_state") ?? 0,
                                Priority = ResponseHelper.GetNullableFloatSafe(attr, "priority"),
                                CustomSort = ResponseHelper.GetStringSafe(attr, "custom_sort"),
                                Sequence = ResponseHelper.GetNullableFloatSafe(attr, "sequence"),
                                PromisedDate = ResponseHelper.GetNullableDateTimeSafe(attr, "promised_date"),
                                TargetStartDate = ResponseHelper.GetNullableDateTimeSafe(attr, "target_start_date"),
                                TargetEndDate = ResponseHelper.GetNullableDateTimeSafe(attr, "target_end_date"),
                                ActualEndDate = ResponseHelper.GetNullableDateTimeSafe(attr, "actual_end_date"),
                                ProductionClockHoursAllowedPerDay = ResponseHelper.GetNullableFloatSafe(attr, "production_clock_hours_allowed_per_day"),
                                StoryPoints = ResponseHelper.GetNullableFloatSafe(attr, "story_points"),
                                BudgetHoursTotal = ResponseHelper.GetNullableFloatSafe(attr, "budget_hours_total"),
                                BillingType = ResponseHelper.GetNullableInt32Safe(attr, "billing_type"),
                                BillingRateInCents = ResponseHelper.GetNullableFloatSafe(attr, "billing_rate_in_cents"),
                                BillingAmountInCents = ResponseHelper.GetNullableFloatSafe(attr, "billing_amount_in_cents"),
                                AccountsId = ResponseHelper.GetStringSafe(attr, "accounts_id"),
                                Deleted = ResponseHelper.GetNullableInt32Safe(attr, "deleted") == 1,
                                RecurringOptionsId = ResponseHelper.GetNullableGuidSafe(attr, "recurring_options_id"),
                                RecurringState = ResponseHelper.GetNullableInt32Safe(attr, "recurring_state"),
                                ProjectManagerMemberId = ResponseHelper.GetNullableGuidSafe(attr, "project_manager_member_id"),
                                SalesPersonMemberId = ResponseHelper.GetNullableGuidSafe(attr, "sales_person_member_id"),
                                EstimatorMemberId = ResponseHelper.GetNullableGuidSafe(attr, "estimator_member_id"),
                                PurchaserMemberId = ResponseHelper.GetNullableGuidSafe(attr, "purchaser_member_id"),
                                ProfitCenterId = ResponseHelper.GetNullableInt32Safe(attr, "profit_center_id"),
                                BranchId = ResponseHelper.GetNullableInt32Safe(attr, "branch_id"),
                                BillingBatchId = ResponseHelper.GetNullableInt32Safe(attr, "billing_batch_id"),
                                BudgetOccurrences = ResponseHelper.GetNullableInt32Safe(attr, "budget_occurrences"),
                                ExternalId = ResponseHelper.GetStringSafe(attr, "external_id"),
                                UpdatedAt = ResponseHelper.GetNullableDateTimeSafe(attr, "updated")
                            };

                            await connection.ExecuteAsync(@"
                                INSERT INTO Tasks (
                                    id, name, tenants_id, companies_id, parent_tasks_id, locations_id, projects_id, workflows_id,
                                    summary_contract_description, summary_production_description, detail_contract_description, detail_production_description,
                                    workflow_state, priority, custom_sort, sequence, promised_date, target_start_date, target_end_date, actual_end_date,
                                    production_clock_hours_allowed_per_day, story_points, budget_hours_total, billing_type, billing_rate_in_cents,
                                    billing_amount_in_cents, accounts_id, deleted, recurring_options_id, recurring_state,
                                    project_manager_member_id, sales_person_member_id, estimator_member_id, purchaser_member_id,
                                    profit_center_id, branch_id, billing_batch_id, budget_occurrences, external_id,
                                    updated_at
                                ) VALUES (
                                    @Id, @Name, @TenantsId, @CompaniesId, @ParentTasksId, @LocationsId, @ProjectsId, @WorkflowsId,
                                    @SummaryContractDescription, @SummaryProductionDescription, @DetailContractDescription, @DetailProductionDescription,
                                    @WorkflowState, @Priority, @CustomSort, @Sequence, @PromisedDate, @TargetStartDate, @TargetEndDate, @ActualEndDate,
                                    @ProductionClockHoursAllowedPerDay, @StoryPoints, @BudgetHoursTotal, @BillingType, @BillingRateInCents,
                                    @BillingAmountInCents, @AccountsId, @Deleted, @RecurringOptionsId, @RecurringState,
                                    @ProjectManagerMemberId, @SalesPersonMemberId, @EstimatorMemberId, @PurchaserMemberId,
                                    @ProfitCenterId, @BranchId, @BillingBatchId, @BudgetOccurrences, @ExternalId,
                                    @UpdatedAt
                                )", taskEntity);

                            totalRecordsSynced++;
                        }
                    }

                    currentPage++;
                } while (true);

                return CreateSuccessResult("Tasks sync completed successfully", totalRecordsSynced);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing tasks");
                return CreateErrorResult($"Internal server error: {ex.Message}");
            }
        }
    }
} 