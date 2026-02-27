using tavern_api.Commons.DTOs;
using tavern_api.Entities;

namespace tavern_api.Commons.Contracts.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<List<NotificationDTO>> GetAllUserNotification(string userId);
    Task<List<NotificationDTO>> GetAllUserReceivedNotification(string userReceivedEmail);
}
