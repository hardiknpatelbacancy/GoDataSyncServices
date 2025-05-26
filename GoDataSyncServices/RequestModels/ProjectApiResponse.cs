using System.Text.Json.Serialization;

namespace GoDataSyncServices.RequestModels
{
    public class ProjectApiResponse
    {
        [JsonPropertyName("data")]
        public List<ProjectData> Data { get; set; }
    }

    public class ProjectData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("attributes")]
        public ProjectAttributes Attributes { get; set; }

        [JsonPropertyName("relationships")]
        public ProjectRelationships Relationships { get; set; }
    }

    public class ProjectAttributes
    {
        [JsonPropertyName("external_project_id")]
        public string ExternalProjectId { get; set; }

        [JsonPropertyName("external_division_id")]
        public string ExternalDivisionId { get; set; }

        [JsonPropertyName("project_name")]
        public string ProjectName { get; set; }

        [JsonPropertyName("summary_description")]
        public string SummaryDescription { get; set; }

        [JsonPropertyName("detail_description")]
        public string DetailDescription { get; set; }

        [JsonPropertyName("workflow_state")]
        public int WorkflowState { get; set; }

        [JsonPropertyName("project_manager_id")]
        public string ProjectManagerId { get; set; }

        [JsonPropertyName("start_date_target")]
        public DateTime? StartDateTarget { get; set; }

        [JsonPropertyName("end_date_target")]
        public DateTime? EndDateTarget { get; set; }

        [JsonPropertyName("resource_projects_id")]
        public string ResourceProjectsId { get; set; }

        [JsonPropertyName("resource_tasks_id")]
        public string ResourceTasksId { get; set; }

        [JsonPropertyName("travel_projects_id")]
        public string TravelProjectsId { get; set; }

        [JsonPropertyName("travel_tasks_id")]
        public string TravelTasksId { get; set; }

        [JsonPropertyName("divisions_id")]
        public string? DivisionsId { get; set; }

        [JsonPropertyName("branch_id")]
        public int? BranchId { get; set; }

        [JsonPropertyName("default_tax_authority_id")]
        public int? DefaultTaxAuthorityId { get; set; }

        [JsonPropertyName("clients_name")]
        public string ClientsName { get; set; }

        [JsonPropertyName("parent_projects_id")]
        public Guid? parent_projects_id { get; set; }

        [JsonPropertyName("deleted")]
        public bool? deleted { get; set; }
    }

    public class ProjectRelationships
    {
        [JsonPropertyName("tenants")]
        public RelationshipData Tenants { get; set; }

        [JsonPropertyName("companies")]
        public RelationshipData Companies { get; set; }

        [JsonPropertyName("locations")]
        public RelationshipData Locations { get; set; }

        [JsonPropertyName("clients")]
        public RelationshipData Clients { get; set; }

        [JsonPropertyName("workflows")]
        public RelationshipData Workflows { get; set; }
    }

    public class RelationshipData
    {
        [JsonPropertyName("data")]
        public List<RelationshipItem> Data { get; set; }
    }

    public class RelationshipItem
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
} 