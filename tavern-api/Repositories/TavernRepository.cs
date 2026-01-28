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
                    IsDm = m.IsDm,
                    ProfilePicture = m.User.ProfilePicture,
                    MembershipId = m.Id
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

    public async Task<List<FolderDTO>> GetAllUsersMembershipFolders(string id)
    {
        try
        {

            var folders = await _context.Folders
                .AsNoTracking()
                .Where(f => f.MembershipId == id && !f.IsDeleted)
                .Include(f => f.Items)
                .Include(f => f.Membership)
                    .ThenInclude(m => m.User)
                .Include(f => f.Membership)
                    .ThenInclude(m => m.Tavern)
                .ToListAsync();

            var result = folders.Select(f => new FolderDTO
            {
                AssignedUserId = f.Membership.UserId,
                FolderName = f.FolderName,
                MembershipId = f.Membership.Id,
                Id = f.Id,
                TotalItems = f.Items.Count,
                AssignedUsername = f.Membership.User.Username,
                Items = f.Items.Where(i => !i.IsDeleted).Select(i => new ItemDTO
                {
                    FolderId = f.Id,
                    FolderName = f.FolderName,
                    Id = i.Id,
                    ItemName = i.ItemName,
                    Size = i.ItemSize,
                    TavernId = f.Membership.Tavern.Id,
                    TavernName = f.Membership.Tavern.Name,
                    UserSignedInId = f.Membership.User.Id,
                    UserSignedInUsername = f.Membership.User.Username,
                    Extension = i.Extension,
                    CreatedAt = i.CreatedAt
                }).ToList()
            }).ToList();

            return result;

            // when we change the database, we'll use this approach (SQL SERVER OR POSTGRESQL)

            //return await _context.Folders
            //    .AsNoTracking()
            //    .Where(f => f.MembershipId == id)
            //    .Select(f => new FolderDTO
            //    {
            //        AssignedUserId = f.Membership.UserId,
            //        FolderName = f.FolderName,
            //        MembershipId = f.Membership.Id,
            //        Id = f.Id,
            //        TotalItems = f.Items.Count(),
            //        AssignedUsername = f.Membership.User.Username,
            //        Items = f.Items.Select(i => new ItemDTO
            //        {
            //            FolderId = f.Id,
            //            FolderName = f.FolderName,
            //            Id = i.Id,
            //            ItemName = i.ItemName,
            //            Size = i.ItemSize,
            //            TavernId = f.Membership.Tavern.Id,
            //            TavernName = f.Membership.Tavern.Name,
            //            UserSignedInId = f.Membership.User.Id,
            //            UserSignedInUsername = f.Membership.User.Username,
            //            Extension = i.Extension,
            //            CreatedAt = i.CreatedAt
            //        }).ToList()
            //    })
            //    .ToListAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<List<TavernUserDTO>> GetAllUserTavernUserAsync(string userId)
    {
        try
        {
            return await _context.Memberships
                .AsNoTracking()
                .Where(m => m.UserId == userId && !m.IsDeleted)
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

    public async Task<Folder> CreateFolderAsync(Folder entity)
    {
        try
        {
            var folder = await _context.Folders.AddAsync(entity);
            await _context.SaveChangesAsync();

            return folder.Entity;
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<Item> CreateNewFileAsync(Item entity)
    {
        try
        {
            var item = await _context.Items.AddAsync(entity);
            await _context.SaveChangesAsync();

            return item.Entity;
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<List<ItemDTO>> GetAllFileInFolderAndTavernAndSignedUser(string folderId)
    {
        try
        {
            return await _context
                .Items
                .AsNoTracking()
                .Where(i => i.FolderId == folderId && !i.IsDeleted)
                .Select(i => new ItemDTO
                {
                    FolderId = folderId,
                    FolderName = i.Folder.FolderName,
                    Id = i.Id,
                    ItemName = i.ItemName,
                    Size = i.ItemSize,
                    TavernId = i.Folder.Membership.TavernId,
                    TavernName = i.Folder.Membership.Tavern.Name,
                    UserSignedInId = i.Folder.Membership.Id,
                    UserSignedInUsername = i.Folder.Membership.User.Username
                })
                .ToListAsync();
        } catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public async Task<Folder> GetFolderByIsAsync(string folderId)
    {
        try {

            return await _context
                .Folders
                .AsNoTracking()
                .Where(f => f.Id == folderId && !f.IsDeleted)
                .FirstOrDefaultAsync();

        } catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde, se persistir, contate o suporte");
        }
    }

    public async Task<Item> FindItemByIdAsync(string itemId)
    {
        try
        {

            return await _context.Items
                .AsNoTracking()
                .Where(i => i.Id == itemId && !i.IsDeleted)
                .FirstOrDefaultAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde, se persistir, contate o suporte");
        }
    }

    public async Task RemoveItemFromFolderAsync(Item entity)
    {
        try
        {

            _context.Items.Update(entity);
            await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Ocorreu um problema. Tente novamente mais tarde, se persistir, contate o suporte");
        }
    }
}
