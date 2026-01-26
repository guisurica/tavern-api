using System.IO;

namespace tavern_api.Commons.Contracts.Services;

public interface IFileService 
{
    Task ValidateImageSizeInBytes(long bytes);
    Task<string> SaveUserImage(Stream fileStream, string fileExtension, string userId);
}
