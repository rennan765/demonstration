using _4oito6.Demonstration.Domain.Model.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Data.Repositories
{
    public interface IPersonRepository : IDisposable
    {
        Task<IEnumerable<Person>> GetAllAsync();

        Task<Person> GetByIdAsync(int id);

        Task UpdateContactInformationAsync(Person person);

        Task CloneAsync(IEnumerable<Person> persons);
    }
}