using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tavern_api.Commons;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Exceptions;
using tavern_api.Database;
using tavern_api.Entities;

namespace tavern_api.Repositories;

public class GameDayRepository : BaseRepository<GameDay>, IGameDayRepository
{
    private readonly TavernDbContext _context;

    public GameDayRepository(TavernDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<GameDay>> GetByTavernIdAsync(string tavernId)
    {
        try
        {
            return await _context.GameDays
                .AsNoTracking()
                .Where(g => g.TavernId == tavernId)
                .OrderBy(g => g.ScheduledAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }
}