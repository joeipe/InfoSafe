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

        public async Task<TEntity> FindAsync(int id)
        {
            return await _dataContext.db.GetAsync<TEntity>(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dataContext.db.GetAllAsync<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> SearchForAsync(string query, object parameters = null!)
        {
            return await _dataContext.db.QueryAsync<TEntity>(query, parameters);
        }
    }
}