namespace tavern_api.Commons.DTOs;

public record CreateCommentDTO
{
    public string PostId { get; set; }
    public string TavernId { get; set; }
    public string UserId { get; set; }
    public string CommentContent { get; set; }
    public string? ParentCommentId { get; set; }
}
