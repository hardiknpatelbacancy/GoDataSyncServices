using System;

namespace GoDataSyncServices.Models
{
    public class TeamMembers
    {
        public Guid TeamId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 