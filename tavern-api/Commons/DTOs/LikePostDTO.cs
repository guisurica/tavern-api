namespace tavern_api.Commons.DTOs;

public record LikePostDTO
{
    public string PostId { get; set; }
    public string MembershipId { get; set; }
}
