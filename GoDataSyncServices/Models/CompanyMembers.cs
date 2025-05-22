using System;

namespace GoDataSyncServices.Models
{
    public class CompanyMembers
    {
        public Guid CompanyId { get; set; }
        public Guid MemberId { get; set; }
        public bool? IsArchived { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 