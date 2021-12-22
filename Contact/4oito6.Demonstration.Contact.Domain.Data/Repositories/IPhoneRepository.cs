using _4oito6.Demonstration.Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Data.Repositories
{
    public interface IPhoneRepository : IDisposable
    {
        Task<IEnumerable<Phone>> GetAllAsync();

        Task<IEnumerable<Phone>> GetByNumberAsync(IEnumerable<Phone> phones);

        Task DeleteWithoutPersonAsync();
    }
}