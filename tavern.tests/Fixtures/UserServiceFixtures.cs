using tavern_api.Commons.DTOs;

namespace tavern.tests.Fixtures;

public static class UserServiceFixtures
{
    public static LoginUserDTO UserWithEmailThatWillBeNotFound = new LoginUserDTO
    {
        Email = "foobar@hotmail.com",
        Password = "dummypass",
    };

    public static string UserThatWilBeNotFoundId = "SomeId";

    public static CreateUserDTO UserWithEmailAlreadyRegistered = new CreateUserDTO
    {
        Email = "foobar@hotmail.com",
        Password = "dummypass",
        Username = "foobar"
    };

    public static ChangeUsernameDTO UsernameChangeDTO = new ChangeUsernameDTO
    {
        NewUsername = "foobar"
    };

    public static CreateUserDTO CreateUserAsyncSuccessDTO = new CreateUserDTO
    {
        Email = "foo@bar.com",
        Password = "dummypass",
        Username = "dummyuser"
    };

    public static UserDTO UserDTO = new UserDTO
    {
        Discriminator = "9223",
        Email = "foo@bar.com",
        Id = "ad638386-9ace-470c-879d-833be858bad6",
        ProfilePicture = null,
        Taverns = null,
        Username = "dummyuser"
    };
}
