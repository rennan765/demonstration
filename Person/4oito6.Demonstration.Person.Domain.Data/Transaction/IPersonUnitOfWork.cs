using _4oito6.Demonstration.Domain.Data.Transaction;
using _4oito6.Demonstration.Person.Domain.Data.Repositories;

namespace _4oito6.Demonstration.Person.Domain.Data.Transaction
{
    public interface IPersonUnitOfWork : IPersonRepositoryRoot, IBaseUnitOfWork
    {
    }
}