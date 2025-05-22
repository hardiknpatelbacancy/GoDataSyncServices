using System;

namespace GoDataSyncServices.Models
{
    public class DailyReports
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string Summary { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 