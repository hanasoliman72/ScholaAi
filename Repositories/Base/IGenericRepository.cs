namespace ScholaAi.Repositories.Base
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> getByIdAsync(string id);
        Task<IEnumerable<T>> getAllAsync();
        Task AddAsync(T entity);
        Task updateAsync(T entity);
        Task deleteAsync(T entity);
    }
}
