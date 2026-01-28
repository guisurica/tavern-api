using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface IFolderService
{
    Task<Result<string>> CreateFolderAsync(CreateFolderDTO input, string userId);
    Task<Result<string>> CreateFileAsync(CreateFileDTO input, string userId);
    Task<Result<byte[]>> GetFileBytesAsync(string itemId, string userId);
    Task<Result<string>> DeleteFileAsync(string itemId, string userId);
}
