using tavern_api.Commons.Enums;

namespace tavern_api.Commons.DTOs;

public record UserTavernDTO
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Discriminator { get; set; } 
    public MembershipStatus StatusInTavern { get; set; }
    public bool IsDm { get; set; }
    public string? ProfilePicture { get; set; }
    public string MembershipId { get; set; }
}
