using System.Threading;
using System.Threading.Tasks;
using ContactApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Data
{
    /// <summary>
    /// Represents DB context
    /// </summary>
    public partial interface IDbContext
    {
        DbSet<TEntity> Set<TEntity,T>() where TEntity : BaseEntity<T> where T : struct;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}