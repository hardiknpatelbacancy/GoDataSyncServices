using System;

namespace GoDataSyncServices.Models
{
    public class Roles
    {
        public Guid Id { get; set; }
        public string ParentType { get; set; }
        public string ParentId { get; set; }
        public string Name { get; set; }
        public bool? Enabled { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 