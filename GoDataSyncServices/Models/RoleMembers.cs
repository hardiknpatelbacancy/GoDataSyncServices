using System;

namespace GoDataSyncServices.Models
{
    public class RoleMembers
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid RolesId { get; set; }
        public Guid MembersId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 