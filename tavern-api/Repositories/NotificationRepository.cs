using Microsoft.EntityFrameworkCore;
using tavern_api.Commons;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Database;
using tavern_api.Entities;
using tavern_api.Migrations;

namespace tavern_api.Repositories;

sealed internal class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    private readonly TavernDbContext _context;

    public NotificationRepository(TavernDbContext context) : base(context)
    {
        _context = context;
    }

    public Task<List<NotificationDTO>> GetAllUserNotification(string userId)
    {
        try
        {
            return _context
                .Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    NotificationMessage = n.NotificationMessage,
                    NotificationType = n.NotificationType,
                    TavernId = n.TavernId,
                    UserReceiverEmail = n.UserReceiverEmail,
                })
                .ToListAsync();
        } catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }

    public Task<List<NotificationDTO>> GetAllUserReceivedNotification(string userReceivedEmail)
    {
        try
        {
            return _context
                .Notifications
                .AsNoTracking()
                .Where(n => n.UserReceiverEmail == userReceivedEmail)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    UserId = n.UserId,
                    NotificationMessage = n.NotificationMessage,
                    NotificationType = n.NotificationType,
                    TavernId = n.TavernId,
                    UserReceiverEmail = n.UserReceiverEmail,
                    AlreadySeen = n.AlreadySeen
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }
}
