using System;

namespace GoDataSyncServices.Models
{
    public class Files
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long? FileSize { get; set; }
        public Guid ParentId { get; set; }
        public string ParentType { get; set; }
        public Guid AuthorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 