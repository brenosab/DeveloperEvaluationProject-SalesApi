using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Common
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, new()
    {
        private readonly DefaultContext _context;

        /// <summary>
        /// Initializes a new instance of BaseRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public BaseRepository(DefaultContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Creates a new entity in the database
        /// </summary>
        /// <param name="entity">The entity to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created entity</returns>
        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        /// <summary>
        /// Retrieves a entity by their unique identifier
        /// </summary>
        /// <param name="id">The unique identifier of the entity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="includes">
        /// Expressions indicating the related entities to include in the query.
        /// Use this to eager-load navigation properties.
        /// </param>
        /// <returns>The entity if found, null otherwise</returns>
        public async Task<TEntity?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id, cancellationToken);
        }
        /// <summary>
        /// Deletes a entity from the database
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the entity was deleted, false if not found</returns>
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken);
            if (entity == null)
                return false;

            _context.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        /// <summary>
        /// Updates a entity from the database
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete</param>
        /// <param name="entity">The entity to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if the entity was updated, false if not found</returns>
        public async Task<bool> UpdateAsync(Guid id, TEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
