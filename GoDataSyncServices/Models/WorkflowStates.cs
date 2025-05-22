using System;

namespace GoDataSyncServices.Models
{
    public class WorkflowStates
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid WorkflowsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Order { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 