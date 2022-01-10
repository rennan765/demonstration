using _4oito6.Demonstration.Application;
using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Services;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Application
{
    public class AuditTrailAppServices : AppServiceBase, IAuditTrailAppServices
    {
        private readonly IAuditTrailServices _services;

        public AuditTrailAppServices
        (
            IAuditTrailServices services,
            ILogger<AuditTrailAppServices> logger,
            IAuditTrailSender auditTrail
        ) : base(logger, auditTrail, new IDisposable[] { services, auditTrail })
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public Task ProcessMessageAsync(AuditTrailMessage message)
        {
            try
            {
                if (message is null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                return _services.ProcessAsync(message);
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(ex);
            }
        }

        public Task SendAsync(string code, string message, string additionalInformation = null)
        {
            try
            {
                return AuditTrail.SendAsync(code, message, additionalInformation);
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(ex);
            }
        }
    }
}