using Serilog;
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
    private readonly INotificationRepository _notificationRepository;

    public TavernService(ITavernRepository tavernRepository, IUserRepository userRepository, INotificationRepository notificationRepository)
    {
        _tavernRepository = tavernRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<string>> AcceptUserInTavernAsync(AcceptUserInTavernDTO input, string userId)
    {
        try
        {
            Log.Information("AcceptUserInTavernAsync service starting - {userid}", userId);

            var userWhoWantsEnterTavern = await _userRepository.GetById(input.UserIdWhoWantsEnterTavern);
            if (userWhoWantsEnterTavern == null)
            {
                Log.Warning("AcceptUserInTavernAsync - User who wants enter the tavern not found - {id}", userId);

                return new Result<string>().Failure("Usuário não encontrado", null, 404);
            }

            var userWhoWillAcceptUserInTavern = await _userRepository.GetByEmailAsync(input.UserWhoWillAcceptsNewUserTavern);
            if (userWhoWillAcceptUserInTavern == null)
            {
                Log.Warning("AcceptUserInTavernAsync - User who will accepts others users in tavern not found - {email}", 
                    input.UserWhoWillAcceptsNewUserTavern);

                return new Result<string>().Failure("Usuário não encontrado", null, 404);
            }

            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
            {
                Log.Warning("AcceptUserInTavernAsync - Tavern not found - {id}", input.TavernId);

                return new Result<string>().Failure("Taverna não encontrada", null, 404);
            }

            var usersAlreadyInTavern = await _tavernRepository.GetAllUserMembershipsAsync(tavernFound.Id);

            tavernFound.CanAddUserInTavern(usersAlreadyInTavern.Count());

            var newUserMembership = Membership.Create(tavernFound.Id, userWhoWantsEnterTavern.Id, false, MembershipStatus.COMMON);

            await _tavernRepository.CreateMembership(newUserMembership);

            return new Result<string>().Success("Usuário adicionado a taverna com sucesso", null, 201);

        } catch (InfrastructureException ex)
        {
            Log.Error(ex.Message, ex);

            return new Result<string>().Failure("Um erro aconteceu ao tentar concluir sua solicitação. Caso o problema persista, contate o suporte", null, 500);
        } catch (DomainException ex)
        {
            Log.Error(ex.Message, ex);

            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<string>> AskForEnterAsync(AskForEnterDTO input, string userId)
    {
        try
        {
            Log.Information("AskForEnterAsync - Ask for enter service starting - {id}", userId);

            var userWhoWillReceiveNotification = await _userRepository.GetByEmailAsync(input.ReceriverEmail);
            if (userWhoWillReceiveNotification == null)
            {
                Log.Warning("AskForEnterAsync - Not found receiver user - {email}", input.ReceriverEmail);

                return new Result<string>().Failure("Usuário não encontrado", null, 404);
            }

            var userWhoWantsToJoinTavern = await _userRepository.GetById(userId);
            if (userWhoWantsToJoinTavern == null)
            {
                Log.Warning("AskForEnterAsync - Not found user who wants to join the tavern - {id}", userId);

                return new Result<string>().Failure("Usuário não encontrado", null, 404);
            }

            var tavernTheUserWantsToJoin = await _tavernRepository.GetById(input.TavernId);
            if (tavernTheUserWantsToJoin == null)
            {
                Log.Warning("AskForEnterAsync - Not found tavern - {tavernId}", input.TavernId);

                return new Result<string>().Failure("Taverna não encontrada", null, 404);
            }

            var allUserNotifications = await _notificationRepository.GetAllUserNotification(userId);
            if (allUserNotifications.Any(n => n.TavernId == tavernTheUserWantsToJoin.Id))
                return new Result<string>().Failure("Você tem um pedido de entrada pendente para essa taverna. Aguarde até que seja aceito!", null, 400);

            var newNotification = Notification.Create(
                input.InviteMessage,
                input.ReceriverEmail,
                input.TavernId,
                userId,
                NotificationType.NewInviteOrder);

            await _notificationRepository.CreateAsync(newNotification);

            return new Result<string>().Success("Notificação enviada com sucesso", userId, 201);

        } catch(InfrastructureException ex)
        {
            Log.Error(ex.Message, ex);

            return new Result<string>().Failure("Um erro aconteceu ao tentar concluir sua solicitação. Caso o problema persista, contate o suporte", null, 500);

        }
        catch (DomainException ex)
        {
            Log.Warning(
                ex,
                "AskForEnterAsync - Finished - Error"
            );

            return new Result<string>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<List<TavernDTO>>> GetAllApplicationTaverns(string userId, int pageNumber)
    {
        try
        {
            Log.Information(
                    "GetAllApplicationTaverns - Starting - {userId} - {pageNumber}",
                    userId,
                    pageNumber
                );

            var userFound = await _userRepository.GetById(userId);
            if (userFound == null)
                return new Result<List<TavernDTO>>().Failure("Usuário não encontrado", null, 404);

            var userMemberships = await _tavernRepository.GetAllUserMembershipsAsync(userId);

            var getAllTaverns = await _tavernRepository.GetAllApplicationTavernsAsync(pageNumber);

            var globalTaverns = new List<TavernDTO>();

            FillTavernsWhereUserNotIn(userMemberships, getAllTaverns, globalTaverns);

            if (globalTaverns.Count <= 0)
                return new Result<List<TavernDTO>>().Success("Não existem tavernas para mostrar", new List<TavernDTO>(), 200);

            return new Result<List<TavernDTO>>().Success("", globalTaverns, 200);

        } catch (DomainException ex)
        {

            Log.Warning(
                "GetAllApplicationTaverns - Finished - {userId} - {pageNumber}",
                userId,
                pageNumber
            );

            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 400);
        } catch (InfrastructureException ex)
        {
            Log.Error(
                ex,
                "GetAllApplicationTaverns - Finished - Error"
            );

            return new Result<List<TavernDTO>>().Failure("Um erro aconteceu ao tentar concluir sua solicitação. Caso o problema persista, contate o suporte", null, 500);
        }
    }

    private void FillTavernsWhereUserNotIn(
            List<TavernDTO> userMemberships,
            List<TavernDTO> getAllTaverns,
            List<TavernDTO> globalTaverns

        )
    {
        if (userMemberships.Count <= 0)
        {
            globalTaverns.AddRange(getAllTaverns);
        } else
        {
            for (int i = 0; i < getAllTaverns.Count(); i++)
            {
                if (userMemberships[i].Id != getAllTaverns[i].Id)
                {
                    globalTaverns.Add(getAllTaverns[i]);
                }
            }
        }

    }

    public async Task<Result<List<TavernDTO>>> GetUserTavernsAsync(string id)
    {
        try
        {
            Log.Information(
                    "GetUserTavernsAsync - Starting - {id}",
                    id
                );

            var allUsersMemberships = await _tavernRepository.GetAllUserMembershipsAsync(id);
            if (allUsersMemberships.Count <= 0) return new Result<List<TavernDTO>>().Success("Usuário não possui tavernas", new List<TavernDTO>(), 200);

            Log.Information(
                "GetUserTavernsAsync - Finished - {id}",
                id
            );

            return new Result<List<TavernDTO>>().Success("", allUsersMemberships, 200);
        }
        catch (ArgumentException ex)
        {
            Log.Warning(ex, "GetUserTavernsAsync - Warning");

            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 404);
        }
        catch (InfrastructureException ex)
        {
            Log.Error(ex, "GetUserTavernsAsync - Error");

            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            Log.Warning(ex, "GetUserTavernsAsync - Warning");

            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<TavernDTO>> GetTavernAsync(string id)
    {
        try
        {
            Log.Information(
                    "GetTavernAsync - Starting - {id}",
                    id
                );

            var tavernFound = await _tavernRepository.GetById(id);
            if (tavernFound == null) return new Result<TavernDTO>().Failure("Taverna não encontrada", null, 404);

            var usersInTavern = await _tavernRepository.GetAllTavernUsers(id);

            var getAllTavernGameDays = await _tavernRepository.GetAllTavernGameDaysAsync(id);

            List<FolderDTO> folders = new List<FolderDTO>();
            List<ItemDTO> items = new List<ItemDTO>();
            
            for(int i = 0; i < usersInTavern.Count; i++)
            {
                var user = usersInTavern[i];
                var getAllUserMembershipFolders = await _tavernRepository.GetAllUsersMembershipFolders(user.MembershipId);
                folders.AddRange(getAllUserMembershipFolders);
            }

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
                    StatusInTavern = u.StatusInTavern,
                    ProfilePicture = u.ProfilePicture
                }).ToList(),
                GameDays = getAllTavernGameDays,
                Folders = folders
            };

            Log.Information(
                "GetTavernAsync - Finished - {id}",
                id
            );

            return new Result<TavernDTO>().Success("", tavernDTO, 200);

        }
        catch (DomainException ex)
        {
            Log.Warning(ex, "GetTavernAsync - Warning");

            return new Result<TavernDTO>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            Log.Error(ex, "GetTavernAsync - Error");

            return new Result<TavernDTO>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<string>> RemoveUserTavernAsync(RemoveUserTavernDTO input, string id)
    {
        try
        {
            var userWhoWillBeRemovedInTavern = await _userRepository.GetById(input.UserId);
            if (userWhoWillBeRemovedInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var userWillRemoveInTavern = await _userRepository.GetById(id);
            if (userWillRemoveInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
                return new Result<string>().Failure("Taverna não encontrada", null, 404);

            var allTavernUsers = await _tavernRepository.GetAllTavernUsers(input.TavernId);


            if (!allTavernUsers.Select(t => t.Id).Contains(userWhoWillBeRemovedInTavern.Id))
                return new Result<string>().Failure("Esse usuário não esta na taverna", null, 409);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(userWillRemoveInTavern.Id, input.TavernId);
            if (userMembership == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, 400);

            if (userMembership.IsDm)
                return new Result<string>().Failure("O DM não pode ser removido da taverna", null, 400);

            tavernFound.UserHasPermissionToPerformAction(userMembership);

            var userWhoWillBeRemovedMembership = await _tavernRepository.GetUserMembershipAsync(userWhoWillBeRemovedInTavern.Id, input.TavernId);

            var membershipToUpdate = tavernFound.DeleteMembership(userWhoWillBeRemovedMembership);

            await _tavernRepository.RemoveMembership(membershipToUpdate);

            return new Result<string>().Success("Usuário removido com sucesso", string.Empty, 201);

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

    public async Task<Result<string>> AddUserToTavernAsync(AddUserTavernDTO input, string id)
    {
        try
        {
            var userWhoWillBeAddedInTavern = await _userRepository.GetByNameAndDiscriminator(input.Username, input.Discriminator);
            if (userWhoWillBeAddedInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var userWillAddInTavern = await _userRepository.GetById(id);
            if (userWillAddInTavern == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
                return new Result<string>().Failure("Taverna não encontrada", null, 404);

            var allTavernUsers = await _tavernRepository.GetAllTavernUsers(input.TavernId);

            if (allTavernUsers.Select(t => t.Id).Contains(userWhoWillBeAddedInTavern.Id))
                return new Result<string>().Failure("Usuário já está participando desta taverna", null, 409);

            tavernFound.CanAddUserInTavern(allTavernUsers.Count);

            var userMembership = await _tavernRepository.GetUserMembershipAsync(userWillAddInTavern.Id, input.TavernId);
            if (userMembership == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, 400);

            tavernFound.UserHasPermissionToPerformAction(userMembership);

            var newMembership = Membership.Create(input.TavernId, userWhoWillBeAddedInTavern.Id, false, input.Status);

            await _tavernRepository.CreateMembership(newMembership);

            return new Result<string>().Success("Usuário adicionado com sucesso", string.Empty, 201);

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

    public async Task<Result<TavernDTO>> CreateTavernAsync(CreateTavernDTO input, string userEmail)
    {
        try
        {
            var userFound = await _userRepository.GetByEmailAsync(userEmail);
            if (userFound == null)
                return new Result<TavernDTO>().Failure("Usuário não encontrado.", null, 404);

            var newTavern = Tavern.Create(input.Name, input.Description, input.Capacity, userFound.Email);

            var tavernSaved = await _tavernRepository.CreateAsync(newTavern);

            var newMembershipCreated = Membership.Create(tavernSaved.Id, userFound.Id, true, MembershipStatus.ADMMIN);

            await _tavernRepository.CreateMembership(newMembershipCreated);

            return new Result<TavernDTO>().Success("Taverna criada com sucesso.", new TavernDTO
            {
                Id = tavernSaved.Id,
                Name = tavernSaved.Name,
                Description = tavernSaved.Description,
                Capacity = tavernSaved.Capacity
            }, 201);

        }
        catch (DomainException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<TavernDTO>> UpdateTavernAsync(UpdateTavernDTO input, string userId)
    {
        try
        {
            if (input == null || string.IsNullOrWhiteSpace(input.TavernId))
                return new Result<TavernDTO>().Failure("TavernId obrigatório.", null, 400);

            var tavern = await _tavernRepository.GetById(input.TavernId);
            if (tavern == null) return new Result<TavernDTO>().Failure("Taverna não encontrada", null, 404);

            var user = await _userRepository.GetById(userId);
            if (user == null) return new Result<TavernDTO>().Failure("Usuário não encontrado", null, 404);

            var membership = await _tavernRepository.GetUserMembershipAsync(user.Id, tavern.Id);
            if (membership == null) return new Result<TavernDTO>().Failure("Usuário não pertence a essa taverna", null, 400);

            tavern.UserHasPermissionToPerformAction(membership);

            if (!string.IsNullOrWhiteSpace(input.Name) && input.Name != tavern.Name)
            {
                tavern.ChangeName(input.Name);
            }

            if (input.Description != null && input.Description != tavern.Description)
            {
                tavern.ChangeDescription(input.Description);
            }

            if (input.Capacity.HasValue && input.Capacity.Value != tavern.Capacity)
            {
                var currentUsers = await _tavernRepository.GetAllTavernUsers(tavern.Id);
                if (input.Capacity.Value < currentUsers.Count)
                    return new Result<TavernDTO>().Failure("Nova capacidade menor que o número atual de usuários na taverna", null, 409);

                tavern.ChangeCapacity(input.Capacity.Value);
            }

            var updated = await _tavernRepository.UpdateAsync(tavern);

            return new Result<TavernDTO>().Success("Taverna atualizada com sucesso", new TavernDTO
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description,
                Capacity = updated.Capacity,
                Level = updated.Level,
                CurrentExperience = updated.CurrentExperience,
                LevelExperienceLimit = updated.LevelExperienceLimit
            }, 200);
        }
        catch (DomainException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<TavernDTO>().Failure(ex.Message, null, 500);
        }
    }
}
