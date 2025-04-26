

namespace StorageHub.Infrastructure
{
    public interface IFileHubRepository
    {
        Task<FileHub> GetFileByIdAsync(Guid id);
        Task<IEnumerable<FileHub>> GetAllFilesAsync();
        Task<int> AddFileAsync(FileHub file);
        Task UpdateFileAsync(FileHub file);
        Task<int> DeleteFileAsync(Guid id);
        Task<int> DeletePermanentlyByID(Guid id);
        Task<FileHub> GetFileByNameAsync(string name);

    }
}
