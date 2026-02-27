using tavern_api.Commons;

namespace tavern_api.Entities;

public enum NotificationType
{
    NewComment,
    NewPost,
    NewInviteOrder
}

public class Notification : BaseEntity
{
    public string UserReceiverEmail { get; private set; } = string.Empty;
    public string TavernId { get; private set; } = string.Empty;
    public Tavern Tavern { get; private set; } = null!;
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = null!;
    public NotificationType NotificationType { get; private set; } = NotificationType.NewInviteOrder;
    public string NotificationMessage { get; private set; } = string.Empty;
    public bool AlreadySeen { get; private set; } = false;

    private Notification() { }

    private Notification(string notificationMessage, string receiverEmail, string tavernId, string userId, NotificationType type)
    {
        NotificationMessage = notificationMessage;
        UserReceiverEmail = receiverEmail;
        TavernId = tavernId;
        UserId = userId;
        NotificationType = type;
    }

    public static Notification Create(string notificationMessage, string receiverEmail, string tavernId, string userId, NotificationType type)
    {
        return new Notification(notificationMessage, receiverEmail, tavernId, userId, type);
    }

    public void SeeNotification()
    {
        this.AlreadySeen = true;
    }

}
