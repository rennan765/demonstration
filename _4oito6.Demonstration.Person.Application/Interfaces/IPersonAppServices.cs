using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.Person.Application.Model.Person;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Person.Application.Interfaces
{
    public interface IPersonAppServices : IAppServiceBase
    {
        Task<PersonResponse> GetByIdAsync(int id);

        Task<PersonResponse> CreateAsync(PersonRequest request);

        Task<PersonResponse> UpdateAsync(int id, PersonRequest request);
    }
}