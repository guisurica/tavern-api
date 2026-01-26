using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface IFolderService
{
    Task<Result<string>> CreateFolderAsync(CreateFolderDTO input, string userId);
}
