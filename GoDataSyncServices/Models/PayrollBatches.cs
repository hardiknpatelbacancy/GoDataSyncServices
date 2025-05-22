using System;

namespace GoDataSyncServices.Models
{
    public class PayrollBatches
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public int BatchNumber { get; set; }
        public int Interval { get; set; }
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }
        public string Description { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 