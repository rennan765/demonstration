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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Application
{
    public class ContactAppServices : AppServiceBase, IContactAppServices
    {
        private static readonly object _runCheckSyncRoot;
        private static readonly int _maintainInterval;
        private static DateTime? _lastMaintainCheck;

        private static readonly int _cloneInterval;
        private static DateTime? _lastCloneCheck;

        private readonly IContactServices _contact;
        private readonly ICloningServices _cloning;

        private readonly IContactUnitOfWork _uow;
        private readonly ISQSHelper _sqs;

        static ContactAppServices()
        {
            _maintainInterval = (int)TimeSpan.FromMinutes(1).TotalSeconds;
            _cloneInterval = (int)TimeSpan.FromDays(3).TotalSeconds;
            _runCheckSyncRoot = new object();
        }

        public ContactAppServices
        (
            IContactServices contact,
            ICloningServices cloning,

            IContactUnitOfWork uow,
            ISQSHelper sqs,

            ILogger<ContactAppServices> logger,
            IAuditTrailSender auditTrail
        ) : base(logger, auditTrail, new IDisposable[] { contact, cloning, uow })
        {
            _contact = contact ?? throw new ArgumentNullException(nameof(contact));
            _cloning = cloning ?? throw new ArgumentNullException(nameof(cloning));

            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _sqs = sqs ?? throw new ArgumentNullException(nameof(sqs));
        }

        public static string MaintainInformationMessage => "CONTACT_IN01";

        public static string CloneInformationMessage => "CONTACT_IN02";

        public static string WarningMessage => "CONTACT_WA01";

        [ExcludeFromCodeCoverage]
        private bool IsAbleToRun(DateTime? lastRunCheck, int runInterval)
        {
            var now = DateTime.UtcNow;

            if (lastRunCheck is null || now.Subtract(lastRunCheck.Value).TotalSeconds >= runInterval)
            {
                lock (_runCheckSyncRoot)
                {
                    if (lastRunCheck is null || now.Subtract(lastRunCheck.Value).TotalSeconds >= runInterval)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [ExcludeFromCodeCoverage]
        public virtual bool IsAbleToMaintain()
        {
            if (IsAbleToRun(_lastMaintainCheck, _maintainInterval))
            {
                _lastMaintainCheck = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        [ExcludeFromCodeCoverage]
        public virtual bool IsAbleToClone()
        {
            if (IsAbleToRun(_lastCloneCheck, _cloneInterval))
            {
                _lastCloneCheck = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        public virtual async Task MaintainInformationAsync(int personId)
        {
            var message = new AuditTrailMessage();
            _uow.EnableTransactions();

            var person = await _contact.MaintainContactInformationAsync(personId)
                .ConfigureAwait(false);

            Notify(person);
            if (IsValid)
            {
                _uow.Commit();

                message.Code = MaintainInformationMessage;
                message.Message = "Informações de contato atualizadas com sucesso.";
                Logger.LogInformation(message.Message);

                message.AdditionalInformation = $"PhoneIds: {string.Join(",", person.Phones.Select(p => p.Id))}. ";
                message.AdditionalInformation += $"MainPhoneId: {person.MainPhone.Id}. ";

                if (person.Address != null)
                {
                    message.AdditionalInformation += $"AddressId: {person.Address.Id}. ";
                }
            }
            else
            {
                _uow.Rollback();

                message.Code = WarningMessage;
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

        public async Task MaintainInformationByQueueAsync()
        {
            try
            {
                if (!IsAbleToMaintain())
                {
                    return;
                }

                var request = await _sqs.GetAsync<MaintainInformationRequest>().ConfigureAwait(false);
                if (request is null)
                {
                    HandleEmptyQueue();
                    return;
                }

                await MaintainInformationAsync(request.PersonId).ConfigureAwait(false);
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

        public async Task CloneAsync()
        {
            try
            {
                if (!IsAbleToClone())
                {
                    HandleEmptyQueue("Aguardando agendamento para rodar o clone.");
                    return;
                }

                Logger.LogInformation($"Processamento de clone iniciado na data UTC {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}.");

                _uow.EnableTransactions();
                await _cloning.CloneAsync().ConfigureAwait(false);
                _uow.Commit();

                var message = $"Processamento de clone finalizado na data UTC {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}.";
                await AuditTrail.SendAsync(CloneInformationMessage, message).ConfigureAwait(false);

                Logger.LogInformation(message);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex).ConfigureAwait(false);
            }
            finally
            {
                _uow.Rollback();
                _uow.CloseConnections();
            }
        }
    }
}