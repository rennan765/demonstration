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

        public async Task ProcessMessageAsync(AuditTrailMessage message)
        {
            try
            {
                if (message is null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                await _services.ProcessAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex).ConfigureAwait(false);
            }
        }

        public async Task SendAsync(string code, string message, string additionalInformation = null)
        {
            try
            {
                await AuditTrail.SendAsync(code, message, additionalInformation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex).ConfigureAwait(false);
            }
        }
    }
}