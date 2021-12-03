using _4oito6.Demonstration.Application.Interfaces;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Application.Interfaces
{
    public interface IContactAppServices : IAppServiceBase
    {
        Task MaintainInformationByPersonIdAsync(int personId);

        Task MaintainInformationByQueueAsync(int personId);
    }
}