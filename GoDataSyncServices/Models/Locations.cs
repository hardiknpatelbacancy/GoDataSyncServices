using System;

namespace GoDataSyncServices.Models
{
    public class Locations
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string AccountsId { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 