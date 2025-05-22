using System;

namespace GoDataSyncServices.Models
{
    public class Filters
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public string Name { get; set; }
        public string SearchTemplate { get; set; }
        public Guid AccountsId { get; set; }
        public Guid AuthorId { get; set; }
        public int? ParentType { get; set; }
        public bool? Private { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 