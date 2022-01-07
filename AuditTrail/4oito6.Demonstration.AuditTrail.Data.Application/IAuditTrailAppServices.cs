using _4oito6.Demonstration.Application.Interfaces;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Application
{
    public interface IAuditTrailAppServices : IAppServiceBase
    {
        Task ProcessMessageAsync(AuditTrailMessage message);
    }
}