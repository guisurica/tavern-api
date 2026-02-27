using Serilog;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Responses;

namespace tavern_api.Services;

sealed public class NotificationService : INotificationService
{ 
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<List<NotificationDTO>>> GetAllUserReceivedNotifcationsAsync(string userEmail)
    {
        try
        {
            Log.Information("GetAllUserReceivedNotifcationsAsync - {email}", userEmail);

            var notifications = await _notificationRepository.GetAllUserReceivedNotification(userEmail);

            return new Result<List<NotificationDTO>>().Success(string.Empty, notifications, 200);

        } catch (InfrastructureException ex)
        {
            Log.Error(ex.Message, ex);

            return new Result<List<NotificationDTO>>().Failure(ex.Message, null, 500);
        } catch (DomainException ex)
        {
            Log.Warning("GetAllUserReceivedNotifcationsAsync - {email}", userEmail);

            return new Result<List<NotificationDTO>>().Failure(ex.Message, null, 400);
        }
    }

    public async Task<Result<string>> SeenNotificationAsync(string id)
    {
        try
        {
            Log.Information("SeenNotificationAsync - {id}", id);

            var notifications = await _notificationRepository.GetById(id);

            notifications.SeeNotification();

            await _notificationRepository.UpdateAsync(notifications);

            return new Result<string>().Success(string.Empty, "", 200);

        }
        catch (InfrastructureException ex)
        {
            Log.Error(ex.Message, ex);

            return new Result<string>().Failure(ex.Message, null, 500);
        }
        catch (DomainException ex)
        {
            Log.Warning("SeenNotificationAsync - {notificationid}", id);

            return new Result<string>().Failure(ex.Message, null, 400);
        }
    }
}

