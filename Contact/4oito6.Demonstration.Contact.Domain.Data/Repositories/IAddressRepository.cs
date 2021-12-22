using _4oito6.Demonstration.Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Data.Repositories
{
    public interface IAddressRepository : IDisposable
    {
        Task<IEnumerable<Address>> GetAllAsync();

        Task<Address> GetByAddressAsync(Address address);

        Task DeleteWithoutPersonAsync();

        Task CloneAsync(IEnumerable<Address> addresses);
    }
}