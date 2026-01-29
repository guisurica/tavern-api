namespace tavern_api.Commons.DTOs;
public record CommentDTO
{
    public string CommentId { get; set; } 
    public string CommentContent { get; set; }
    public string? ParentCommentId { get; set; }
    public string UserWhoCommentId { get; set; }
    public string UserWhoCommentUsername { get; set;}
}
