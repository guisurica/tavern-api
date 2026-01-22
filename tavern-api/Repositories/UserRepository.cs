using Microsoft.EntityFrameworkCore;
using tavern_api.Commons;
using tavern_api.Commons.Contracts.Repositories;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Helpers;
using tavern_api.Database;
using tavern_api.Entities;

namespace tavern_api.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly TavernDbContext _context;

    public UserRepository(TavernDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(User entity)
    {
        for(int attempt = 0; attempt < User.MAX_DISCRIMINATOR_ATTEMPTS; attempt++)
        {
            try
            {
                var user = await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
                return user.Entity;
            } catch (DbUpdateException ex) when (IsUniqueViolation.Execute(ex))
            {
                _context.Users.Entry(entity).State = EntityState.Detached;
            }
        }

        throw new InfrastructureException("Ocorreu um problema ao criar o usuário. Tente novamente mais tarde. Se persistir, contate o suporte.");
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        try
        {
            var entity = _context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Email == email);

            return entity;

        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }

    public Task<User> GetByNameAndDiscriminator(string username, string discriminator)
    {
        try
        {
            var entity = _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username && u.Discriminator == discriminator);

            return entity;

        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Ocorreu um problema. Tente novamente mais tarde. Se persistir, contate o suporte.");
        }
    }
}
