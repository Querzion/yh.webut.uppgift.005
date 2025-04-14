// using System.Diagnostics;
// using System.Linq.Expressions;
// using Data.Contexts;
// using Data.Interfaces;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Storage;
//
// namespace Data.Repositories;

// public abstract class BaseRepository<TEntity>(DataContext context) : IBaseRepository<TEntity> where TEntity : class
// {
//     private readonly DataContext _context = context;
//     private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
//     private IDbContextTransaction _transaction = null!;
//     
//     #region CRUD Methods
//     public virtual async Task<bool> CreateAsync(TEntity entity)
//     {
//         if (entity == null)
//             return false;
//
//         try
//         {
//             await _dbSet.AddAsync(entity);
//             await _context.SaveChangesAsync();
//             return true;
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine($"Error creating {nameof(TEntity)} entity :: {ex.Message}");
//             return false;
//         }
//     }
//
//     public virtual async Task<IEnumerable<TEntity>?> GetAllAsync()
//     {
//         try
//         {
//             return await _dbSet.ToListAsync();
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine(ex.Message);
//             return null;
//         }
//     }
//
//     public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> expression)
//     {
//         if (expression == null)
//             return null;
//         
//         try
//         {
//             return await _dbSet.FirstOrDefaultAsync(expression);
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine(ex.Message);
//             return null;
//         }
//     }
//     
//     public virtual async Task<bool> UpdateAsync(TEntity updatedEntity)
//     {
//         if (updatedEntity == null)
//             return false;
//
//         try
//         {
//             _dbSet.Update(updatedEntity);
//             await _context.SaveChangesAsync();
//             return true;
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine($"Error updating {nameof(TEntity)} entity :: {ex.Message}");
//             return false;
//         }
//     }
//
//     public virtual async Task<bool> DeleteAsync(TEntity entity)
//     {
//         if (entity == null)
//             return false;
//
//         try
//         {
//             _dbSet.Remove(entity);
//             await _context.SaveChangesAsync();
//             return true;
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine($"Error deleting {nameof(TEntity)} entity :: {ex.Message}");
//             return false;
//         }
//     }
//
//     public virtual async Task<bool> AlreadyExistsAsync(Expression<Func<TEntity, bool>> expression)
//     {
//         try
//         {
//             return await _dbSet.AnyAsync(expression);
//         }
//         catch (Exception ex)
//         {
//             Debug.WriteLine(ex.Message);
//             return false;
//         }
//     }
//     #endregion
//
//     #region Transaction Management Methods
//
//     public virtual async Task BeginTransactionAsync()
//     {
//         // if (_transaction != null)
//         // {
//         //     _transaction = await _context.Database.BeginTransactionAsync();
//         // }
//         _transaction ??= await _context.Database.BeginTransactionAsync();
//     }
//
//     public virtual async Task CommitTransactionAsync()
//     {
//         if (_transaction != null)
//         {
//             await _transaction.CommitAsync();
//             await _transaction.DisposeAsync();
//             _transaction = null!;
//         }
//     }
//
//     public virtual async Task RollbackTransactionAsync()
//     {
//         if (_transaction != null)
//         {
//             await _transaction.RollbackAsync();
//             await _transaction.DisposeAsync();
//             _transaction = null!;
//         }
//     }
//
//     #endregion
// }