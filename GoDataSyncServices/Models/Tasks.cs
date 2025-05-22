using System;

namespace GoDataSyncServices.Models
{
    public class Tasks
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid ProjectsId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? CreatedById { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ActualHours { get; set; }
        public bool? IsBillable { get; set; }
        public decimal? BillableRate { get; set; }
        public string Notes { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 