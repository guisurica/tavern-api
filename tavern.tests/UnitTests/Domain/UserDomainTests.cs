using FluentAssertions;
using tavern.tests.Fixtures;
using tavern_api.Commons.Exceptions;
using tavern_api.Entities;

namespace tavern.tests.Tests.Domain;

public class UserDomainTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void VerifyPassword_Should_ThrowDomainException_When_PasswordEmptyOrInvalid(string password)
    {
        Action act = () => User.Create(UserDomainFixtures.ValidUsername, UserDomainFixtures.ValidEmail, password);

        act.Should().Throw<DomainException>()
            .WithMessage("Senha não pode ser vazia");
    }

    [Fact]
    public void VerifyPassword_Should_ThrowDomainException_When_PasswordIsTooSmall() 
    {
        Action act = () => User.Create(UserDomainFixtures.ValidUsername, UserDomainFixtures.ValidEmail, UserDomainFixtures.ShortPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Senha deve ter no mínimo 6 caracteres");
    }

    [Fact]
    public void VerifyPassword_Should_ThrowDomainException_When_PasswordIsTooLong()
    {
        Action act = () => User.Create(UserDomainFixtures.ValidUsername, UserDomainFixtures.ValidEmail, UserDomainFixtures.LongPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Senha deve ter no máximo 100 caracteres");
    }

    [Fact]  
    public void VerifyEmail_Should_ThrowDomainException_When_EmailEmptyOrNull()
    {
        Action act = () => User.Create(UserDomainFixtures.ValidUsername, UserDomainFixtures.EmptyEmail, UserDomainFixtures.ShortPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Email não pode ser vazio");
    }

    [Fact]
    public void VerifyEmail_Should_ThrowDomainException_When_EmailIsInvalid()
    {
        Action act = () => User.Create(UserDomainFixtures.ValidUsername, UserDomainFixtures.InvalidEmail, UserDomainFixtures.ShortPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Email inválido");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    public void VerifyUsername_Should_Throw_When_UsernameIsNullOrEmpty(string username)
    {
        Action act = () => User.Create(username, UserDomainFixtures.ValidEmail, UserDomainFixtures.ValidPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Username não pode ser vazio");
    }

    [Fact]
    public void VerifyUsername_Should_Throw_When_UsernameIsTooShort()
    {
        Action act = () => User.Create(UserDomainFixtures.ShortUsername, UserDomainFixtures.ValidEmail, UserDomainFixtures.ValidPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Username deve ter no mínimo 3 caracteres");
    }

    [Fact]
    public void VerifyUsername_Should_Throw_When_UsernameIsTooLong()
    {
        Action act = () => User.Create(UserDomainFixtures.LongUsername, UserDomainFixtures.ValidEmail, UserDomainFixtures.ValidPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Username deve ter no máximo 30 caracteres");
    }

    [Fact]
    public void VerifyUsername_Should_Throw_When_UsernameHasNotAllowedChars()
    {
        Action act = () => User.Create(UserDomainFixtures.UsernameWithInvalidChars, UserDomainFixtures.ValidEmail, UserDomainFixtures.ValidPassword);

        act.Should().Throw<DomainException>()
            .WithMessage("Username deve conter apenas letras, números e underscore");
    }
}

