using Dapper;
using Dapper.Contrib.Extensions;

namespace InfoSafe.Read.Data
{
    public class DapperGenericRepository<TEntity> : IDapperGenericRepository<TEntity> where TEntity : class
    {
        private readonly ReadDbContext _dataContext;

        public DapperGenericRepository(
            ReadDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public virtual async Task CreateAsync(TEntity item)
        {
            await _dataContext.db.InsertAsync(item);
        }

        public virtual async Task UpdateAsync(TEntity item)
        {
            await _dataContext.db.UpdateAsync(item);
        }

        public virtual async Task DeleteAsync(TEntity item)
        {
            await _dataContext.db.DeleteAsync(item);
        }

        public virtual async Task<TEntity> FindAsync(int id)
        {
            return await _dataContext.db.GetAsync<TEntity>(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dataContext.db.GetAllAsync<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> SearchForAsync(string query, object parameters = null!)
        {
            return await _dataContext.db.QueryAsync<TEntity>(query, parameters);
        }
    }
}