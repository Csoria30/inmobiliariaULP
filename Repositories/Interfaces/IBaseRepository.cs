namespace inmobiliariaULP.Repositories.Interfaces
{
    // Interfaz base con métodos comunes básicos
    public interface IBaseRepository<T, TKey> where T : class
    {
        Task<T> GetByIdAsync(TKey id);
        Task<int> UpdateAsync(T entity);
        Task<(IEnumerable<T> Items, int Total)> GetAllAsync(int page, int pageSize, string? search = null);
    }

    // Interfaz para entidades que usan soft delete
    public interface ISoftDeleteRepository<TKey>
    {
        Task<int> DeleteAsync(TKey id, bool estado);
    }

    // Interfaz para diferentes tipos de Add
    public interface IAddRepository<T, TResult>
    {
        Task<TResult> AddAsync(T entity);
    }
}