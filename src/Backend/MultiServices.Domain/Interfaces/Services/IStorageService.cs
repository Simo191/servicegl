using MultiServices.Domain.Common;

namespace MultiServices.Domain.Interfaces.Services;

public interface IStorageService
{
    Task<Result<string>> UploadFileAsync(Stream stream, string fileName, string contentType, string folder);
    Task<Result> DeleteFileAsync(string fileUrl);
    Task<Result<string>> GetPresignedUrlAsync(string fileUrl, TimeSpan expiry);
}
