using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContactApp.Data.Entities;
using ContactApp.Models;
using ContactApp.Repository;
using Microsoft.EntityFrameworkCore;

namespace ContactApp.Services
{
    public class ContactService : IContactService
    {
        private readonly IRepository<Contact, int> _contactRepository;

        public ContactService(IRepository<Contact, int> contactRepository)
        {
            _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        }

        public async Task<Contact> GetById(int id,  CancellationToken token = default)
        {
            return await _contactRepository.Table
                                            .Include(x=>x.Phones)
                                            .FirstOrDefaultAsync(x=>x.Id == id, cancellationToken: token)
                                            .ConfigureAwait(false);

        }
    }
}