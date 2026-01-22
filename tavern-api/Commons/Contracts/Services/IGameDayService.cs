using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface IGameDayService
{
    Task<Result<GameDayDTO>> CreateGameDayAsync(CreateGameDayDTO input, string userId);
    Task<Result<List<GameDayDTO>>> GetTavernGameDaysAsync(string tavernId);
    Task<Result<GameDayDTO>> GetGameDayAsync(string id);
    Task<Result<string>> DeleteGameDayAsync(string id, string userId);
    Task<Result<string>> ConcludeGameDayAsync(ConcludeGameDayDTO input, string userId);
    Task<Result<string>> RescheduleGameDayAsync(ResheduleGameDayDTO input, string userId);
}