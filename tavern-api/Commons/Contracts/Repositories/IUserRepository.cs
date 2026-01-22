using tavern_api.Entities;

namespace tavern_api.Commons.Contracts.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    public Task<User> GetByEmailAsync(string email);
    Task<User> CreateUserAsync(User entity);
    Task<User> GetByNameAndDiscriminator(string username, string discriminator);
}
