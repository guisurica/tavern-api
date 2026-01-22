using Microsoft.EntityFrameworkCore;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.UserContracts;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Helpers;
using tavern_api.Commons.Responses;
using tavern_api.Entities;

namespace tavern_api.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITavernRepository _tavernRepository;

    public UserService(IUserRepository userRepository, ITavernRepository tavernRepository)
    {
        _userRepository = userRepository;
        _tavernRepository = tavernRepository;
    }

    public async Task<Result<string>> ChangeUsernameAsync(ChangeUsernameDTO input, string id)
    {
        try
        {
            var userFound = await _userRepository.GetById(id);
            if (userFound == null)
                return new Result<string>().Failure("Usuário não encontrado", null, 404);

            userFound.ChangeUsername(input.NewUsername);

            await _userRepository.UpdateAsync(userFound);

            return new Result<string>().Success("Nome de usuário atualizado com sucesso.", string.Empty, 200);
        }
        catch (InfrastructureException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            return new Result<string>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<UserDTO>> CreateUserAsync(CreateUserDTO input)
    {
        try
        {
            var userAlreadyExists = await _userRepository.GetByEmailAsync(input.Email);
            if (userAlreadyExists != null) 
                return new Result<UserDTO>().Failure("Email já cadastrado", null, 409);

            var userCreated = User.Create(input.Username, input.Email, input.Password);

            var userSaved = await _userRepository.CreateUserAsync(userCreated);

            var userDTO = new UserDTO
            {
                Email = userSaved.Email,
                ProfilePicture = userSaved.ProfilePicture,
                Username = userSaved.Username,
                Id = userSaved.Id.ToString(),
                Discriminator = userSaved.Discriminator
            };

            return new Result<UserDTO>().Success("Usuário criado com sucesso", userDTO, 201);
        }
        catch(InfrastructureException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<UserDTO>> GetUserProfileAsync(string id)
    {
        try
        {
            var userFound = await _userRepository.GetById(id);
            if (userFound == null)
                return new Result<UserDTO>().Failure("Usuário não encontrado", null, 404);

            var taverns = await _tavernRepository.GetAllUserTavernUserAsync(id);

            var userDTO = new UserDTO
            {
                Email = userFound.Email,
                Id = userFound.Id.ToString(),
                ProfilePicture = userFound.ProfilePicture,
                Username = userFound.Username,
                Taverns = taverns
            };

            return new Result<UserDTO>().Success("Usuário criado com sucesso", userDTO, 201);
        }
        catch (InfrastructureException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<List<TavernDTO>>> GetUserTavernsAsync(string id)
    {
        try
        {
            var allUsersMemberships = await _tavernRepository.GetAllUserMembershipsAsync(id);
            if (allUsersMemberships.Count <= 0) return new Result<List<TavernDTO>>().Success("Usuário não possui tavernas", new List<TavernDTO>(), 200);

            return new Result<List<TavernDTO>>().Success("", allUsersMemberships, 200);
        }
        catch (ArgumentException ex)
        {
            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 404);
        }
        catch (InfrastructureException ex)
        {
            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            return new Result<List<TavernDTO>>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<UserDTO>> LoginUserAsync(LoginUserDTO input)
    {
        try
        {
            var userFounded = await _userRepository.GetByEmailAsync(input.Email);
            if (userFounded == null)
                return new Result<UserDTO>().Failure("Usuário não encontrado", null, 404);

            userFounded.ComparePasswordHash(input.Password);

            var userDTO = new UserDTO
            {
                Email = userFounded.Email,
                ProfilePicture = userFounded.ProfilePicture,
                Username = userFounded.Username,
                Id = userFounded.Id.ToString()
            };

            return new Result<UserDTO>().Success("Usuário logado com sucesso", userDTO, 200);
        } 
        catch (ArgumentException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 404);
        } 
        catch (InfrastructureException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            return new Result<UserDTO>().Failure(ex.Message, null, 400);
        }
    }
}
