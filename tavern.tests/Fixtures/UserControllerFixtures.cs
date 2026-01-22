using tavern_api.Commons.DTOs;

namespace tavern.tests.Fixtures;

public static class UserControllerFixtures
{
    public static LoginUserDTO LoginUserDTO = new LoginUserDTO
    {
        Email = "foo@bar.com",
        Password = "password",
    };

    public static CreateUserDTO RegisterUserDTO = new CreateUserDTO
    {
        Email = "foo@barnew.com",
        Password = "password",
        Username = "foobar"
    };
}
