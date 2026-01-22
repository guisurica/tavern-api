using tavern_api.Commons;
using tavern_api.Commons.Enums;

namespace tavern_api.Entities;

public sealed class Membership : BaseEntity
{
    public bool IsDm { get; private set; }
    public bool IsActive { get; private set; }
    public MembershipStatus Status { get; private set; }
    public string TavernId { get; private set; } = null!;
    public Tavern Tavern { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public User User { get; private set; } = null!;

    private Membership() { }

    private Membership(string tavernId, string userId, bool isDm, MembershipStatus status)  
    {
        TavernId = tavernId;
        UserId = userId;
        IsDm = isDm;
        IsActive = true;
        Status = status;
    }

    public static Membership Create(string tavernId, string userId, bool isDm, MembershipStatus status)
    {
        return new Membership(tavernId, userId, isDm, status);
    }
}
