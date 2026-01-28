using System.IO;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface IFileService 
{
    Task ValidateImageSizeInBytes(long bytes);
    Task<string> SaveUserImage(Stream fileStream, string fileExtension, string userId);
    Task<Result<string>> CreateFileFromWebUpload(Stream stream, string fileName, string fileId);

    Task<Result<byte[]>> GetFileBytesAsync(string itemId);
    Task<Result<string>> DeleteItemFromDisk(string itemId);
}
