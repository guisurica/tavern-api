using tavern_api.Entities;

namespace tavern_api.Commons.DTOs;

public class NotificationDTO
{
    public string Id { get; set; }
    public string UserReceiverEmail { get; set; } = string.Empty;
    public string TavernId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public NotificationType NotificationType { get; set; } = NotificationType.NewInviteOrder;
    public string NotificationMessage { get; set; } = string.Empty;
    public bool AlreadySeen { get; set; }
}
