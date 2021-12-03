using _4oito6.Demonstration.Domain.Data.Repositories;
using _4oito6.Demonstration.Person.Domain.Data.Repositories.NonTransactional;

namespace _4oito6.Demonstration.Person.Domain.Data.Repositories
{
    public interface IPersonRepositoryRoot : IBaseRepositoryRoot
    {
        IPersonNonTransactionRepositoryRoot NonTransactional { get; }

        IPersonRepository Person { get; }
    }
}