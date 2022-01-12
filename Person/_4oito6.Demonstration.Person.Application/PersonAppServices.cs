using _4oito6.Demonstration.Application;
using _4oito6.Demonstration.CrossCutting.AuditTrail.Interface;
using _4oito6.Demonstration.Person.Application.Interfaces;
using _4oito6.Demonstration.Person.Application.Model.Person;
using _4oito6.Demonstration.Person.Domain.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Person.Application
{
    public class PersonAppServices : AppServiceBase, IPersonAppServices
    {
        private readonly IPersonServices _services;
        private readonly IPersonUnitOfWork _uow;

        public PersonAppServices
        (
            IPersonServices services,
            IPersonUnitOfWork uow,

            IAuditTrailSender auditTrail,
            ILogger<PersonAppServices> logger
        )
            : base(logger, auditTrail, new IDisposable[] { services, uow, auditTrail })
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<PersonResponse> CreateAsync(PersonRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _uow.EnableTransactions();

            var person = await _services
                .CreateAsync(request.ToPerson())
                .ConfigureAwait(false);

            Notify(person);

            if (IsValid)
            {
                _uow.Commit();
            }
            else
            {
                _uow.Rollback();
            }

            _uow.CloseConnections();
            return person?.ToPersonResponse();
        }

        public async Task<PersonResponse> GetByEmailAsync(string email)
        {
            var person = await _services.GetByEmailAsync(email).ConfigureAwait(false);
            _uow.CloseConnections();

            if (person is null)
            {
                HttpStatusCode = System.Net.HttpStatusCode.NotFound;
            }

            return person?.ToPersonResponse();
        }

        public async Task<PersonResponse> GetByIdAsync(int id)
        {
            var person = await _services.GetByIdAsync(id).ConfigureAwait(false);
            _uow.CloseConnections();

            if (person is null)
            {
                HttpStatusCode = System.Net.HttpStatusCode.NotFound;
            }

            return person?.ToPersonResponse();
        }

        public async Task<PersonResponse> UpdateAsync(int id, PersonRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _uow.EnableTransactions();

            var person = await _services
                .UpdateAsync(request.ToPerson(id))
                .ConfigureAwait(false);

            Notify(person);

            if (IsValid)
            {
                _uow.Commit();
            }
            else
            {
                _uow.Rollback();
            }

            _uow.CloseConnections();
            return person?.ToPersonResponse();
        }
    }
}