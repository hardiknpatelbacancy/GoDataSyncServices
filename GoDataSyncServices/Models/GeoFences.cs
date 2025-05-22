using System;

namespace GoDataSyncServices.Models
{
    public class GeoFences
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Guid ParentId { get; set; }
        public int ParentType { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid AuthorId { get; set; }
        public Guid EditorId { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 