using System;

namespace GoDataSyncServices.Models
{
    public class Assemblies
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public string Description { get; set; }
        public string UnitMeasure { get; set; }
        public string Sort { get; set; }
        public string PartNumber { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 