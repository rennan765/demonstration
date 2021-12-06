using _4oito6.Demonstration.Domain.Data.Repositories;

namespace _4oito6.Demonstration.Contact.Domain.Data.Repositories
{
    public interface IContactRepositoryRoot : IBaseRepositoryRoot
    {
        IPersonRepository Person { get; }

        IPhoneRepository Phone { get; }

        IAddressRepository Address { get; }
    }
}