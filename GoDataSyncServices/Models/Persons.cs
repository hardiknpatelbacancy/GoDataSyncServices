using System;

namespace GoDataSyncServices.Models
{
    public class Persons
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public bool? AccountActive { get; set; }
        public DateTime? AccountLockedDate { get; set; }
        public string AccountLockedReason { get; set; }
        public string AddressL1 { get; set; }
        public string AddressL2 { get; set; }
        public bool? AllowPayCc { get; set; }
        public bool? AllowPayEcheck { get; set; }
        public string BillAttention { get; set; }
        public string City { get; set; }
        public int? ClientId { get; set; }
        public string Email { get; set; }
        public string EmailLowerCase { get; set; }
        public string FirstName { get; set; }
        public int? InvalidLoginAttempts { get; set; }
        public bool? IsMainContact { get; set; }
        public DateTime? LastInvalidAttempt { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public bool? RecordUpdatedByPortal { get; set; }
        public string Salutation { get; set; }
        public bool? ServicesActive { get; set; }
        public bool? ServiceShowCompleteOnly { get; set; }
        public string State { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Username { get; set; }
        public string Zip { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 