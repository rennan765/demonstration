using _4oito6.Demonstration.Application;
using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Services;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        public Task ProcessMessageAsync(SQSEvent.SQSMessage message)
        {
            try
            {
                if (message is null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                var auditTrailMessage = JsonConvert.DeserializeObject<AuditTrailMessage>(message.Body);
                if (auditTrailMessage is null)
                {
                    throw new InvalidOperationException(nameof(message));
                }

                return _services.ProcessAsync(auditTrailMessage);
            }
            catch (Exception ex)
            {
                return HandleExceptionAsync(ex);
            }
        }
    }
}