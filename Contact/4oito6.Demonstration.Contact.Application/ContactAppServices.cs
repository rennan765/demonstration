using _4oito6.Demonstration.Application;
using _4oito6.Demonstration.Contact.Application.Arguments;
using _4oito6.Demonstration.Contact.Application.Interfaces;
using _4oito6.Demonstration.Contact.Domain.Data.Transaction;
using _4oito6.Demonstration.Contact.Domain.Services.Interfaces;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Model;
using _4oito6.Demonstration.SQS.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Application
{
    public class ContactAppServices : AppServiceBase, IContactAppServices
    {
        private readonly IContactServices _services;
        private readonly IContactUnitOfWork _uow;
        private readonly ISQSHelper _sqs;

        public ContactAppServices
        (
            IContactServices service,
            IContactUnitOfWork uow,
            ISQSHelper sqs,

            ILogger<ContactAppServices> logger,
            IAuditTrailSender auditTrail
        ) : base(logger, auditTrail, new IDisposable[] { service, uow })
        {
            _services = service ?? throw new ArgumentNullException(nameof(service));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
        }

        public static string InformationMessage => "CONTACT_IN01";

        public static string WarningMessage => "CONTACT_WA01";

        public virtual async Task MaintainInformationAsync(int personId)
        {
            var message = new AuditTrailMessage();
            _uow.EnableTransactions();

            var person = await _services.MaintainContactInformationAsync(personId)
                .ConfigureAwait(false);

            Notify(person);
            if (IsValid)
            {
                _uow.Commit();

                message.Code = InformationMessage;
                message.Message = "Informações de contato atualizadas com sucesso.";
                Logger.LogWarning(message.Message);

                message.AdditionalInformation = $"PhoneIds: {string.Join(",", person.Phones.Select(p => p.Id))}. ";
                message.AdditionalInformation += $"MainPhoneId: {person.MainPhone.Id}";

                if (person.Address != null)
                {
                    message.AdditionalInformation += $"AddressId: {person.Address.Id}";
                }
            }
            else
            {
                _uow.Rollback();

                message.Code = InformationMessage;
                message.Message = "Falha ao realizar a manutenção das informações de contato.";

                Logger.LogWarning(message.Message);
                message.AdditionalInformation = JsonConvert.SerializeObject(Notifications);
            }

            await AuditTrail.SendAsync(message).ConfigureAwait(false);
        }

        public async Task MaintainInformationByPersonIdAsync(int personId)
        {
            try
            {
                await MaintainInformationAsync(personId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex).ConfigureAwait(false);
            }
            finally
            {
                _uow.CloseConnections();
            }
        }

        public async Task MaintainInformationByQueueAsync(int personId)
        {
            try
            {
                var request = await _sqs.GetAsync<MaintainInformationRequest>().ConfigureAwait(false);
                if (request is null)
                {
                    HandleEmptyQueue();
                    return;
                }

                await MaintainInformationAsync(personId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex).ConfigureAwait(false);
            }
            finally
            {
                _uow.CloseConnections();
            }
        }
    }
}