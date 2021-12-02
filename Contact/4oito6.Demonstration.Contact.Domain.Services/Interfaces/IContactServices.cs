using _4oito6.Demonstration.Domain.Model.Entities;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Services.Interfaces
{
    public interface IContactServices : IDisposable
    {
        public Task<Person> MaintainContactInformationAsync(int personId);
    }
}