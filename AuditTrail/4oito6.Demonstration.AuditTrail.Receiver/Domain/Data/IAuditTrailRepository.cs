using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Domain.Data
{
    public interface IAuditTrailRepository : IDisposable
    {
        Task InsertAsync(AuditTrailMessage message);
    }
}