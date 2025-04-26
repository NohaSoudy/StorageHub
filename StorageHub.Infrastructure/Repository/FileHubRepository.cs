using Microsoft.EntityFrameworkCore;

namespace StorageHub.Infrastructure
{
    public class FileHubRepository : IFileHubRepository
    {
        private readonly AppDbContext _context;

        public FileHubRepository (AppDbContext context)
        {
            _context = context;
        }

        public async Task<FileHub> GetFileByIdAsync(Guid id)
        {
            return await _context.FileHub.FindAsync(id);
        }

        public async Task<IEnumerable<FileHub>> GetAllFilesAsync()
        {
            return await _context.FileHub.ToListAsync();
        }

        public async Task<int> AddFileAsync(FileHub file)
        {
            file.CreatedAt = DateTime.UtcNow; 
            await _context.FileHub.AddAsync(file);
          return  await _context.SaveChangesAsync();
        }

        public async Task UpdateFileAsync(FileHub file)
        {
            file.UpdatedAt = DateTime.UtcNow;
            _context.FileHub.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteFileAsync(Guid id)
        {
            var file = await _context.FileHub.FindAsync(id);
            if (file != null)
            {
                file.IsDeleted = true;
                file.DeletedAt = DateTime.UtcNow;
               return await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("File not found");
            }
        }
        public async Task<int> DeletePermanentlyByID(Guid id)
        {
            var file = await _context.FileHub.FindAsync(id);
            if (file != null)
            {
                file.IsDeleted = true;
                file.DeletedAt = DateTime.UtcNow;
                _context.FileHub.Remove(file);
                return await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("File not found");
            }
        }

        public Task<FileHub> GetFileByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
    }

}
