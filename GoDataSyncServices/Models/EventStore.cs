using System;

namespace GoDataSyncServices.Models
{
    public class EventStore
    {
        public Guid EventId { get; set; }
        public Guid AggregateId { get; set; }
        public string AggregateType { get; set; }
        public string EventType { get; set; }
        public string EventData { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid? TenantsId { get; set; }
        public Guid? CompaniesId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 