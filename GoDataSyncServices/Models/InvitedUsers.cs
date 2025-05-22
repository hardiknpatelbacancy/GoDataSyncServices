using System;

namespace GoDataSyncServices.Models
{
    public class InvitedUsers
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? CompanyId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoUrl { get; set; }
        public string Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 