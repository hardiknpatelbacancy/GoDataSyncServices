using System;

namespace GoDataSyncServices.Models
{
    public class Companies
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string Name { get; set; }
        public bool? Enabled { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? AttachInvoices { get; set; }
        public string GeneralLaborId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 