using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Responses;
using tavern_api.Entities;

namespace tavern_api.Services;

internal sealed class FolderService : IFolderService
{
    private readonly ITavernRepository _tavernRepository;
    private readonly IUserRepository _userRepository;

    public FolderService(ITavernRepository tavernRepository, IUserRepository userRepository)
    {
        _tavernRepository = tavernRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<string>> CreateFolderAsync(CreateFolderDTO input, string userId)
    {
        try
        {
            if (input == null)
                return new Result<string>().Failure("Houve um problema ao tentar executar sua requisição.", null, 404);

            if (string.IsNullOrEmpty(input.TavernId))
                return new Result<string>().Failure("Uma pasta deve estar vinculada há uma taverna", null, 400);

            var tavernFound = await _tavernRepository.GetById(input.TavernId);
            if (tavernFound == null)
                return new Result<string>().Failure("Taverna não encontrada", null, 404);

            var userDungeonMasterFound = await _userRepository.GetById(userId);
            if (userDungeonMasterFound == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var userWillBeAssinedToFolder = await _userRepository.GetById(input.UserId);
            if (userWillBeAssinedToFolder == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var membershipFound = await _tavernRepository.GetUserMembershipAsync(userWillBeAssinedToFolder.Id, tavernFound.Id);
            if (membershipFound == null)
                return new Result<string>().Failure("Usuário não pertence a essa taverna", null, 400);

            var newFolder = Folder.Create(input.FolderName, membershipFound.Id);

            await _tavernRepository.CreateFolderAsync(newFolder);

            return new Result<string>().Success("Pasta criada com sucesso", null, 201);
        
        } catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        } catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
    }
}
