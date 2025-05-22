using System;

namespace GoDataSyncServices.Models
{
    public class CompanyDivisions
    {
        public Guid Id { get; set; }
        public Guid? CompanyId { get; set; }
        public string DivisionNo { get; set; }
        public string Name { get; set; }
        public string PaymentAchCredentials { get; set; }
        public string PaymentCardCredentials { get; set; }
        public string ReplyTo { get; set; }
        public bool? AttachInvoices { get; set; }
        public string FromEmail { get; set; }
        public string TaxauthoritiesId { get; set; }
        public string GeneralLaborId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 