using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Domain.Services
{
    public interface IAuditTrailServices : IDisposable
    {
        Task ProcessAsync(AuditTrailMessage message);
    }
}