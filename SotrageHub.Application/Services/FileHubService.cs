using StorageHub.Domain;
using Microsoft.AspNetCore.Http;
using StorageHub.Infrastructure;
using AutoMapper;

namespace SotrageHub.Application
{
    public class FileHubService : IFileHubService
    {
        private readonly string _fileHubServiceRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        private readonly IFileHubRepository _fileHubRepository;
        private readonly IMapper _mapper;
        public FileHubService(IFileHubRepository fileHubRepository, IMapper mapper)
        {
            if (!Directory.Exists(_fileHubServiceRoot))
                Directory.CreateDirectory(_fileHubServiceRoot);
            _fileHubRepository = fileHubRepository;
            _mapper = mapper;
        }

        public async Task<int> SaveFileAsync(IFormFile file)
        {
            var filePath = Path.Combine(_fileHubServiceRoot, file.FileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            var fileHubDto = new FileHubDTO
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileSize = file.Length,
                FilePath = Path.GetFileName(filePath),
            };
            var fileEntity = _mapper.Map<FileHub>(fileHubDto);
            int _returnSave = await _fileHubRepository.AddFileAsync(fileEntity);
            return _returnSave;
        }
        public async Task<FileHubDTO?> GetFileById(Guid id)
        {
            var file = await _fileHubRepository.GetFileByIdAsync(id);
            //var path = Path.Combine(_fileHubServiceRoot, file.FileName);
            //if (!FileHub.Exists(path)) return null;
            if (file == null) return null;
            else
            {
                return _mapper.Map<FileHubDTO>(file);
            }
        }

        public async Task<Stream?> GetFileStreamAsync(string fileName)
        {
            var path = Path.Combine(_fileHubServiceRoot, fileName);
            // if (!FileHub.Exists(path)) return null;
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
        public async Task<IEnumerable<FileHubDTO>> GetAllFiles()
        {
            var files = _fileHubRepository.GetAllFilesAsync().Result.Where(a => !a.IsDeleted);
            if (files == null) return null;
            else
            {
                return _mapper.Map<IEnumerable<FileHubDTO>>(files);
            }
        }

        public bool DeleteFile(Guid id)
        {
            var fileHubDto = _fileHubRepository.GetFileByIdAsync(id);
            if (fileHubDto == null) return false;
            var path = Path.Combine(_fileHubServiceRoot, fileHubDto.Result.FilePath);
            Task<int> deleteFileResult = _fileHubRepository.DeleteFileAsync(fileHubDto.Result.Id);
            return deleteFileResult.Result > 0;

        }

        public bool DeleteFilePermanently(Guid id)
        {
            var fileHubDto = _fileHubRepository.GetFileByIdAsync(id);
            if (fileHubDto == null) return false;
            var path = Path.Combine(_fileHubServiceRoot, fileHubDto.Result.FilePath);
            Task<int> deleteFileResult = _fileHubRepository.DeletePermanentlyByID(fileHubDto.Result.Id);
            if (!File.Exists(path)) return false;
            File.Delete(path);
            return deleteFileResult.Result > 0;

        }
    }
}
