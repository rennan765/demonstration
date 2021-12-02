using _4oito6.Demonstration.Contact.Domain.Data.Repositories;
using _4oito6.Demonstration.Domain.Data.Transaction;

namespace _4oito6.Demonstration.Contact.Domain.Data.Transaction
{
    public interface IContactUnitOfWork : IBaseUnitOfWork, IContactRepositoryRoot
    {
    }
}