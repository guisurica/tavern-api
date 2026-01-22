using FluentAssertions;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json;
using tavern.tests.Factory;
using tavern.tests.Fixtures;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern.tests.IntegrationTests;

public class AuthControllerIntegrationTests : IClassFixture<TavernApiFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(TavernApiFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginUserAsync_WhenCalled_ReturnsUserInformation()
    {
        var loginInput = UserControllerFixtures.LoginUserDTO;

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginInput);

        var responseString = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Result<UserDTO>>(responseString);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        result.Data.Should().NotBe(null);
        result.Code.Should().Be(200);
        result.Message.Should().Be("Usuário logado com sucesso");
    }

    [Fact]
    public async Task RegisterUserAsync_WhenCalled_ReturnsUserCreated()
    {
        var registerInput = UserControllerFixtures.RegisterUserDTO;

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerInput);

        var responseString = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Result<UserDTO>>(responseString);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        result.Data.Should().NotBe(null);
        result.Code.Should().Be(201);
        result.Message.Should().Be("Usuário criado com sucesso");

    }
}
