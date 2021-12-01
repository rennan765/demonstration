using _4oito6.Demonstration.Application.Interfaces;
using Amazon.Lambda.SQSEvents;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Application
{
    public interface IAuditTrailAppServices : IAppServiceBase
    {
        Task ProcessMessageAsync(SQSEvent.SQSMessage message);
    }
}