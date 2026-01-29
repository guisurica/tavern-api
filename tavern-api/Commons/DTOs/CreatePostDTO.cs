namespace tavern_api.Commons.DTOs;

public record CreatePostDTO
{
    public string PostTitle { get; set; }
    public string PostContent { get; set; }
    public IFormFile PostImage { get; set; }
    public string TavernId { get; set; }
    public string UserId { get; set; }

}
