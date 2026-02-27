using Microsoft.EntityFrameworkCore;
using tavern_api.Commons.Exceptions;
using tavern_api.Database;

namespace tavern_api.Commons;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    private readonly TavernDbContext _context;

    public BaseRepository(TavernDbContext context)
    {
        _context = context;
    }

    public async Task<T> GetById(string id)
    {
        try
        {
            var found = await _context
            .Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

            return found;

        } catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }

    public async Task<T> CreateAsync(T entity)
    {
        try
        {
            var entityValue = await _context.Set<T>().AddAsync(entity);

            await _context.SaveChangesAsync();
            return entityValue.Entity;

        } catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }

    public async  Task<T> UpdateAsync(T entity)
    {
        try
        {
            var entityValue = _context.Set<T>().Update(entity);

            await _context.SaveChangesAsync();
            return entityValue.Entity;

        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }

    public async Task RemoveAsync(T entity)
    {
        try
        {
            entity.IsDeleted = true;
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex) 
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }
}
