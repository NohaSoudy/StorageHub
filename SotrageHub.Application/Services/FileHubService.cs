using StorageHub.Domain;
using Microsoft.AspNetCore.Http;
using StorageHub.Infrastructure;
using AutoMapper;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon.S3.Model;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Runtime;

namespace SotrageHub.Application
{
    public class FileHubService : IFileHubService
    {
        private readonly IFileHubRepository _fileHubRepository;
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _s3Client;
        private readonly MinioSettings _minioSettings;
        public FileHubService(IFileHubRepository fileHubRepository, IMapper mapper, IAmazonS3 s3Client, IOptions<MinioSettings> minioSettings)
        {
            _fileHubRepository = fileHubRepository;
            _mapper = mapper;
            _s3Client = s3Client;
            _minioSettings = minioSettings.Value;
        }
        #region GET
        public async Task<FileHubDTO?> GetFileById(Guid id)
        {
            var file = await _fileHubRepository.GetFileByIdAsync(id);
            if (file == null) return null;
            else
            {
                return _mapper.Map<FileHubDTO>(file);
            }
        }

        public async Task<byte[]> GetFileStreamAsync(string filePath)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _minioSettings.BucketName,
                    Key = filePath
                };
                using (var response = await _s3Client.GetObjectAsync(request))
                using (var memoryStream = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(memoryStream);

                    return memoryStream.ToArray();
                }
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when reading object");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Get File Stream. Message:'{ex.Message}'");
                throw;
            }
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
        #endregion


        #region SAVE
        public async Task<int> SaveFileAsync(IFormFile file)
        {
            var fileGuid = Guid.NewGuid();
            using (var stream = file.OpenReadStream())
            {
                var uploadFileMINIO = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = fileGuid.ToString(),
                    ContentType = file.ContentType ?? "application/octet-stream",
                    BucketName = _minioSettings.BucketName
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadFileMINIO);
            }

            var fileHubDto = new FileHubDTO
            {
                Id = fileGuid,
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileSize = file.Length,
                FilePath = fileGuid.ToString()
            };
            var fileEntity = _mapper.Map<FileHub>(fileHubDto);
            int _returnSave = await _fileHubRepository.AddFileAsync(fileEntity);
            return _returnSave;
        }
        #endregion

        #region DELETE
        public async Task<bool> DeleteFile(Guid id)
        {
            var fileHubDto = _fileHubRepository.GetFileByIdAsync(id);
            if (fileHubDto == null) return false;
            bool deleteFileResult = await DeleteFileStreamAsync(fileHubDto.Result.FilePath);
            return deleteFileResult;
        }

        public async Task<bool> DeleteFileStreamAsync(string filePath)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _minioSettings.BucketName,
                    Key = filePath
                };
                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when reading object");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Get File Stream. Message:'{ex.Message}'");
                throw;
            }

        }
        public async Task<bool> DeleteFilePermanently(Guid id)
        {
            var fileHubDto = _fileHubRepository.GetFileByIdAsync(id);
            if (fileHubDto == null) return false;
            bool deleteFileResult = await DeleteFileStreamAsync(fileHubDto.Result.FilePath);
            return deleteFileResult;
        }
        #endregion
    }
}
