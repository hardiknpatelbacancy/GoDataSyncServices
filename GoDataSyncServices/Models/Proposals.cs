using System;

namespace GoDataSyncServices.Models
{
    public class Proposals
    {
        public Guid Id { get; set; }
        public Guid TenantsId { get; set; }
        public Guid CompaniesId { get; set; }
        public Guid ProjectsId { get; set; }
        public Guid ClientsId { get; set; }
        public string ProposalNumber { get; set; }
        public int ProposalVersion { get; set; }
        public Guid CreatedById { get; set; }
        public string TemplateConfig { get; set; }
        public int? Status { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ViewedAt { get; set; }
        public DateTime? SignedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string SigningDetails { get; set; }
        public Guid? PdfFileId { get; set; }
        public bool? IsArchived { get; set; }
        public string ClientEmail { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 