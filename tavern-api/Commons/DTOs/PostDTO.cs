namespace tavern_api.Commons.DTOs;

public record PostDTO
{
    public string PostId { get; set; }  
    public string PostTitle { get; set; }
    public string PostContent { get; set; }
    public string? PostImageUrl { get; set; }
    public List<LikeDTO> Likes { get; set; }
    public List<CommentDTO> Comments { get; set; }
    public string MembershipUserId { get; set; }
    public string MembershipUsername { get; set; }
    public string TavernId { get; set; }
    public string? UserWhoPostedImage { get; set; }
    public bool UserAlreadyLiked { get; set; } = false;
    
}
