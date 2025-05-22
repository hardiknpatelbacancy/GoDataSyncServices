using System;

namespace GoDataSyncServices.Models
{
    public class ChangeRequestComments
    {
        public Guid Id { get; set; }
        public Guid ProposalId { get; set; }
        public string CommentText { get; set; }
        public Guid AuthorId { get; set; }
        public bool IsFromClient { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 