using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Services.Interfaces;
using _4oito6.Demonstration.Contact.Domain.Services.Validators;
using _4oito6.Demonstration.Domain.Model.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Services
{
    public class ContactServices : DisposableObject, IContactServices
    {
        private readonly IContactRepositoryRoot _repoRoot;

        public ContactServices(IContactRepositoryRoot repoRoot)
            : base(new IDisposable[] { repoRoot })
        {
            _repoRoot = repoRoot ?? throw new ArgumentNullException(nameof(repoRoot));
        }

        public async Task<Person> MaintainContactInformationAsync(int personId)
        {
            var person = await _repoRoot.Person.GetByIdAsync(personId).ConfigureAwait(false);
            if (person is null)
            {
                person = Person.GetDefaultInstance();
                person.Validate(person, new PersonNotFoundValidator());
                return person;
            }

            if (person.Address != null)
            {
                var address = await _repoRoot.Address.GetByAddressAsync(person.Address).ConfigureAwait(false);
                if (address != null)
                {
                    person.ClearAddress();
                    person.Attach(address);
                }
            }

            var phones = (await _repoRoot.Phone.GetByNumberAsync(person.Phones).ConfigureAwait(false))
                .ToDictionary(x => x.ToString());

            if (phones.Any())
            {
                var mainPhone = phones[person.MainPhone.ToString()];
                person.ClearPhones();

                person.Attach(mainPhone, isMainPhone: true);
                person.Attach(phones.Select(p => p.Value));
            }

            await _repoRoot.Person.UpdateContactInformationAsync(person).ConfigureAwait(false);
            await _repoRoot.Address.DeleteWithoutPersonAsync().ConfigureAwait(false);
            await _repoRoot.Phone.DeleteWithoutPersonAsync().ConfigureAwait(false);
            return person;
        }
    }
}