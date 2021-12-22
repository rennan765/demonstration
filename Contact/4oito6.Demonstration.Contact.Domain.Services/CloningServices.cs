using _4oito6.Demonstration.Commons;
using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Contact.Domain.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _4oito6.Demonstration.Contact.Domain.Services
{
    public class CloningServices : DisposableObject, ICloningServices
    {
        private readonly IContactRepositoryRoot _repoRoot;

        public CloningServices(IContactRepositoryRoot repoRoot)
            : base(new IDisposable[] { repoRoot })
        {
            _repoRoot = repoRoot ?? throw new ArgumentNullException(nameof(repoRoot));
        }

        public async Task CloneAsync()
        {
            var addresses = await _repoRoot.Address.GetAllAsync().ConfigureAwait(false);
            if (addresses.Any())
            {
                await _repoRoot.Address.CloneAsync(addresses).ConfigureAwait(false);
            }

            var phones = await _repoRoot.Phone.GetAllAsync().ConfigureAwait(false);
            if (addresses.Any())
            {
                await _repoRoot.Phone.CloneAsync(phones).ConfigureAwait(false);
            }

            var persons = await _repoRoot.Person.GetAllAsync().ConfigureAwait(false);
            if (persons.Any())
            {
                await _repoRoot.Person.CloneAsync(persons).ConfigureAwait(false);
            }
        }
    }
}