using System;

namespace GoDataSyncServices.Models
{
    public class ClientMembers
    {
        public Guid ClientId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 