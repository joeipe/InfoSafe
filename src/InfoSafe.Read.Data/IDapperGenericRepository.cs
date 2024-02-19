namespace InfoSafe.Read.Data
{
    public interface IDapperGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> FindAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<IEnumerable<TEntity>> SearchForAsync(string query, object parameters = null!);
    }
}