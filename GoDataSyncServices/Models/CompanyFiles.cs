using System;

namespace GoDataSyncServices.Models
{
    public class CompanyFiles
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 