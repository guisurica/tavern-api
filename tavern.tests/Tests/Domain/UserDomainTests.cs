using FluentAssertions;
using tavern_api.Commons.Exceptions;
using tavern_api.Entities;

namespace tavern.tests.Tests.Domain;

public class UserDomainTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void VerifyPassword_Should_ThrowDomainException_When_PasswordEmptyOrInvalid(string password)
    {
        Action act = () => User.Create("foo", "foo@bar.com", password);

        act.Should().Throw<DomainException>()
            .WithMessage("Senha não pode ser vazia");
    }

    [Fact]
    public void VerifyPassword_Should_ThrowDomainException_When_PasswordIsTooSmall() 
    {
        Action act = () => User.Create("foo", "foo@bar.com", "123");

        act.Should().Throw<DomainException>()
            .WithMessage("Senha deve ter no mínimo 6 caracteres");
    }

    [Fact]
    public void VerifyPassword_Should_ThrowDomainException_When_PasswordIsTooLong()
    {
        Action act = () => User.Create("foo", "foo@bar.com", "1290391023901293091203901293019203910293019230192039102930192129039102390129309120390129301920391029301923019203910293019212903910239012930912039012930192039102930192301920391029301921290391023901293091203901293019203910293019230192039102930192129039102390129309120390129301920391029301923019203910293019212903910239012930912039012930192039102930192301920391029301921290391023901293091203901293019203910293019230192039102930192");

        act.Should().Throw<DomainException>()
            .WithMessage("Senha deve ter no máximo 100 caracteres");
    }

    [Fact]  
    public void VerifyEmail_Should_ThrowDomainException_When_EmailEmptyOrNull()
    {
        Action act = () => User.Create("foo", " ", "password");

        act.Should().Throw<DomainException>()
            .WithMessage("Email não pode ser vazio");
    }

    [Fact]
    public void VerifyEmail_Should_ThrowDomainException_When_EmailIsInvalid()
    {
        Action act = () => User.Create("foo", "bar", "password");

        act.Should().Throw<DomainException>()
            .WithMessage("Email inválido");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void VerifyUsername_Should_Throw_When_UsernameIsNullOrEmpty(string username)
    {
        Action act = () => User.Create(username, "bar", "password");

        act.Should().Throw<DomainException>()
            .WithMessage("Username não pode ser vazio");
    }

    [Fact]
    public void VerifyUsername_Should_Throw_When_UsernameIsTooShort()
    {
        Action act = () => User.Create("fo", "bar", "password");

        act.Should().Throw<DomainException>()
            .WithMessage("Username deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void VerifyUsername_Should_Throw_When_UsernameIsTooLong()
    {
        Action act = () => User.Create("fofofofofofofofofofofofofofofofofofofofofofofofofofofofofofofofo", "bar", "password");

        act.Should().Throw<DomainException>()
            .WithMessage("Username deve ter no máximo 30 caracteres");
    }

    [Fact]
    public void VerifyUsername_Should_Throw_When_UsernameHasNotAllowedChars()
    {
        Action act = () => User.Create("foobar$$", "bar", "password");

        act.Should().Throw<DomainException>()
            .WithMessage("Username deve conter apenas letras, números e underscore");
    }
}

