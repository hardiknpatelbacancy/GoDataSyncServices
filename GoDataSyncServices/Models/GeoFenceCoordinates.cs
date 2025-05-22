using System;

namespace GoDataSyncServices.Models
{
    public class GeoFenceCoordinates
    {
        public Guid Id { get; set; }
        public Guid GeofenceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsInner { get; set; }
        public int CoordinateOrder { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 