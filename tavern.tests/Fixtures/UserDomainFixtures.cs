namespace tavern.tests.Fixtures;

public static class UserDomainFixtures
{
    public static string ValidUsername = "valid_user";
    public static string ValidEmail = "valid@email.com";
    public static string ValidPassword = "validpass123";

    public static string EmptyPassword = null;
    public static string WhiteSpacePassword = " ";
    public static string ShortPassword = "123";
    public static string LongPassword = new string('a', 101);

    public static string EmptyEmail = " ";
    public static string InvalidEmail = "bar";

    public static string EmptyUsername = " ";
    public static string ShortUsername = "fo";
    public static string LongUsername = new string('a', 31);
    public static string UsernameWithInvalidChars = "foobar$$";
}
