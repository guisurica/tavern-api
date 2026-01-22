using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using tavern.tests.Fixtures;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.UserContracts;
using tavern_api.Commons.DTOs;
using tavern_api.Entities;
using tavern_api.Services;

namespace tavern.tests.Tests.Services;

public class UserServicesTests
{

    private readonly IUserService _userService;

    private readonly IUserRepository _userRepositoryMock;
    private readonly ITavernRepository _tavernRepositoryMock;

    public UserServicesTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _tavernRepositoryMock = Substitute.For<ITavernRepository>();

        _userService = new UserService(_userRepositoryMock, _tavernRepositoryMock);
    }

    [Fact]
    public async Task LoginUserAsync_Should_ReturnError_UserNotFound()
    {
        var input = UserServiceFixtures.UserWithEmailThatWillBeNotFound;

        var result = await _userService.LoginUserAsync(input);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(input.Email);

        result.Message.Should().Be("Usuário não encontrado");
        result.Data.Should().Be(null);
        result.Code.Should().Be(404);
    }

    [Fact]
    public async Task GetUserProfileAsync_Should_ReturnError_UserNotFound()
    {
        var input = UserServiceFixtures.UserThatWilBeNotFoundId;

        var result = await _userService.GetUserProfileAsync(input);

        await _userRepositoryMock.Received(1)
            .GetById(input);

        result.Message.Should().Be("Usuário não encontrado");
        result.Data.Should().Be(null);
        result.Code.Should().Be(404);
    }

    [Fact]
    public async Task CreateUserAsync_Should_ReturnError_EmailAlreadyInUse() 
    {
        var input = UserServiceFixtures.UserWithEmailAlreadyRegistered;

         _userRepositoryMock.GetByEmailAsync(input.Email)
            .Returns(User.Create("guilherme", "guilherme@hotmail.com", "password"));

        var result = await _userService.CreateUserAsync(input);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(input.Email);

        result.Message.Should().Be("Email já cadastrado");
        result.Data.Should().Be(null);
        result.Code.Should().Be(409);
    }

    [Fact]
    public async Task ChangeUsernameAsync_Should_ReturnFailure_UserNotFound()
    {
        var inputId = UserServiceFixtures.UserThatWilBeNotFoundId;
        var input = UserServiceFixtures.UsernameChangeDTO;

        _userRepositoryMock
            .GetById(inputId)
            .ReturnsNull();

        var result = await _userService.ChangeUsernameAsync(input, inputId);

        _userRepositoryMock.Received(1)
            .GetById(inputId);

        result.Data.Should().Be(null);
        result.Message.Should().Be("Usuário não encontrado");
        result.Code.Should().Be(404);

    }

    [Fact]
    public async Task CreateUserAsync_Should_ReturnSuccess_UserCreatedSuccessfully()
    {
        var input = UserServiceFixtures.CreateUserAsyncSuccessDTO;
        var dummyUserCreated = User.Create(input.Username, input.Email, input.Password);

        _userRepositoryMock
            .GetByEmailAsync(input.Email)
            .ReturnsNull();

        _userRepositoryMock
            .CreateUserAsync(Arg.Any<User>())
            .Returns(dummyUserCreated);

        var result = await _userService.CreateUserAsync(input);

        _userRepositoryMock
            .Received(1)
            .CreateUserAsync(Arg.Any<User>());

        _userRepositoryMock
            .Received(1)
            .GetByEmailAsync(input.Email);

        result.Data.Should().BeOfType<UserDTO>();
        result.Code.Should().Be(201);
        result.Message.Should().Be("Usuário criado com sucesso");
    }

}
