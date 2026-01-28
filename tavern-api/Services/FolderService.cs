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
    private readonly IFileService _fileService;

    public FolderService(ITavernRepository tavernRepository, IUserRepository userRepository, IFileService fileService)
    {
        _tavernRepository = tavernRepository;
        _userRepository = userRepository;
        _fileService = fileService;
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

    public async Task<Result<string>> CreateFileAsync(CreateFileDTO input, string userId)
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

            var filesInFolder = await _tavernRepository.GetAllFileInFolderAndTavernAndSignedUser(input.FolderId);
            if (filesInFolder.Select(f => f.ItemName + f.Extension).Contains(input.FormFile.FileName))
                return new Result<string>().Failure("Já existe um arquivo com esse nome nessa pasta", null, 409);

            var newfile = Item.Create(input.FormFile.FileName, Path.GetExtension(input.FormFile.FileName), input.FolderId, input.FormFile.Length);

            var fileRequest = await _fileService.CreateFileFromWebUpload(input.FormFile.OpenReadStream(), input.FormFile.FileName, newfile.Id);
            if (!fileRequest.IsSuccess)
                return new Result<string>().Failure(fileRequest);

            await _tavernRepository.CreateNewFileAsync(newfile);

            return new Result<string>().Success("Arquivo criado com sucesso", null, 201);

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

    public async Task<Result<byte[]>> GetFileBytesAsync(string itemId, string userId)
    {
        try
        {
            var userFound = await _userRepository.GetById(userId);
            if (userFound == null)
                return new Result<byte[]>().Failure("Usuário não encontrado", null, 404);

            var fileBytes = await _fileService.GetFileBytesAsync(itemId);
            if (!fileBytes.IsSuccess)
                return new Result<byte[]>().Failure(fileBytes);

            return new Result<byte[]>().Success(fileBytes);
        }
        catch (DomainException ex)
        {
            return new Result<byte[]>().Failure(ex.Message, null, 400);
        }
        catch (InfrastructureException ex)
        {
            return new Result<byte[]>().Failure(ex.Message, null, 500);
        }
    }

    public async Task<Result<string>> DeleteFileAsync(string itemId, string userId)
    {
        try
        {
            var userFoud = await _userRepository.GetById(userId);
            if (userFoud == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            var itemFound = await _tavernRepository.FindItemByIdAsync(itemId);
            if (itemFound == null)
                return new Result<string>().Failure("Item não encontrado", null, 404);

            itemFound.Delete();

            await _tavernRepository.RemoveItemFromFolderAsync(itemFound);

            var deleteItemRequest = await _fileService.DeleteItemFromDisk(itemId);
            if (!deleteItemRequest.IsSuccess)
                return new Result<string>().Failure(deleteItemRequest);

            return new Result<string>().Success("Arquivo deletado com sucesso", null, 201);

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
