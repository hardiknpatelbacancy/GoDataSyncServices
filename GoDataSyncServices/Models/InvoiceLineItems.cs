using System;

namespace GoDataSyncServices.Models
{
    public class InvoiceLineItems
    {
        public Guid Id { get; set; }
        public Guid? InvoiceId { get; set; }
        public string Name { get; set; }
        public int AmountCents { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 