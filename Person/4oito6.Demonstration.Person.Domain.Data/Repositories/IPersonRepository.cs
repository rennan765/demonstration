using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Person.Domain.Data.Repositories
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    public interface IPersonRepository : IDisposable
    {
        Task<Person> GetByIdAsync(int id);

        Task<Person> GetByEmailAsync(string email);

        Task<Person> GetByEmailOrDocumentAsync(string email, string document);

        Task<Person> InsertAsync(Person person);

        Task<Person> UpdateAsync(Person person);
    }
}