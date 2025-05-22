using System;

namespace GoDataSyncServices.Models
{
    public class Tenants
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string? owner_email { get; set; }
        public string? owner_name { get; set; }
        public bool? enabled { get; set; } = true;
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
} 