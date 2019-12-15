using System.Threading;
using System.Threading.Tasks;
using ContactApp.Data.Entities;

namespace ContactApp.Services
{
    public interface IContactService
    {
        Task<Contact> GetById(int id, CancellationToken token = default);
    }
}