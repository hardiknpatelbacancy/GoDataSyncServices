using System;

namespace GoDataSyncServices.Models
{
    public class Projects
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public string ExternalProjectId { get; set; }
        public string ExternalDivisionId { get; set; }
        public string ProjectName { get; set; }
        public Guid? LocationsId { get; set; }
        public string SummaryDescription { get; set; }
        public string DetailDescription { get; set; }
        public Guid WorkflowsId { get; set; }
        public int WorkflowState { get; set; }
        public Guid ClientsId { get; set; }
        public string ProjectManagerId { get; set; }
        public Guid? ParentProjectsId { get; set; }
        public DateTime? StartDateTarget { get; set; }
        public DateTime? EndDateTarget { get; set; }
        public string AccountsId { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ResourceProjectsId { get; set; }
        public Guid? ResourceTasksId { get; set; }
        public Guid? TravelProjectsId { get; set; }
        public Guid? TravelTasksId { get; set; }
        public int? DivisionsId { get; set; }
        public int? BranchId { get; set; }
        public int? DefaultTaxAuthorityId { get; set; }
        public string ClientsName { get; set; }
        public bool? TransactionMade { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 