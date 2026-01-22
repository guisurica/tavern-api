using System.Collections.Generic;
using System.Threading.Tasks;
using tavern_api.Entities;

namespace tavern_api.Commons.Contracts.Repositories;

public interface IGameDayRepository : IBaseRepository<GameDay>
{
    Task<List<GameDay>> GetByTavernIdAsync(string tavernId);
}