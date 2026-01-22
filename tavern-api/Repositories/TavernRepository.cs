using Microsoft.EntityFrameworkCore;
using tavern_api.Commons;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.DTOs;
using tavern_api.Commons.Exceptions;
using tavern_api.Database;
using tavern_api.Entities;

namespace tavern_api.Repositories;

public class TavernRepository : BaseRepository<Tavern>, ITavernRepository
{
    private readonly TavernDbContext _context;

    public TavernRepository(TavernDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task RemoveMembership(Membership entity)
    {
        try
        {
            _context.Memberships
                .Update(entity);

            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<Membership> CreateMembership(Membership entity)
    {
        try
        {
            
            var entitySaved = await _context.Memberships.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entitySaved.Entity;

        } catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<List<UserTavernDTO>> GetAllTavernUsers(string tavernId)
    {
        try
        {
            return await _context.Memberships
                .AsNoTracking()
                .Where(m => m.TavernId == tavernId && !m.IsDeleted)
                .Select(m => new UserTavernDTO
                {
                    Discriminator = m.User.Discriminator,
                    Id = m.User.Id,
                    StatusInTavern = m.Status,
                    Username = m.User.Username,
                    IsDm = m.IsDm
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<List<TavernDTO>> GetAllUserMembershipsAsync(string id)
    {
        try
        {
            return await _context.Memberships
                .AsNoTracking()
                .Where(m => m.UserId == id && !m.IsDeleted)
                .Select(t => new TavernDTO 
                { 
                    Description = t.Tavern.Description,
                    Name = t.Tavern.Name,
                    Id = t.Tavern.Id,
                    Capacity = t.Tavern.Capacity
                })
                .ToListAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<List<TavernUserDTO>> GetAllUserTavernUserAsync(string id)
    {
        try
        {
            return await _context.Memberships
                .AsNoTracking()
                .Where(m => m.UserId == id && !m.IsDeleted)
                .Select(t => new TavernUserDTO
                {
                    Name = t.Tavern.Name,
                    Id = t.Tavern.Id,
                })
                .ToListAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<Membership> GetUserMembershipAsync(string userId, string tavernId)
    {
        try
        {
            return await _context.Memberships
                .AsNoTracking()
                .Where(m => m.UserId == userId && m.TavernId == tavernId && !m.IsDeleted)
                .FirstOrDefaultAsync();
                
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<List<TavernGameDaysDTO>> GetAllTavernGameDaysAsync(string id)
    {
        try
        {
            return await _context.GameDays
                .AsNoTracking()
                .Where(g => g.TavernId == id && !g.IsDeleted)
                .Select(g => new TavernGameDaysDTO
                {
                    Id = g.Id,
                    Notes = g.Notes,
                    ScheduleAt = g.ScheduledAt,
                    TavernId = g.TavernId,
                    IsConcluded = g.IsConcluded,
                })
                .ToListAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<GameDay> GetGameDayByIdAsync(string gameDayId)
    {
        try
        {
            return await _context.GameDays
                .AsNoTracking()
                .Where(g => g.Id == gameDayId && !g.IsDeleted)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task UpdateGameDayAsync(GameDay entity)
    {
        try
        {
            _context.GameDays
                .Update(entity);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }
}
