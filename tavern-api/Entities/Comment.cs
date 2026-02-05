using tavern_api.Commons;

namespace tavern_api.Entities;

public class Comment : BaseEntity
{
    public string MembershipId { get; private set; }
    public Membership Membership { get; private set; }
    public string PostId { get; private set; }
    public Post Post { get; private set; }
    public string CommentContent { get; private set; }
    public string? ParentCommentId { get; private set; } = null;
    
    private Comment() { }

    private Comment(string membershipId, string postId, string commentContent, string? parentCommentId = null)
    {
        MembershipId = membershipId;
        PostId = postId;
        CommentContent = commentContent;
        ParentCommentId = parentCommentId;
    }

    public static Comment Create(string membershipId, string postId, string commentContent, string? parentCommentId)
    {
        return new Comment(membershipId, postId, commentContent, parentCommentId);
    }
}
