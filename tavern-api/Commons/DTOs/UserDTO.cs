namespace tavern_api.Commons.DTOs;

public record UserDTO
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? ProfilePicture { get; set; } = null;
    public string Id { get; set; } = null!;
    public List<TavernUserDTO> Taverns { get; set; }
    public string Discriminator { get; set; } = null!;

}
