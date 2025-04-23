using System.Diagnostics;
using System.Linq.Expressions;
using Data.Contexts;
using Data.Interfaces;
using Data.Models;
using Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Repositories;

public abstract class BaseRepository<TEntity, TModel> : IBaseRepository<TEntity, TModel> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _table;
    private IDbContextTransaction _transaction = null!;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }

    #region CRUD Methods

        public virtual async Task<RepositoryResult<bool>> AddAsync(TEntity entity)
        {
            if (entity == null)
                return new RepositoryResult<bool> { Succeeded = false, StatusCode = 400, Error = "Entity cannot be null" };
        
            try
            {
                _table.Add(entity);
                await _context.SaveChangesAsync();
                return new RepositoryResult<bool> { Succeeded = true, StatusCode = 201 };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new RepositoryResult<bool> { Succeeded = false, StatusCode = 500, Error = ex.Message };
            }
        }
        
        public virtual async Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync( bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes )
        {
            /* GetAllAsync(settings) SQL equalent ex.
             *  SELECT
             *  * (All) or something like this
             *      Projects.Id,
             *      Projects.ProjectName,
             *      Statuses.StatusName
             *  FORM table (Projects)
             *  WHERE StatusId = 1
             *  ORDER BY DESC
             *  SORT BY Created/Started/Completed
             *
             *  JOIN Clients ON ...
             *  JOIN Statuses ON ...
             *  JOIN AspNetUsers ON ...
             */
            
            IQueryable<TEntity> query = _table;
            if (where != null)
                query = query.Where(where);
            
            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);
            
            if (sortBy != null)
                query = orderByDecending
                    ? query.OrderByDescending(sortBy)
                    : query.OrderBy(sortBy);
            
            var entities = await query.ToListAsync();
            var result = entities.Select(entity => entity.MapTo<TModel>());
            return new RepositoryResult<IEnumerable<TModel>> { Succeeded = true, StatusCode = 200, Result = result };
        }
        
        public virtual async Task<RepositoryResult<IEnumerable<TSelect>>> GetAllAsync<TSelect>(Expression<Func<TEntity, TSelect>> selector, bool orderByDecending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes )
        {
            /* GetAllAsync(settings) SQL equalent ex.
             *  SELECT
             *  * (All) or something like this
             *      Projects.Id,
             *      Projects.ProjectName,
             *      Statuses.StatusName
             *  FORM table (Projects)
             *  WHERE StatusId = 1
             *  ORDER BY DESC
             *  SORT BY Created/Started/Completed
             *
             *  JOIN Clients ON ...
             *  JOIN Statuses ON ...
             *  JOIN AspNetUsers ON ...
             */
            
            IQueryable<TEntity> query = _table;
            if (where != null)
                query = query.Where(where);
            
            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);
            
            if (sortBy != null)
                query = orderByDecending
                    ? query.OrderByDescending(sortBy)
                    : query.OrderBy(sortBy);
            
            var entities = await query.Select(selector).ToListAsync();
            var result = entities.Select(entity => entity!.MapTo<TSelect>());
            return new RepositoryResult<IEnumerable<TSelect>> { Succeeded = true, StatusCode = 200, Result = result };
        }

        
        public virtual async Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table;
            
            if (includes != null && includes.Length != 0)
                foreach (var include in includes)
                    query = query.Include(include);
            
            // Added ! extra that the teacher didn't add.
            var entity = await query.FirstOrDefaultAsync(where!);
            if (entity == null)
                return new RepositoryResult<TModel>
                    { Succeeded = false, StatusCode = 404, Error = "Entity not found." };

            var result = entity.MapTo<TModel>();
            return new RepositoryResult<TModel> { Succeeded = true, StatusCode = 200, Result = result };
        }

        public virtual async Task<RepositoryResult<TEntity>> GetEntityAsync(Expression<Func<TEntity, bool>> where,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _table;

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            var entity = await query.FirstOrDefaultAsync(where);
            if (entity == null)
                return new RepositoryResult<TEntity>
                    { Succeeded = false, StatusCode = 404, Error = "Entity not found." };

            return new RepositoryResult<TEntity> { Succeeded = true, StatusCode = 200, Result = entity };
        }

        public virtual async Task<RepositoryResult<bool>> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                return new RepositoryResult<bool> { Succeeded = false, StatusCode = 400, Error = "Entity cannot be null" };

            try
            {
                _table.Update(entity);
                await _context.SaveChangesAsync();
                return new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new RepositoryResult<bool> { Succeeded = false, StatusCode = 500, Error = ex.Message };
            }
        }

        
        public virtual async Task<RepositoryResult<bool>> DeleteAsync(TEntity entity)
        {
            if (entity == null)
                return new RepositoryResult<bool> { Succeeded = false, StatusCode = 400, Error = "Entity cannot be null" };

            try
            {
                _table.Remove(entity);
                await _context.SaveChangesAsync();
                return new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new RepositoryResult<bool> { Succeeded = false, StatusCode = 500, Error = ex.Message };
            }
        }


    #endregion
    
    public virtual async Task<RepositoryResult<TEntity?>> FindEntityAsync(Expression<Func<TEntity, bool>> findBy)
    {
        var entity = await _table.FirstOrDefaultAsync(findBy);
        if (entity == null)
            return new RepositoryResult<TEntity?> { Succeeded = false, StatusCode = 404, Error = "Entity not found." };

        return new RepositoryResult<TEntity?> { Succeeded = true, StatusCode = 200, Result = entity };
    }
    
    public virtual async Task<RepositoryResult<bool>> ExistsAsync(Expression<Func<TEntity, bool>> findBy)
    {
        var exists = await _table.AnyAsync(findBy);
        return !exists
            ? new RepositoryResult<bool> { Succeeded = false, StatusCode = 404, Error = "Entity not found." }
            : new RepositoryResult<bool> { Succeeded = true, StatusCode = 200 };
    }
    
    public virtual async Task<RepositoryResult<bool>> SaveChangesAsync()
    {
        try
        {
            var changes = await _context.SaveChangesAsync();
            return new RepositoryResult<bool>
            {
                Succeeded = changes > 0,
                StatusCode = changes > 0 ? 200 : 204
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult<bool>
            {
                Succeeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }
    
    #region Transaction Management Methods

        public virtual async Task BeginTransactionAsync()
        {
            _transaction ??= await _context.Database.BeginTransactionAsync();
        }
    

        public virtual async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

        public virtual async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
        }

    #endregion
}
