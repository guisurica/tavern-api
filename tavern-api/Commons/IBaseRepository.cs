namespace tavern_api.Commons;

public interface IBaseRepository<T> where T : BaseEntity
{
    public Task<T> GetById(string id);
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task RemoveAsync(T entity);
}
