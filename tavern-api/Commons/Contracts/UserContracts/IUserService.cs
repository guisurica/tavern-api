using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.UserContracts;

public interface IUserService
{
    public Task<Result<UserDTO>> CreateUserAsync(CreateUserDTO input);
    public Task<Result<UserDTO>> LoginUserAsync(LoginUserDTO input);
    public Task<Result<List<TavernDTO>>> GetUserTavernsAsync(string id);
    public Task<Result<string>> ChangeUsernameAsync(ChangeUsernameDTO input, string id);
    public Task<Result<UserDTO>> GetUserProfileAsync(string id);
}
