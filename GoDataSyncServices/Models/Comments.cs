using System;

namespace GoDataSyncServices.Models
{
    public class Comments
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string ParentType { get; set; }
        public string CommentText { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 