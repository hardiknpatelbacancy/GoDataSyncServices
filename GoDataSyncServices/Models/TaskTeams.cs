using System;

namespace GoDataSyncServices.Models
{
    public class TaskTeams
    {
        public Guid TaskId { get; set; }
        public Guid TeamId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 