using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Enums;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Responses;
using tavern_api.Entities;

namespace tavern_api.Services;

internal sealed class TavernService : ITavernService
{

    private readonly ITavernRepository _tavernRepository;
    private readonly IUserRepository _userRepository;

    public TavernService(ITavernRepository tavernRepository, IUserRepository userRepository)
    {
        _tavernRepository = tavernRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<List<TavernDTO>>> GetUserTavernsAsync(string id)
    {
        try
        {
            var allUsersMemberships = await _tavernRepository.GetAllUserMembershipsAsync(id);
            if (allUsersMemberships.Count <= 0) return new Result<List<TavernDTO>>().Success("Usuário não possui tavernas", new List<TavernDTO>(), System.Net.HttpStatusCode.OK);

            return new Result<List<TavernDTO>>().Success("", allUsersMemberships, System.Net.HttpStatusCode.OK);
        }
        catch (ArgumentException ex)
        {
            return new Result<List<TavernDTO>>().Failure(ex.Message, null, System.Net.HttpStatusCode.NotFound);
        }
        catch (InfrastructureException ex)
        {
            return new Result<List<TavernDTO>>().Failure(ex.Message, null, System.Net.HttpStatusCode.InternalServerError);
        }
        catch (DomainException ex)
        {
            return new Result<List<TavernDTO>>().Failure(ex.Message, null, System.Net.HttpStatusCode.BadRequest);
        }
    }

    public async Task<Result<TavernDTO>> GetTavernAsync(string id)
    {
        try
        {
            var tavernFound = await _tavernRepository.GetById(id);
            if (tavernFound == null) return new Result<TavernDTO>().Failure("Taverna não encontrada", null, System.Net.HttpStatusCode.NotFound);

            var usersInTavern = await _tavernRepository.GetAllTavernUsers(id);

            var getAllTavernGameDays = await _tavernRepository.GetAllTavernGameDaysAsync(id);
            
            var tavernDTO = new TavernDTO
            {
                Id = tavernFound.Id,
                Name = tavernFound.Name,
                Description = tavernFound.Description,
                Capacity = tavernFound.Capacity,
                Level = tavernFound.Level,
                CurrentExperience = tavernFound.CurrentExperience,
                LevelExperienceLimit = tavernFound.LevelExperienceLimit,
                Users = usersInTavern.Select(u => new UserTavernDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Discriminator = u.Discriminator,
                    IsDm = u.IsDm,
                    StatusInTavern = u.StatusInTavern
                }).ToList(),
                GameDays = getAllTavernGameDays
            };

            return new Result<TavernDTO>().Success("", tavernDTO, System.Net.HttpStatusCode.OK);

        }
        catch (DomainException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, System.Net.HttpStatusCode.BadRequest);
        }
        catch (InfrastructureException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, System.Net.HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<string>> RemoveUserTavernAsync(RemoveUserTavernDTO input, string id)
    {
        try
        {
            var userWhoWillBeRemovedInTavern = await _userRepository.GetById(input.UserId);
            if (userWhoWillBeRemovedInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);

            var userWillRemoveInTavern = await _userRepository.GetById(id);
            if (userWillRemoveInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);

            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
                return new Result<string>().Failure("Taverna não encontrada", null, System.Net.HttpStatusCode.NotFound);

            var allTavernUsers = await _tavernRepository.GetAllTavernUsers(input.TavernId);

            if (!allTavernUsers.Select(t => t.Id).Contains(userWhoWillBeRemovedInTavern.Id))
                return new Result<string>().Failure("Esse usuário não esta na taverna", null, System.Net.HttpStatusCode.Conflict);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(userWillRemoveInTavern.Id, input.TavernId);
            if (userMembership == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, System.Net.HttpStatusCode.BadRequest);

            tavernFound.UserHasPermissionToPerformAction(userMembership);

            var userWhoWillBeRemovedMembership = await _tavernRepository.GetUserMembershipAsync(userWhoWillBeRemovedInTavern.Id, input.TavernId);

            var membershipToUpdate = tavernFound.DeleteMembership(userWhoWillBeRemovedMembership);

            await _tavernRepository.RemoveMembership(membershipToUpdate);

            return new Result<string>().Success("Usuário removido com sucesso", string.Empty, System.Net.HttpStatusCode.Created);

        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, System.Net.HttpStatusCode.BadRequest);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, System.Net.HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<string>> AddUserToTavernAsync(AddUserTavernDTO input, string id)
    {
        try
        {
            var userWhoWillBeAddedInTavern = await _userRepository.GetByNameAndDiscriminator(input.Username, input.Discriminator);
            if (userWhoWillBeAddedInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);

            var userWillAddInTavern = await _userRepository.GetById(id);
            if (userWillAddInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, System.Net.HttpStatusCode.NotFound);

            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
                return new Result<string>().Failure("Taverna não encontrada", null, System.Net.HttpStatusCode.NotFound);
            
            var allTavernUsers = await _tavernRepository.GetAllTavernUsers(input.TavernId);

            if (allTavernUsers.Select(t => t.Id).Contains(userWhoWillBeAddedInTavern.Id))
                return new Result<string>().Failure("Usuário já está participando desta taverna", null, System.Net.HttpStatusCode.Conflict);

            tavernFound.CanAddUserInTavern(allTavernUsers.Count);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(userWillAddInTavern.Id, input.TavernId);
            if (userMembership == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, System.Net.HttpStatusCode.BadRequest);

            tavernFound.UserHasPermissionToPerformAction(userMembership);

            var newMembership = Membership.Create(input.TavernId, userWhoWillBeAddedInTavern.Id, false, input.Status);

            await _tavernRepository.CreateMembership(newMembership);

            return new Result<string>().Success("Usuário adicionado com sucesso", string.Empty, System.Net.HttpStatusCode.Created);
        
        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, System.Net.HttpStatusCode.BadRequest);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, System.Net.HttpStatusCode.InternalServerError);
        }
    }

    public async Task<Result<TavernDTO>> CreateTavernAsync(CreateTavernDTO input, string userEmail)
    {
        try
        {
            var userFound = await _userRepository.GetByEmailAsync(userEmail);
            if (userFound == null)
                return new Result<TavernDTO>().Failure("Usuário não encontrado.", null, System.Net.HttpStatusCode.NotFound);

            var newTavern = Tavern.Create(input.Name, input.Description, input.Capacity);

            var tavernSaved = await _tavernRepository.CreateAsync(newTavern);

            var newMembershipCreated = Membership.Create(tavernSaved.Id, userFound.Id, true, MembershipStatus.ADMMIN);

            await _tavernRepository.CreateMembership(newMembershipCreated);

            return new Result<TavernDTO>().Success("Taverna criada com sucesso.", new TavernDTO
            {
                Id = tavernSaved.Id,
                Name = tavernSaved.Name,
                Description = tavernSaved.Description,
                Capacity = tavernSaved.Capacity
            }, System.Net.HttpStatusCode.Created);

        }
        catch(DomainException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, System.Net.HttpStatusCode.BadRequest);
        }
        catch(InfrastructureException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, System.Net.HttpStatusCode.InternalServerError);
        }
    }
}
