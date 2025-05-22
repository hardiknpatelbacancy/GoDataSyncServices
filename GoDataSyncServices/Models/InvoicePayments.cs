using System;

namespace GoDataSyncServices.Models
{
    public class InvoicePayments
    {
        public Guid Id { get; set; }
        public Guid? InvoiceId { get; set; }
        public int AmountCents { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 