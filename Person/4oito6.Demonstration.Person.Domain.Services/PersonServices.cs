using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Person.Domain.Services
{
    using _4oito6.Demonstration.Domain.Model.Entities;
    using _4oito6.Demonstration.Domain.Model.Validators;

    public class PersonServices : DisposableObject, IPersonServices
    {
        private readonly IPersonRepositoryRoot _repoRoot;

        public PersonServices(IPersonRepositoryRoot repoRoot)
            : base(new IDisposable[] { repoRoot })
        {
            _repoRoot = repoRoot ?? throw new ArgumentNullException(nameof(repoRoot));
        }

        public async Task<Person> CreateAsync(Person person)
        {
            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            var byEmailOrDocument = await _repoRoot.Person
                .GetByEmailOrDocumentAsync(person.Email, person.Document)
                .ConfigureAwait(false);

            if (byEmailOrDocument != null)
            {
                person.Validate(person, new EmailDocumentPersonValidator());
            }

            if (!person.IsValid)
            {
                return person;
            }

            var newPerson = await _repoRoot.Person.InsertAsync(person).ConfigureAwait(false);
            await _repoRoot.Person.RequestMaintainContactInformationAsync(newPerson).ConfigureAwait(false);
            return newPerson;
        }

        public Task<Person> GetByEmailAsync(string email)
            => _repoRoot.Person.GetByEmailAsync(email);

        public Task<Person> GetByIdAsync(int id)
            => _repoRoot.Person.GetByIdAsync(id);

        public async Task<Person> UpdateAsync(Person person)
        {
            if (person is null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            var byId = await _repoRoot.Person.GetByIdAsync(person.Id).ConfigureAwait(false);
            if (byId is null)
            {
                person.Validate(person, new NotFoundPersonValidator());
            }

            person.ValidateToUpdate();

            if (!person.IsValid)
            {
                return person;
            }

            var updatedPerson = await _repoRoot.Person.UpdateAsync(person).ConfigureAwait(false);
            await _repoRoot.Person.RequestMaintainContactInformationAsync(updatedPerson).ConfigureAwait(false);
            return updatedPerson;
        }
    }
}