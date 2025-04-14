using System.Linq.Expressions;
using Data.Models;

namespace Data.Interfaces;

public interface IBaseRepository<TEntity, TModel> where TEntity : class
{
    Task<RepositoryResult<bool>> AddAsync(TEntity entity);
    Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync( bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes );
    Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector, bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes );
    Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> findBy);
    Task<RepositoryResult<bool>> UpdateAsync(TEntity entity);
    Task<RepositoryResult<bool>> DeleteAsync(TEntity entity);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}