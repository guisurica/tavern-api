using tavern_api.Commons;

namespace tavern_api.Entities;

public class Like : BaseEntity
{
    public string MembershipId { get; set; }
    public Membership Membership { get; set; }
    public string PostId { get; private set; }
    public Post Post { get; private set; }
    
    private Like() {}

    private Like(string membershipId, string postId)
    {
        MembershipId = membershipId;
        PostId = postId;
    }

    public static Like Create(string membershipId, string postId)
    {
        return new Like(membershipId, postId);
    }
}
