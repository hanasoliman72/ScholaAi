namespace ScholaAi.Repositories.Base
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> getByIdAsync(int id);
        Task<IEnumerable<T>> getAllAsync();
        Task addAsync(T entity);
        Task updateAsync(T entity);
        Task deleteAsync(T entity);
    }
}
