using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface ITavernService
{
    Task<Result<TavernDTO>> CreateTavernAsync(CreateTavernDTO input, string userEmail);
    Task<Result<string>> AddUserToTavernAsync(AddUserTavernDTO input, string id);
    Task<Result<string>> RemoveUserTavernAsync(RemoveUserTavernDTO input, string id);
    Task<Result<TavernDTO>> GetTavernAsync(string id);
    Task<Result<List<TavernDTO>>> GetUserTavernsAsync(string id);
    Task<Result<TavernDTO>> UpdateTavernAsync(UpdateTavernDTO input, string userId);
    Task<Result<List<TavernDTO>>> GetAllApplicationTaverns(string userId, int pageNumber);
    Task<Result<string>> AcceptUserInTavernAsync(AcceptUserInTavernDTO input, string userId);

    Task<Result<string>> AskForEnterAsync(AskForEnterDTO input, string userId);
}
