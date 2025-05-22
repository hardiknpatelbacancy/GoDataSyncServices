using System;

namespace GoDataSyncServices.Models
{
    public class TaskTags
    {
        public Guid TaskId { get; set; }
        public Guid TagId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 