using System;

namespace GoDataSyncServices.Models
{
    public class Clients
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 