using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContactApp.Data.Entities;

namespace ContactApp.Repository
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="T">Primitive type for Id of Entity</typeparam>
    public partial interface IRepository<TEntity,T> where TEntity : BaseEntity<T> where T : struct
    {
        IQueryable<TEntity> Table { get; }

        Task<TEntity> GetById(object id, CancellationToken token = default);

        Task<int> InsertAsync(TEntity entity, CancellationToken token = default);

        Task<int> UpdateAsync(TEntity entity, CancellationToken token = default);

        Task<int> DeleteAsync(TEntity entity, CancellationToken token = default);
    }
}