using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContactApp.Data;
using ContactApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Repository
{
    /// <summary>
    /// Represents the Entity Framework repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="T">Struct of Id of Base entity</typeparam>
    public partial class EfRepository<TEntity,T> : IRepository<TEntity,T> where TEntity : BaseEntity<T> where T :struct
    {
        #region Fields

        private readonly IDbContext _context;

        private DbSet<TEntity> _entities;

        #endregion

        #region Properties

        public virtual IQueryable<TEntity> Table => Entities;


        protected virtual DbSet<TEntity> Entities => _entities ?? (_entities = _context.Set<TEntity,T>());

        #endregion

        #region Ctor

        public EfRepository(IDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public virtual async Task<TEntity> GetById(object id, CancellationToken token = default)
        {
            return await Entities.FindAsync(new object[]{id},token)
                                    .ConfigureAwait(false);
        }


        public virtual async Task<int> InsertAsync(TEntity entity, CancellationToken token = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                await Entities.AddAsync(entity,token).ConfigureAwait(false);
                return await _context.SaveChangesAsync(token).ConfigureAwait(false);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual async Task<int> UpdateAsync(TEntity entity, CancellationToken token = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Update(entity);
                return await _context.SaveChangesAsync(token).ConfigureAwait(false);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }

        public virtual  async Task<int> DeleteAsync(TEntity entity, CancellationToken token = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                Entities.Remove(entity);
                return await _context.SaveChangesAsync(token).ConfigureAwait(false);
            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetFullErrorTextAndRollbackEntityChanges(exception), exception);
            }
        }
        
        #endregion

        #region Utilities

        protected string GetFullErrorTextAndRollbackEntityChanges(DbUpdateException exception)
        {
            if (_context is DbContext dbContext)
            {
                var entries = dbContext.ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch (InvalidOperationException)
                    {
                        // ignored
                    }
                });
            }
            
            try
            { 
                var result =  _context.SaveChangesAsync().Result;
                return exception.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString(); 
            }
        }

        #endregion
    }
}