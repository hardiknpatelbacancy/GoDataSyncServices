using System;

namespace GoDataSyncServices.Models
{
    public class PaymentMethods
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public string Summary { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 