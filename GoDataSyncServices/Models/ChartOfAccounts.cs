using System;

namespace GoDataSyncServices.Models
{
    public class ChartOfAccounts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? TenantsId { get; set; }
        public Guid? CompaniesId { get; set; }
        public string ExternalId { get; set; }
        public string AccountId { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }
        public string ProofStatementType { get; set; }
        public string AccountType { get; set; }
        public string JobCostCategory { get; set; }
        public string AccountCategory { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 