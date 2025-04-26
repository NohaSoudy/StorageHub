namespace StorageHub.Domain
{
    public class FileHubDTO
    {
        public Guid Id { get; set; } 
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
