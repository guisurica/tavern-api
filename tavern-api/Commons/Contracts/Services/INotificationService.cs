using tavern_api.Commons.DTOs;
using tavern_api.Commons.Responses;

namespace tavern_api.Commons.Contracts.Services;

public interface INotificationService
{
    Task<Result<List<NotificationDTO>>> GetAllUserReceivedNotifcationsAsync(string userEmail);
    Task<Result<string>> SeenNotificationAsync(string id);
}