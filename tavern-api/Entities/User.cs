using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using tavern_api.Commons;
using tavern_api.Commons.Exceptions;

namespace tavern_api.Entities;

public sealed class User : BaseEntity
{
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? ProfilePicture { get; private set; } = null;
    public string Discriminator { get; private set; } = null!;
        
    public static readonly int MAX_DISCRIMINATOR_ATTEMPTS = 20;
    public static readonly int MAX_DISCRIMINATOR = 10_000;

    private User() { }

    private User(string username, string email, string password, string discriminator)
    {
        Username = username;
        Email = email;
        PasswordHash = password;
        Discriminator = discriminator;
    }

    #region Static Methods

    public static User Create(string username, string email, string password)
    {
        VerifyUsername(username);
        VerifyEmail(email);
        VerifyPassword(password);
        
        var discriminator = GenerateDiscriminator();

        var hashedPass = HashPassword(password);

        return new User(username, email, hashedPass, discriminator);
    }

    private static string GenerateDiscriminator()
    {
        return new Random().Next(0, MAX_DISCRIMINATOR).ToString("D4");
    }

    private static void VerifyUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username não pode ser vazio");

        if (username.Length < 3)
            throw new DomainException("Username deve ter no mínimo 3 caracteres");

        if (username.Length > 30)
            throw new DomainException("Username deve ter no máximo 30 caracteres");

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
            throw new DomainException("Username deve conter apenas letras, números e underscore");
    }

    private static void VerifyEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email não pode ser vazio");

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("Email inválido");
    }

    private static void VerifyPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new DomainException("Senha não pode ser vazia");

        if (password.Length < 6)
            throw new DomainException("Senha deve ter no mínimo 6 caracteres");

        if (password.Length > 100)
            throw new DomainException("Senha deve ter no máximo 100 caracteres");
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    #endregion

    public void ChangeUsername(string newUsername)
    {
        VerifyUsername(newUsername);
        this.Username = newUsername;
    }

    public void ChangeEmail(string newEmail)
    {
        VerifyEmail(newEmail);
        this.Email = newEmail;
    }

    public void ChangePassword(string newPassword)
    {
        VerifyPassword(newPassword);
        this.PasswordHash = HashPassword(newPassword);
    }

    public void ComparePasswordHash(string password)
    {
        VerifyPassword(password);

        if (!BCrypt.Net.BCrypt.Verify(password, this.PasswordHash))
            throw new DomainException("Senha incorreta");
    }

}
