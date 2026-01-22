namespace tavern_api.Commons.DTOs;

public record LoginUserDTO
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
