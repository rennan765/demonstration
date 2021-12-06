using _4oito6.Demonstration.Domain.Model.Entities;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Data.Repositories
{
    public interface IAddressRepository : IDisposable
    {
        Task<Address> GetByAddressAsync(Address address);

        Task DeleteWithoutPersonAsync();
    }
}