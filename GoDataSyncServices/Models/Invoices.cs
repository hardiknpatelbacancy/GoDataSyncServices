using System;

namespace GoDataSyncServices.Models
{
    public class Invoices
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? CompanyId { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 