using System;

namespace GoDataSyncServices.Models
{
    public class FileComments
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public string CommentText { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
} 