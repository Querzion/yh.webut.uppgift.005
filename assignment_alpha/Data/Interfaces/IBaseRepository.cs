using System.Linq.Expressions;

namespace Data.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : class
{
    public Task<bool> CreateAsync(TEntity entity);
    public Task<IEnumerable<TEntity>?> GetAllAsync();
    public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression);
    public Task<bool> UpdateAsync(TEntity updatedEntity);
    public Task<bool> DeleteAsync(TEntity entity);
    public Task<bool> AlreadyExistsAsync(Expression<Func<TEntity, bool>> expression);

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}