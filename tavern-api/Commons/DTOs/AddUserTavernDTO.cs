using tavern_api.Commons.Enums;

namespace tavern_api.Commons.DTOs;

public record AddUserTavernDTO
{
    public string TavernId { get; set; }
    public string Username { get; set; }
    public string Discriminator { get; set; }
    public MembershipStatus Status { get; set; }
}
