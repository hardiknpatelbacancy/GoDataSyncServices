using System;

namespace GoDataSyncServices.Models
{
    public class ProjectTags
    {
        public Guid ProjectId { get; set; }
        public Guid TagId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 