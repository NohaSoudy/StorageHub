using System.ComponentModel.DataAnnotations;

namespace StorageHub.Infrastructure
{
    public class FileHub
    {
        [Key]
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }
        public string? ContentType { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

    }


}
