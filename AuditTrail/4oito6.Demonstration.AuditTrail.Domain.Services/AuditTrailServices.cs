using _4oito6.Demonstration.AuditTrail.Receiver.Domain.Data;
using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.AuditTrail.Receiver.Domain.Services
{
    public class AuditTrailServices : DisposableObject, IAuditTrailServices
    {
        private readonly IAuditTrailRepository _repository;

        public AuditTrailServices(IAuditTrailRepository repository)
            : base(new IDisposable[] { repository })
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public static string DefautlCode => "AUDITTRAIL_WA_01";
        public static string DefaultMessage => "Código genérido de trilha de auditoria adicionado.";

        public Task ProcessAsync(AuditTrailMessage message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (string.IsNullOrEmpty(message.Code))
            {
                message.Code = DefautlCode;
                message.Message += DefaultMessage;
            }

            return _repository.InsertAsync(message);
        }
    }
}