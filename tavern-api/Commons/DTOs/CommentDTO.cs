namespace tavern_api.Commons.DTOs;
public record CommentDTO
{
    public string CommentId { get; set; } 
    public string CommentContent { get; set; }
    public string? ParentCommentId { get; set; }
    public string UserWhoCommentId { get; set; }
    public string UserWhoCommentUsername { get; set;}
    public string? UserWhoCommmentProfilePicture { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PostId { get; set; }
    public List<CommentDTO> Replies { get; set; }
}
