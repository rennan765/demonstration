using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Person.Domain.Services.Interfaces
{
    using _4oito6.Demonstration.Domain.Model.Entities;

    public interface IPersonServices : IDisposable
    {
        Task<Person> GetByIdAsync(int id);

        Task<Person> CreateAsync(Person person);

        Task<Person> UpdateAsync(Person person);
    }
}