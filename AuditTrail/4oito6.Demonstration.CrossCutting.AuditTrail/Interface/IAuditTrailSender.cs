using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.CrossCutting.AuditTrail.Interface
{
    public interface IAuditTrailSender : IDisposable
    {
        Task SendAsync(string code, string message, string additionalInformation = null);

        Task SendAsync(AuditTrailMessage message);
    }
}