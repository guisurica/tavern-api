using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Responses;
using tavern_api.Entities;

namespace tavern_api.Services.UserServices;

internal sealed class GameDayService : IGameDayService
{
    private readonly IGameDayRepository _gameDayRepository;
    private readonly ITavernRepository _tavernRepository;
    private readonly IUserRepository _userRepository;

    public GameDayService(
        IGameDayRepository gameDayRepository,
        ITavernRepository tavernRepository,
        IUserRepository userRepository)
    {
        _gameDayRepository = gameDayRepository;
        _tavernRepository = tavernRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<string>> RescheduleGameDayAsync(ResheduleGameDayDTO input, string userId)
    {
        try
        {
            var user = await _userRepository.GetById(userId);
            if (user == null) return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var tavern = await _tavernRepository.GetById(input.TavernId);
            if (tavern == null) return new Result<string>().Failure("Taverna não encontrada", null, 404);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(user.Id, tavern.Id);
            if (userMembership == null)
                return new Result<string>().Failure("Usuário não faz parte dessa taverna", null, 404);

            tavern.UserHasPermissionToPerformAction(userMembership);

            var gameDayFound = await _tavernRepository.GetGameDayByIdAsync(input.Id);

            if (gameDayFound.IsConcluded)
                return new Result<string>().Failure("O dia de jogo esta concluido, não pode ser reagendado", null, 409);

            gameDayFound.Reschedule(input.NewDate);

            gameDayFound.UpdateNotes(input.Notes);

            await _tavernRepository.UpdateGameDayAsync(gameDayFound);

            return new Result<string>().Success("Dia de jogo reagendado com sucesso", null, 200);

        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }


    public async Task<Result<string>> ConcludeGameDayAsync(ConcludeGameDayDTO input, string userId)
    {
        try
        {
            var user = await _userRepository.GetById(userId);
            if (user == null) return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var tavern = await _tavernRepository.GetById(input.TavernId);
            if (tavern == null) return new Result<string>().Failure("Taverna não encontrada", null, 404);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(user.Id, tavern.Id);
            if (userMembership == null) 
                return new Result<string>().Failure("Usuário não faz parte dessa taverna", null, 404);

            tavern.UserHasPermissionToPerformAction(userMembership);

            var gameDayFound = await _tavernRepository.GetGameDayByIdAsync(input.Id);

            if (gameDayFound.IsConcluded)
                return new Result<string>().Failure("O dia de jogo já esta concluido", null, 409);

            gameDayFound.ConcludeGameDay();

            tavern.ExperienceGain(4);

            await _tavernRepository.UpdateAsync(tavern);

            await _tavernRepository.UpdateGameDayAsync(gameDayFound);

            return new Result<string>().Success("Dia de jogo concluido com sucesso", null, 200);

        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<GameDayDTO>> CreateGameDayAsync(CreateGameDayDTO input, string userId)
    {
        try
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
                return new Result<GameDayDTO>().Failure("Usuário não encontrado", null, 404);

            var tavern = await _tavernRepository.GetById(input.TavernId);
            if (tavern == null)
                return new Result<GameDayDTO>().Failure("Taverna não encontrada", null, 404);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(user.Id, input.TavernId);
            if (userMembership == null)
                return new Result<GameDayDTO>().Failure("Usuário não pertence a essa taverna", null, 400);

            tavern.UserHasPermissionToPerformAction(userMembership);

            var newGameDay = GameDay.Create(input.TavernId, input.ScheduledAt, input.Notes);

            var saved = await _gameDayRepository.CreateAsync(newGameDay);

            var dto = new GameDayDTO
            {
                Id = saved.Id,
                TavernId = saved.TavernId,
                ScheduledAt = saved.ScheduledAt,
                Notes = saved.Notes
            };

            return new Result<GameDayDTO>().Success("GameDay criado com sucesso", dto, 201);
        }
        catch (DomainException ex)
        {
            return new Result<GameDayDTO>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<GameDayDTO>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<List<GameDayDTO>>> GetTavernGameDaysAsync(string tavernId)
    {
        try
        {
            var tavern = await _tavernRepository.GetById(tavernId);
            if (tavern == null)
                return new Result<List<GameDayDTO>>().Failure("Taverna não encontrada", null, 404);

            var gamedays = await _gameDayRepository.GetByTavernIdAsync(tavernId);

            var dtoList = gamedays.Select(g => new GameDayDTO
            {
                Id = g.Id,
                TavernId = g.TavernId,
                ScheduledAt = g.ScheduledAt,
                Notes = g.Notes,
                IsConcluded = g.IsConcluded
            }).ToList();

            return new Result<List<GameDayDTO>>().Success("", dtoList, 200);
        }
        catch (InfrastructureException ex)
        {
            return new Result<List<GameDayDTO>>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<GameDayDTO>> GetGameDayAsync(string id)
    {
        try
        {
            var gameDay = await _gameDayRepository.GetById(id);
            if (gameDay == null)
                return new Result<GameDayDTO>().Failure("GameDay não encontrado", null, 404);

            var dto = new GameDayDTO
            {
                Id = gameDay.Id,
                TavernId = gameDay.TavernId,
                ScheduledAt = gameDay.ScheduledAt,
                Notes = gameDay.Notes,
                IsConcluded = gameDay.IsConcluded
            };

            return new Result<GameDayDTO>().Success("", dto, 200);
        }
        catch (InfrastructureException ex)
        {
            return new Result<GameDayDTO>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<string>> DeleteGameDayAsync(string id, string userId)
    {
        try
        {
            var gameDay = await _gameDayRepository.GetById(id);
            if (gameDay == null)
                return new Result<string>().Failure("GameDay não encontrado", null, 404);

            var tavern = await _tavernRepository.GetById(gameDay.TavernId);
            if (tavern == null)
                return new Result<string>().Failure("Taverna não encontrada", null, 404);

            var user = await _userRepository.GetById(userId);
            if (user == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(user.Id, tavern.Id);
            if (userMembership == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, 400);

            tavern.UserHasPermissionToPerformAction(userMembership);

            await _gameDayRepository.RemoveAsync(gameDay);

            return new Result<string>().Success("GameDay removido com sucesso", string.Empty, 200);
        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }
}