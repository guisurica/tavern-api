namespace tavern_api.Commons.DTOs;

public record LikeDTO
{
    public string LikeId { get; set; }
    public string UserWhoLikedId { get; set; }
    public string UserWhoLikedUsername { get; set; }
    public string MembershipId { get; set; }
    public string TavernId { get; set; }
}
