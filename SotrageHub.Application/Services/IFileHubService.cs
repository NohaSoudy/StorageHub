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
        Task<Stream?> GetFileStreamAsync(string fileName);
        Task<IEnumerable<FileHubDTO>> GetAllFiles();
        bool DeleteFile(Guid id);
        bool DeleteFilePermanently(Guid id);
    }
}
