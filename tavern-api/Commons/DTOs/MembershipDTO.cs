using tavern_api.Commons.Enums;

namespace tavern_api.Commons.DTOs;

public record MembershipDTO
{
    public string TavernId { get; set; }
    public string UserId { get; set; }
    public bool IsDm { get; set; }
    public MembershipStatus Status { get; set; }
}
