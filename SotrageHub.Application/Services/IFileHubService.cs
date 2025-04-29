using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StorageHub.Domain;

namespace SotrageHub.Application
{
    public interface IFileHubService
    {
        Task<int> SaveFileAsync(IFormFile file);
        Task<FileHubDTO> GetFileById(Guid id);
        Task<byte[]> GetFileStreamAsync(string fileName);
        Task<IEnumerable<FileHubDTO>> GetAllFiles();
        Task<bool> DeleteFile(Guid id);
        Task<bool> DeleteFilePermanently(Guid id);
    }
}
