using System;

namespace GoDataSyncServices.Models
{
    public class PaymentAddresses
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public string AddressLine1 { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 